using Dapper;
using ProductInventoryAPI.Data;
using ProductInventoryAPI.Dtos.User;
using ProductInventoryAPI.Models;
using BCrypt.Net;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProductInventoryAPI.Services.Logging;

namespace ProductInventoryAPI.Services.User
{
    public class UserService:IUserService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly DapperContext _context;
        private readonly ILoggerService _logger;
        public UserService(DapperContext context, IOptions<JwtSettings> jwtOptions,ILoggerService loggerService)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
            _logger = loggerService;
        }
        #region Generate token
        private string GenerateJwtToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
               new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
               new Claim(JwtRegisteredClaimNames.Email, user.Email),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion

        #region Add user
        public async Task<UserModel> RegisterUserAsync(UserCreateDto dto)
        {
            try
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
                // Log user registration
                var logmessage = $"User registered: {user.UserName} with Email: {user.Email} at {DateTime.UtcNow}";
                _logger.LogAsync(logmessage);
                return user;
            }
            catch (Exception ex)
            {
                var logmessage = $"Error in UserService.RegisterUserAsync: {ex.Message}";
                _logger.LogAsync(logmessage);
                return null;
            }
        }
        #endregion

        #region Get user by id
        public async Task<UserModel> GetUserByIdAsync(Guid id)
        {
            try
            {
                using var conn = _context.CreateConnection();
                var sql = @"SELECT * FROM Users WHERE Id = @Id";
                return await conn.QueryFirstOrDefaultAsync<UserModel>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                var logmessage = $"Error in UserService.GetUserByIdAsync: {ex.Message}";
                _logger.LogAsync(logmessage);
                return null;
            }
        }
        #endregion

        #region get all the user data
        public async Task<IEnumerable<UserlistModel>> GetAllUsersAsync()
        {
            try
            {
                using var conn = _context.CreateConnection();
                var sql = @"SELECT Id,UserName,Email FROM Users";
                var users = await conn.QueryAsync<UserlistModel>(sql);
                return users.ToList();
            }
            catch (Exception ex)
            {
                var logmessage = $"Error in UserService.GetAllUsersAsync: {ex.Message}";
                _logger.LogAsync(logmessage);
                return Enumerable.Empty<UserlistModel>();
            }
        }

        #endregion

        #region Delete user
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                using var conn = _context.CreateConnection();
                var sql = @"DELETE FROM Users WHERE Id = @Id";
                var logmessage = $"User deleted with id: {id} at {DateTime.UtcNow}";
                _logger.LogAsync(logmessage);
                return await conn.ExecuteAsync(sql, new { Id = id }) > 0;

            }
            catch (Exception ex)
            {
                var logmessage = $"Error in UserService.DeleteUserAsync: {ex.Message}";
                _logger.LogAsync(logmessage);
                return false;
            }
        }
        #endregion

        #region user Login
        public async Task<LoginResponseModel> LoginAsync(LoginDto dto)
        {
            try
            {
                using var conn = _context.CreateConnection();
                var sql = @"SELECT * FROM Users WHERE Email = @Email";
                var user = await conn.QueryFirstOrDefaultAsync<UserModel>(sql, new { Email = dto.Email.ToLower() });
                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                    return null;

                var token = GenerateJwtToken(user);
                var logmessage = $"User logged in with id: {user.Id} at {DateTime.UtcNow}";
                _logger.LogAsync(logmessage);
                return new LoginResponseModel
                {
                    Token = token,
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                };
            }
            catch (Exception ex)
            {
                var logmessage = $"Error in UserService.LoginAsync: {ex.Message}";
                _logger.LogAsync(logmessage);
            }
            return null;
        }

        #endregion
    }
}
