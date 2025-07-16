using Microsoft.AspNetCore.Mvc;
using ProductInventoryAPI.Dtos.User;
using ProductInventoryAPI.Services.User;

namespace ProductInventoryAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) =>
            _userService = userService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDto dto)
        {
            // Validation can be improved with DataAnnotations
            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("All fields required!");

            var user = await _userService.RegisterUserAsync(dto);
            return Ok(new { user.Id, user.UserName, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userService.LoginAsync(dto);
            if (user == null)
                return Unauthorized("Invalid credentials");
            return Ok(new { user.Id, user.UserName, user.Email });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(new { user.Id, user.UserName, user.Email });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
