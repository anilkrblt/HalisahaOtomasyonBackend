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



        /*──── 1. PROFİL GET ─────────────────────────*/
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);
            if (userId != id) return Forbid();

            var dto = await _serviceManager.AuthService.GetUserAsync(id);
            
            if (dto is null) return NotFound();
            return Ok(dto);
        }

        /*──── 2. CUSTOMER PUT & PATCH ───────────────*/
        [Authorize(Roles = "Customer")]
        [HttpPut("customer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerUpdateDto dto)
        {
            int id = int.Parse(User.FindFirst("id")!.Value);
            var res = await _serviceManager.AuthService.UpdateCustomerAsync(id, dto);
            return res.Succeeded ? NoContent() : UnprocessableEntity(res.Errors);
        }

        [Authorize(Roles = "Customer")]
        [HttpPatch("customer")]
        public async Task<IActionResult> PatchCustomer([FromBody] JsonPatchDocument<CustomerPatchDto> patch)
        {
            if (patch is null) return BadRequest();
            int id = int.Parse(User.FindFirst("id")!.Value);
            var res = await _serviceManager.AuthService.PatchCustomerAsync(id, patch);
            return res.Succeeded ? NoContent() : UnprocessableEntity(res.Errors);
        }

        /*──── 3. OWNER PUT & PATCH ──────────────────*/
        [Authorize(Roles = "Owner")]
        [HttpPut("owner")]
        public async Task<IActionResult> UpdateOwner([FromBody] OwnerUpdateDto dto)
        {
            int id = int.Parse(User.FindFirst("id")!.Value);
            var res = await _serviceManager.AuthService.UpdateOwnerAsync(id, dto);
            return res.Succeeded ? NoContent() : UnprocessableEntity(res.Errors);
        }

        [Authorize(Roles = "Owner")]
        [HttpPatch("owner")]
        public async Task<IActionResult> PatchOwner([FromBody] JsonPatchDocument<OwnerPatchDto> patch)
        {
            if (patch is null) return BadRequest();
            int id = int.Parse(User.FindFirst("id")!.Value);
            var res = await _serviceManager.AuthService.PatchOwnerAsync(id, patch);
            return res.Succeeded ? NoContent() : UnprocessableEntity(res.Errors);
        }

        [HttpDelete("{id:int?}")]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);

            if (userId != id)
                return Forbid();

            var res = await _serviceManager.AuthService.DeleteUserAsync(userId);
            return res.Succeeded ? NoContent() : UnprocessableEntity(res.Errors);
        }



        [HttpPost("register-owner")]
        public async Task<IActionResult> RegisterOwner([FromBody] UserRegisterDto model)
        {
            var result = await _serviceManager.AuthService.RegisterOwnerAsync(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Owner registered successfully.");
        }

        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterDto model)
        {
            var result = await _serviceManager.AuthService.RegisterCustomerAsync(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Customer registered successfully.");
        }


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



        [HttpPost("{id:int}/photos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateUserPhoto(int id, [FromForm] UserPhotosUpdateDto dto)
        {

            if (dto.PhotoFiles == null || dto.PhotoFiles.Count == 0)
                return BadRequest("Fotoğraf yüklenmedi.");

            if (dto.PhotoFiles.Count > 1)
                return BadRequest("En fazla 1 fotoğraf yükleyebilirsiniz.");

            var photo = _serviceManager.PhotoService.GetPhotosAsync("user", id, true);
            if (photo is null)
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