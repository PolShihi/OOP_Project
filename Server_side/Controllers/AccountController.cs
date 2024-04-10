using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server_side.Models.DTOs;
using Server_side.Repositories;

namespace Server_side.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserRepository _userRepository) : ControllerBase
    {
        [HttpPost("register")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            var response = await _userRepository.CreateAccount(userDTO);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await _userRepository.LoginAccount(loginDTO);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _userRepository.GetUsers();
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var response = await _userRepository.DeleteUser(id);
            return Ok(response);
        }
    }
}
