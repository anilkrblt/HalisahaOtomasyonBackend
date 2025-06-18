using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace Service
{
    public class UserValidationService : IUserValidationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserValidationService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> CheckUserExists(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new UserNotFoundException(userId);
            return user;
        }
    }
}
