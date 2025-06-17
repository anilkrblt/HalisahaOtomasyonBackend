using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IServiceManager _serviceManager;

        public AuthController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }


        // Oturum açmış bir kullanıcı diğer kullanıcıların profilini görüntüler
        [HttpGet("/user/info/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetUserInformation(int id)
        {
            var idClaim = User.FindFirst("id");
            if (idClaim == null)
                return Unauthorized("Kullanıcı id bilgisi bulunamadı!");

            int callerId = int.Parse(idClaim.Value);

            var userObject = await _serviceManager.AuthService.GetUserAsync(id);
            if (userObject is null) return NotFound();

            if (userObject is CustomerDto cust)
            {
                cust.Status = await _serviceManager.FriendshipService
                    .GetRelationshipStatusAsync(callerId, id);

                return Ok(cust);
            }

            return Ok(userObject);

        }

        // Oturum açmış bir Kullanıcı kendi profilini görüntüler
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            var idClaim = User.FindFirst("id");
            if (idClaim == null)
                return Unauthorized("Kullanıcı id bilgisi bulunamadı!");

            if (!int.TryParse(idClaim.Value, out int userId))
                return Unauthorized("Geçersiz kullanıcı id!");

            if (userId != id)
                return Forbid("Kendi dışındaki kullanıcıyı görüntüleyemezsin!");

            var dto = await _serviceManager.AuthService.GetUserAsync(id);

            if (dto is null)
                return NotFound("Kullanıcı bulunamadı!");

            return Ok(dto);
        }


        // Müşteri kullanıcı için bilgilerini güncelleme
        [Authorize(Roles = "Customer")]
        [HttpPut("customer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerUpdateDto dto)
        {
            int id = int.Parse(User.FindFirst("id")!.Value);
            var res = await _serviceManager.AuthService.UpdateCustomerAsync(id, dto);
            return res.Succeeded ? NoContent() : UnprocessableEntity(res.Errors);
        }

        // Müşteri kullanıcı için patch ile kısmi güncelleme
        [Authorize(Roles = "Customer")]
        [HttpPatch("customer")]
        public async Task<IActionResult> PatchCustomer([FromBody] JsonPatchDocument<CustomerPatchDto> patch)
        {
            if (patch is null) return BadRequest();
            int id = int.Parse(User.FindFirst("id")!.Value);
            var res = await _serviceManager.AuthService.PatchCustomerAsync(id, patch);
            return res.Succeeded ? NoContent() : UnprocessableEntity(res.Errors);
        }

        // Halısaha sahibi için güncelleme
        [Authorize(Roles = "Owner")]
        [HttpPut("owner")]
        public async Task<IActionResult> UpdateOwner([FromBody] OwnerUpdateDto dto)
        {
            int id = int.Parse(User.FindFirst("id")!.Value);
            var res = await _serviceManager.AuthService.UpdateOwnerAsync(id, dto);
            return res.Succeeded ? NoContent() : UnprocessableEntity(res.Errors);
        }

        // Halısaha sahibi için kısmi güncelleme
        [Authorize(Roles = "Owner")]
        [HttpPatch("owner")]
        public async Task<IActionResult> PatchOwner([FromBody] JsonPatchDocument<OwnerPatchDto> patch)
        {
            if (patch is null) return BadRequest();
            int id = int.Parse(User.FindFirst("id")!.Value);
            var res = await _serviceManager.AuthService.PatchOwnerAsync(id, patch);
            return res.Succeeded ? NoContent() : UnprocessableEntity(res.Errors);
        }

        // Kullanıcı silme
        [HttpDelete("{id:int?}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);

            if (userId != id)
                return Forbid();

            var res = await _serviceManager.AuthService.DeleteUserAsync(userId);
            return res.Succeeded ? NoContent() : UnprocessableEntity(res.Errors);
        }


        // Halısaha sahibi kayıt olma
        [HttpPost("register-owner")]
        public async Task<IActionResult> RegisterOwner([FromBody] RegisterOwnerDto model)
        {
            var result = await _serviceManager.AuthService.RegisterOwnerAsync(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Owner registered successfully.");
        }


        // Müşteri kayıt olma
        [HttpPost("register-customer")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterCustomer([FromForm] CustomerRegisterDto model)
        {
            var result = await _serviceManager.AuthService.RegisterCustomerAsync(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);


            return Ok("Customer registered successfully.");
        }

        // Halısaha sahbi ve müşteri için oturum açma işlemi
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            var userId = await _serviceManager.AuthService.LoginAsync(model);
            if (userId == null)
            {
                return Unauthorized("Başarısız giriş denemesi!");
            }

            return Ok(new { Token = userId });
        }

        // Oturum açıkken şifre güncelleme işlemi
        [Authorize]
        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto model)
        {
            var userId = User?.FindFirst("id")?.Value;
            if (userId == null)
                return Unauthorized("User not authenticated.");

            var result = await _serviceManager.AuthService.UpdatePasswordAsync(model, userId);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password updated successfully.");
        }


        // Şifre unutma durumunda şifre sıfırlamak için e postaya kod gönderme işlemi
        [HttpPost("forgot-password")]
        public async Task<IActionResult> SendForgotPasswordCode([FromBody] ForgotPasswordRequestDto model)
        {
            var success = await _serviceManager.AuthService.SendForgotPasswordCodeAsync(model.Email);
            if (!success)
            {
                return BadRequest("User with this email does not exist.");
            }

            return Ok("Reset code sent to your email address.");
        }

        // Kod ve e posta ile şifreyi değiştirme
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordWithCode([FromBody] ResetPasswordDto model)
        {
            var result = await _serviceManager.AuthService.ResetPasswordWithCodeAsync(model.Email, model.Code, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password has been reset successfully.");
        }


        // Kullanıcı profil fotoğrafı güncelleme (müşteri için önerilir post + put işlemi)
        [HttpPost("{id:int}/photos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateUserPhoto(int id, [FromForm] UserPhotosUpdateDto dto)
        {
            if (dto.PhotoFiles == null || dto.PhotoFiles.Count == 0)
                return BadRequest("Fotoğraf yüklenmedi.");

            if (dto.PhotoFiles.Count > 1)
                return BadRequest("En fazla 1 fotoğraf yükleyebilirsiniz.");

            var photos = await _serviceManager.PhotoService.GetPhotosAsync("user", id, trackChanges: true);

            if (photos == null || !photos.Any())
            {
                await _serviceManager.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "user", id);
            }
            else
            {
                await _serviceManager.PhotoService.DeletePhotosByEntityAsync("user", id, trackChanges: true);
                await _serviceManager.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "user", id);
            }

            return NoContent();
        }


    }
}