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
using Microsoft.AspNetCore.JsonPatch;


namespace Service;

public class AuthService : IAuthService
{

    /* ───── DI alanları ───── */
    private readonly UserManager<ApplicationUser> _userMgr;
    private readonly SignInManager<ApplicationUser> _signInMgr;
    private readonly RoleManager<IdentityRole<int>> _roleMgr;
    private readonly IConfiguration _cfg;
    private readonly IConnectionMultiplexer _redis;
    private readonly IPhotoService _photoService;

    public AuthService(UserManager<ApplicationUser> userMgr,
                       SignInManager<ApplicationUser> signInMgr,
                       IConfiguration cfg,
                       RoleManager<IdentityRole<int>> roleMgr,
                       IConnectionMultiplexer redis,
                       IPhotoService photoService)
    {
        _userMgr = userMgr;
        _signInMgr = signInMgr;
        _cfg = cfg;
        _roleMgr = roleMgr;
        _redis = redis;
        _photoService = photoService;
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

        var db = _redis.GetDatabase();
        var val = await db.StringGetAsync($"resetcode:{email}");
        if (val.IsNullOrEmpty) return IdentityResult.Failed(
                                    new IdentityError { Description = "Code expired." });
        if (val != code) return IdentityResult.Failed(
                                    new IdentityError { Description = "Invalid code." });

        var token = await _userMgr.GeneratePasswordResetTokenAsync(user);
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
            UserName = dto.UserName,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Town = dto.Town,
            City = dto.City,
            Birthday = dto.Birthday.Date
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
            UserName = dto.UserName,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Town = dto.Town,
            City = dto.City,
            Birthday = dto.Birthday.Date,
            FootPreference = (Entities.Models.FootPreference)dto.FootPreference,
            Height = dto.Height,
            Weight = dto.Weight,
            Gender = dto.Gender,
            Positions = dto.PlayingPosition ?? string.Empty
        };

        var result = await _userMgr.CreateAsync(customer, dto.Password);
        if (result.Succeeded)
            await _userMgr.AddToRoleAsync(customer, "Customer");


        /*
        var photos = await _photoService.GetPhotosAsync("user", customer.Id, trackChanges: true);

        if (photos == null || !photos.Any())
        {
            await _photoService.UploadPhotosAsync(dto.PhotoFiles, "user", customer.Id);
        } */

        if (dto.PhotoFiles != null && dto.PhotoFiles.Any())
        {
            await _photoService.UploadPhotosAsync(dto.PhotoFiles, "user", customer.Id);
        }

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
        var creds = new SigningCredentials(
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
            issuer: jwtOpt["validIssuer"],
            audience: jwtOpt["validAudience"],
            claims: claims,
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

    public async Task<object?> GetUserAsync(int userId)
    {
        // 1. User'ı getir
        var user = await _userMgr.FindByIdAsync(userId.ToString());
        if (user is null)
            return null;   // Controller’da NotFound olarak işleyin


        // 2. Rolleri al
        var roles = await _userMgr.GetRolesAsync(user);


        Console.WriteLine(roles[0]);
        // 3. Role göre DTO’ya manuel map
        if (roles.Contains("Owner"))
        {
            Console.WriteLine("Owner bölümü");
            // ApplicationUser’dan OwnerDto’ya map
            var owner = user as Owner;

            return new OwnerDto
            {
                FirstName = owner!.FirstName,
                LastName = owner.LastName,
                UserName = owner.UserName!,
                Email = owner.Email!,
                Role = Shared.DataTransferObjects.Role.Owner,
                City = owner.City!,
                Town = owner.Town!,
                Birthday = (DateTime)owner.Birthday,
            };
        }

        if (roles.Contains("Customer"))
        {
            var photos = await _photoService.GetPhotosAsync("user", userId, true);
            var photo = photos.FirstOrDefault();
            // ApplicationUser’dan CustomerDto’ya map
            Console.WriteLine("ÖNCE");
            var cust = user as Customer;
            Console.WriteLine("SONRA");
            return new CustomerDto
            {
                FirstName = cust!.FirstName ?? "",
                LastName = cust.LastName ?? "",
                UserName = cust.UserName!,
                Email = cust.Email!,
                Role = Shared.DataTransferObjects.Role.Customer,
                PhotoUrl = photo?.Url ?? "",
                City = cust.City!,
                Town = cust.Town!,
                Birthday = (DateTime)cust.Birthday,
                FootPreference = (Shared.DataTransferObjects.FootPreference)cust.FootPreference,
                Height = cust.Height,
                Weight = cust.Weight,
                PlayingPosition = string.IsNullOrWhiteSpace(cust.Positions)
                                  ? null
                                  : cust.Positions,
                Gender = cust.Gender
            };

        }

        return null;
    }

    public async Task<IdentityResult> UpdateCustomerAsync(int userId, CustomerUpdateDto dto)
    {
        // 1) Kullanıcıyı getir
        var user = await _userMgr.FindByIdAsync(userId.ToString()) as Customer;
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "Customer not found." });

        // 2) Alanları manuel map et
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.City = dto.City;
        user.Town = dto.Town;
        user.Birthday = dto.Birthday;
        user.FootPreference = (Entities.Models.FootPreference)dto.FootPreference;
        user.Height = dto.Height;
        user.Weight = dto.Weight;
        user.Gender = dto.Gender;
        user.Positions = dto.PlayingPosition ?? string.Empty;

        // 3) Kaydet
        return await _userMgr.UpdateAsync(user);
    }

    public async Task<IdentityResult> PatchCustomerAsync(int userId, JsonPatchDocument<CustomerPatchDto> patchDoc)
    {
        var user = await _userMgr.FindByIdAsync(userId.ToString()) as Customer;
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "Customer not found." });

        // 1) Entity → DTO
        var dto = new CustomerPatchDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName!,
            Email = user.Email!,
            Role = Shared.DataTransferObjects.Role.Customer,
            City = user.City!,
            Town = user.Town!,
            Birthday = (DateTime)user.Birthday,
            FootPreference = (Shared.DataTransferObjects.FootPreference)user.FootPreference,
            Height = user.Height,
            Weight = user.Weight,
            PlayingPosition = string.IsNullOrWhiteSpace(user.Positions) ? null : user.Positions,
            Gender = user.Gender
        };

        patchDoc.ApplyTo(dto);

        // 3) DTO → Entity
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.City = dto.City;
        user.Town = dto.Town;
        user.Birthday = dto.Birthday;
        user.FootPreference = (Entities.Models.FootPreference)dto.FootPreference;
        user.Height = dto.Height;
        user.Weight = dto.Weight;
        user.Gender = dto.Gender;
        user.Positions = dto.PlayingPosition ?? string.Empty;

        // 4) Kaydet
        return await _userMgr.UpdateAsync(user);
    }

    public async Task<IdentityResult> UpdateOwnerAsync(int userId, OwnerUpdateDto dto)
    {
        var user = await _userMgr.FindByIdAsync(userId.ToString()) as Owner;
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "Owner not found." });

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.City = dto.City;
        user.Town = dto.Town;
        user.Birthday = dto.Birthday;

        return await _userMgr.UpdateAsync(user);
    }

    public async Task<IdentityResult> PatchOwnerAsync(
        int userId, JsonPatchDocument<OwnerPatchDto> patchDoc)
    {
        var user = await _userMgr.FindByIdAsync(userId.ToString()) as Owner;
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "Owner not found." });

        var dto = new OwnerPatchDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName!,
            Email = user.Email!,
            Role = Shared.DataTransferObjects.Role.Owner,
            City = user.City!,
            Town = user.Town!,
            Birthday = (DateTime)user.Birthday,
        };

        patchDoc.ApplyTo(dto);

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.City = dto.City;
        user.Town = dto.Town;
        user.Birthday = dto.Birthday;

        return await _userMgr.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeleteUserAsync(int userId)
    {
        // 1) Kullanıcıyı bul
        var user = await _userMgr.FindByIdAsync(userId.ToString());
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        // 2) Sil
        var result = await _userMgr.DeleteAsync(user);

        // 3) Sonuç
        return result;
    }

}
