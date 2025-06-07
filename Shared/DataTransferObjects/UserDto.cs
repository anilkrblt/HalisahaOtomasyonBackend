using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects
{
    public enum FootPreference { Left, Right }
    public enum Role { Owner, Customer }


    public class UserRegisterDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }
        public string City { get; set; } = null!;
        public string Town { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public string Password { get; set; } = null!;
    }
    public class CustomerRegisterDto : UserRegisterDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FootPreference FootPreference { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string? PlayingPosition { get; set; }
        public bool Gender { get; set; }
    }



    public class UserLoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class UpdatePasswordDto
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }



    // Kullanıcı sadece e-mailini gönderecek
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; }
    }

    // Kullanıcı kod ve yeni şifre ile sıfırlama yapacak
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }

    public record UserPhotosUpdateDto(List<IFormFile> PhotoFiles);










    public class OwnerDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }
        public string City { get; set; } = null!;
        public string Town { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public string Password { get; set; } = null!;
    }


    public class CustomerDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }
        public string City { get; set; } = null!;
        public string Town { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public string Password { get; set; } = null!;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FootPreference FootPreference { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string? PlayingPosition { get; set; }
        public bool Gender { get; set; }
    }




    public class OwnerUpdateDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }
        public string City { get; set; } = null!;
        public string Town { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public string Password { get; set; } = null!;
    }


    public class CustomerUpdateDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }
        public string City { get; set; } = null!;
        public string Town { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public string Password { get; set; } = null!;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FootPreference FootPreference { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string? PlayingPosition { get; set; }
        public bool Gender { get; set; }
    }


    public class OwnerPatchDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }
        public string City { get; set; } = null!;
        public string Town { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public string Password { get; set; } = null!;
    }


    public class CustomerPatchDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }
        public string City { get; set; } = null!;
        public string Town { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public string Password { get; set; } = null!;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FootPreference FootPreference { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string? PlayingPosition { get; set; }
        public bool Gender { get; set; }
    }


}