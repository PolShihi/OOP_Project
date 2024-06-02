using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using Server_side.Repositories;
using System.Data;

namespace Server_side.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserRepository _userRepository, UserManager<AppUser> _userManager) : ControllerBase
    {
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<string>.ErrorResponse("Invalid request.", 400, errors));
            }

            var response = await _userRepository.CreateAccount(registerDTO);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<LoginResponseDTO>.ErrorResponse("Invalid request.", 400, errors));
            }

            var response = await _userRepository.LoginAccount(loginDTO);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserSession>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserRole = (await _userManager.GetRolesAsync(currentUser)).FirstOrDefault();

            if (currentUserRole == "Admin")
            {
                var users = await _userRepository.ListAllAsync();
                return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<UserSession>>.SuccessResponse(users));
            }

            var workers = await _userManager.GetUsersInRoleAsync("Worker");
            var userSessions = new List<UserSession>();
            foreach (var worker in workers)
            {
                userSessions.Add(new UserSession
                {
                    Id = worker.Id,
                    FirstName = worker.FirstName,
                    LastName = worker.LastName,
                    PhoneNumber = worker.PhoneNumber,
                    Email = worker.Email,
                    Role = "Worker"
                });
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<UserSession>>.SuccessResponse(userSessions));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var response = await _userRepository.DeleteAsync(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiResponse<UserSession>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserSession>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<UserSession>.ErrorResponse("There is no user with such id.", 404));
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<UserSession>.SuccessResponse(user));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(string id, RegisterDTO user)
        {
            if (!ModelState.IsValid && user.Password != "")
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<string>.ErrorResponse("Invalid request.", 400, errors));
            }

            var response = await _userRepository.UpdateAsync(id, user);

            return StatusCode(response.StatusCode, response);
        }
    }
}
