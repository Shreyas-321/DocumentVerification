using webApitest.DTOs;
using webApitest.Models;

namespace webApitest.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterUserAsync(UserRegisterDto userRegisterDto);
        Task<User?> ValidateUserAsync(string email, string password);
        Task<User?> ValidateAdminAsync(string username, string password);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto?> UpdateUserAsync(int id, UserRegisterDto userUpdateDto);
        Task<bool> DeleteUserAsync(int id);
        UserResponseDto MapToUserResponseDto(User user);
    }
}
