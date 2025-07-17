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

namespace ProductInventoryAPI.Services.User
{
    public class UserService:IUserService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly DapperContext _context;
        public UserService(DapperContext context, IOptions<JwtSettings> jwtOptions)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
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
        public async Task<LoginResponseModel> LoginAsync(LoginDto dto)
        {
            using var conn = _context.CreateConnection();
            var sql = @"SELECT * FROM Users WHERE Email = @Email";
            var user = await conn.QueryFirstOrDefaultAsync<UserModel>(sql, new { Email = dto.Email.ToLower() });
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return null;

            var token = GenerateJwtToken(user);
            return new LoginResponseModel
            {
                Token = token,
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        #endregion
    }
}
