using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Shared;
using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface IAuthService
    {

        // e-posta kod gönderme servisi eklenecek

        Task<string?> LoginAsync(UserLoginDto model);
        Task<IdentityResult> UpdatePasswordAsync(UpdatePasswordDto model, string userId);

        // 1. Kullanıcının e-postasına doğrulama kodu gönderir
        Task<bool> SendForgotPasswordCodeAsync(string email);

        // 2. Kod ve yeni şifre ile şifre sıfırlama yapar
        Task<IdentityResult> ResetPasswordWithCodeAsync(string email, string code, string newPassword);

        Task<IdentityResult> RegisterOwnerAsync(UserRegisterDto model);
        Task<IdentityResult> RegisterCustomerAsync(CustomerRegisterDto model);



        Task<object?> GetUserAsync(int id);
        Task<IdentityResult> PatchCustomerAsync(int userId, JsonPatchDocument<CustomerPatchDto> patchDoc);
        Task<IdentityResult> UpdateCustomerAsync(int userId, CustomerUpdateDto dto);
        Task<IdentityResult> UpdateOwnerAsync(int userId, OwnerUpdateDto dto);
        Task<IdentityResult> PatchOwnerAsync(int userId, JsonPatchDocument<OwnerPatchDto> patchDoc);
        Task<IdentityResult> DeleteUserAsync(int userId);

    }
}