using Dapper;
using ProductInventoryAPI.Data;
using ProductInventoryAPI.Dtos.User;
using ProductInventoryAPI.Models;
using BCrypt.Net;

namespace ProductInventoryAPI.Services.User
{
    public class UserService:IUserService
    {
        private readonly DapperContext _context;
        public UserService(DapperContext context) => _context = context;

        # region Add user
        public async Task<UserModel> RegisterUserAsync(UserCreateDto dto)
        {
            using var conn = _context.CreateConnection();
            var user = new UserModel
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email.ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            var sql = @"INSERT INTO Users (Id, UserName, Email, Password)
                    VALUES (@Id, @UserName, @Email, @Password)";
            await conn.ExecuteAsync(sql, user);
            return user;
        }
        #endregion

        #region Get user by id
        public async Task<UserModel> GetUserByIdAsync(Guid id)
        {
            using var conn = _context.CreateConnection();
            var sql = @"SELECT * FROM Users WHERE Id = @Id";
            return await conn.QueryFirstOrDefaultAsync<UserModel>(sql, new { Id = id });
        }
        #endregion

        #region Delete user
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            using var conn = _context.CreateConnection();
            var sql = @"DELETE FROM Users WHERE Id = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id }) > 0;
        }
        #endregion

        #region user Login
        public async Task<UserModel> LoginAsync(LoginDto dto)
        {
            using var conn = _context.CreateConnection();
            var sql = @"SELECT * FROM Users WHERE Email = @Email";
            var user = await conn.QueryFirstOrDefaultAsync<UserModel>(sql, new { Email = dto.Email.ToLower() });
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return null;
            return user;
        }
        #endregion
    }
}
