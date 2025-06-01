using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTransferObjects;
using StackExchange.Redis;
using MimeKit;
using MailKit.Security;

namespace Service;

public class AuthService : IAuthService
{
    /* ───── DI alanları ───── */
    private readonly UserManager<ApplicationUser>      _userMgr;
    private readonly SignInManager<ApplicationUser>    _signInMgr;
    private readonly RoleManager<IdentityRole<int>>    _roleMgr;
    private readonly IConfiguration                    _cfg;
    private readonly IConnectionMultiplexer            _redis;

    public AuthService(UserManager<ApplicationUser>   userMgr,
                       SignInManager<ApplicationUser> signInMgr,
                       IConfiguration                 cfg,
                       RoleManager<IdentityRole<int>> roleMgr,
                       IConnectionMultiplexer         redis)
    {
        _userMgr  = userMgr;
        _signInMgr= signInMgr;
        _cfg      = cfg;
        _roleMgr  = roleMgr;
        _redis    = redis;
    }

    /*────────────────────────  ŞİFRE SIFIRLAMA  ───────────────────────*/
    public async Task<bool> SendForgotPasswordCodeAsync(string email)
    {
        var user = await _userMgr.FindByEmailAsync(email);
        if (user is null) return false;

        var code = new Random().Next(100000, 999999).ToString();
        await _redis.GetDatabase()
                    .StringSetAsync($"resetcode:{email}", code, TimeSpan.FromMinutes(3));

        await SendEmailAsync(email, "Password Reset Code", $"Your code is: {code}");
        return true;
    }

    public async Task<IdentityResult> ResetPasswordWithCodeAsync(
            string email, string code, string newPassword)
    {
        var user = await _userMgr.FindByEmailAsync(email);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        var db  = _redis.GetDatabase();
        var val = await db.StringGetAsync($"resetcode:{email}");
        if (val.IsNullOrEmpty)   return IdentityResult.Failed(
                                    new IdentityError { Description = "Code expired." });
        if (val != code)         return IdentityResult.Failed(
                                    new IdentityError { Description = "Invalid code." });

        var token  = await _userMgr.GeneratePasswordResetTokenAsync(user);
        var result = await _userMgr.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded) await db.KeyDeleteAsync($"resetcode:{email}");
        return result;
    }

    /*────────────────────────────  KAYIT  ────────────────────────────*/
    public async Task<IdentityResult> RegisterOwnerAsync(UserRegisterDto dto)
    {
        /* Rol yoksa ekle */
        if (!await _roleMgr.RoleExistsAsync("Owner"))
            await _roleMgr.CreateAsync(new IdentityRole<int>("Owner"));

        var owner = new Owner
        {
            UserName   = dto.UserName,
            Email      = dto.Email,
            FirstName  = dto.FirstName,
            LastName   = dto.LastName,
            Town       = dto.Town,
            City       = dto.City,
            Birthday   = dto.Birthday.Date
        };

        var result = await _userMgr.CreateAsync(owner, dto.Password);
        if (result.Succeeded)
            await _userMgr.AddToRoleAsync(owner, "Owner");

        return result;
    }

    public async Task<IdentityResult> RegisterCustomerAsync(CustomerRegisterDto dto)
    {
        if (!await _roleMgr.RoleExistsAsync("Customer"))
            await _roleMgr.CreateAsync(new IdentityRole<int>("Customer"));

        var customer = new Customer
        {
            UserName       = dto.UserName,
            Email          = dto.Email,
            FirstName      = dto.FirstName,
            LastName       = dto.LastName,
            Town           = dto.Town,
            City           = dto.City,
            Birthday       = dto.Birthday.Date,
            FootPreference = (Entities.Models.FootPreference)dto.FootPreference,
            Height         = dto.Height,
            Weight         = dto.Weight,
            Gender         = dto.Gender,
            Positions      = dto.PlayingPosition ?? string.Empty
        };

        var result = await _userMgr.CreateAsync(customer, dto.Password);
        if (result.Succeeded)
            await _userMgr.AddToRoleAsync(customer, "Customer");

        return result;
    }

    /*────────────────────────────  GİRİŞ  ────────────────────────────*/
    public async Task<string?> LoginAsync(UserLoginDto dto)
    {
        var user = await _userMgr.FindByEmailAsync(dto.Email);
        if (user is null) return null;

        var ok = await _signInMgr.CheckPasswordSignInAsync(user, dto.Password, false);
        return ok.Succeeded ? await CreateTokenAsync(user) : null;
    }

    public async Task<IdentityResult> UpdatePasswordAsync(
            UpdatePasswordDto dto, string userId)
    {
        var user = await _userMgr.FindByIdAsync(userId);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await _userMgr.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
    }

    /*────────────────────────────  JWT  ──────────────────────────────*/
    private async Task<string> CreateTokenAsync(ApplicationUser user)
    {
        var creds  = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")!)),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,  user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new("id",             user.Id.ToString())
        };
        var roles = await _userMgr.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var jwtOpt = _cfg.GetSection("JwtSettings");
        var token = new JwtSecurityToken(
            issuer:  jwtOpt["validIssuer"],
            audience:jwtOpt["validAudience"],
            claims:  claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtOpt["expires"]!)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /*────────────────────  HELPERS  ────────────────────*/
    private static async Task SendEmailAsync(string to, string subj, string body)
    {
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress("Halisaha", "karabulutanil54@gmail.com"));
        msg.To.Add(MailboxAddress.Parse(to));
        msg.Subject = subj;
        msg.Body = new TextPart("plain") { Text = body };

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("karabulutanil54@gmail.com", "irle yvfw ooxp kgmg");
        await smtp.SendAsync(msg);
        await smtp.DisconnectAsync(true);
    }
}
