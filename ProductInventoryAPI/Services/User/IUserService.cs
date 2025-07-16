using ProductInventoryAPI.Dtos.User;
using ProductInventoryAPI.Models;

namespace ProductInventoryAPI.Services.User
{
    public interface IUserService
    {
        Task<UserModel> RegisterUserAsync(UserCreateDto dto);
        Task<UserModel> GetUserByIdAsync(Guid id);
        Task<bool> DeleteUserAsync(Guid id);
        Task<UserModel> LoginAsync(LoginDto dto);
    }
}
