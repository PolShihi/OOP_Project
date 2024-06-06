using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using Server_side.Repositories;

namespace Server_side.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        [Authorize(Roles = "Admin, Manager, Worker")]
        public async Task<IActionResult> SendEmail(EmailSendRequestDTO emailSendRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<string>.ErrorResponse("Invalid request.", 400, errors));
            }

            try
            {
                await _emailService.SendEmailAsync(emailSendRequest.ToEmail, emailSendRequest.Subject, emailSendRequest.Body);
                return StatusCode(StatusCodes.Status200OK, ApiResponse<string>.SuccessResponse("Success."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"{ex.Message}", 500));
            }
        }
    }
}
