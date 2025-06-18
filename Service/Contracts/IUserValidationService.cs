using Entities.Models;

namespace Service.Contracts
{
    public interface IUserValidationService
    {
        Task<ApplicationUser> CheckUserExists(int userId);
    }
}
