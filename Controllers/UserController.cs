using CredipathAPI.DTOs;
using CredipathAPI.Helpers;
using CredipathAPI.Model;
using CredipathAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CredipathAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }


        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterUser([FromBody] UserDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Los datos de usuario son requeridos."));
            }

            var user = new User
            {
                name = dto.Name,
                username = dto.Username,
                email = dto.Email,
                password = dto.Password,
                note = dto.Note,
                address = dto.Address,
                code = dto.Code,
                UserType = Constants.UserType.admin,
            };

            var result = await _userService.RegisterUserAsync(user, dto.Password);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginData)
        {
            var userObj = await _userService.GetUserByUsernameAsync(loginData.usernameOrEmail);

            if (userObj == null || !_userService.VerifyPassword(userObj, loginData.password))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Credenciales incorrectas",
                    result = ""
                });
            }

            var token = _userService.GenerateJwtToken(userObj);

            return Ok(new
            {
                success = true,
                message = "Éxito",
                tokenValue = token
            });
        }

        [HttpPost]
        [Route("RegisterCollaborator")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<User>> RegisterCollaborator(UserDTO dto)
        {
            var user = await _userService.RegisterCollaboratorAsync(dto);
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            await _userService.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(user);
            return NoContent();
        }

        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
    }
}
