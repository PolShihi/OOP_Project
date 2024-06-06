using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using Server_side.Repositories;

namespace Server_side.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IRepository<Report> _reportRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepository;

        public ReportController(IRepository<Report> reportRepository, UserManager<AppUser> userManager, IUserRepository userRepository)
        {
            _reportRepository = reportRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Worker")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Report>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReports()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserRole = (await _userManager.GetRolesAsync(currentUser)).FirstOrDefault();

            if (currentUserRole == "Manager")
            {
                var reports = await _reportRepository.ListAllAsync();
                return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<Report>>.SuccessResponse(reports));
            }

            var reportsOfWorker = await _reportRepository.ListAsync(r => r.AppUserId == currentUser.Id);

            var reportsWorker = new List<Report>();
            foreach (var report in reportsOfWorker)
            {
                reportsWorker.Add(new Report
                {
                    Id = report.Id,
                    Answer = report.Answer,
                    AppUserId = report.AppUserId,
                    Email = report.Email,
                    FirstName = report.FirstName,
                    IsAnswered = report.IsAnswered,
                    LastName = report.LastName,
                    PhoneNumber = report.PhoneNumber,
                    Text = report.Text,
                });
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<Report>>.SuccessResponse(reportsWorker));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Manager, Worker")]
        [ProducesResponseType(typeof(ApiResponse<Report>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Report>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReport(int id)
        {
            var report = await _reportRepository.GetByIdAsync(id);

            if (report is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<Report>.ErrorResponse("There is no report with such id.", 404));
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Report>.SuccessResponse(report));
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<Report>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Report>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddReport(ReportDTO reportDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Report>.ErrorResponse("Invalid request.", 400, errors));
            }

            if (reportDTO.AppUserId is not null)
            {
                var user = await _userRepository.GetByIdAsync(reportDTO.AppUserId);
                if (user is null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Report>.ErrorResponse("Invalid user id.", 400));
                }
            }

            var report = new Report
            {
                FirstName = reportDTO.FirstName,
                LastName = reportDTO.LastName,
                Email = reportDTO.Email,
                PhoneNumber = reportDTO.PhoneNumber,
                Text = reportDTO.Text,
                IsAnswered = reportDTO.IsAnswered,
                Answer = reportDTO.Answer,
                AppUserId = reportDTO.AppUserId,
            };

            report = await _reportRepository.AddAsync(report);
            var newReport = await _reportRepository.GetByIdAsync(report.Id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Report>.SuccessResponse(newReport));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var report = await _reportRepository.GetByIdAsync(id);

            if (report is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<string>.ErrorResponse("There is no report with such id.", 404));
            }

            await _reportRepository.DeleteAsync(report);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<string>.SuccessResponse("Success."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager, Worker")]
        [ProducesResponseType(typeof(ApiResponse<Report>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Report>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<Report>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateReport(int id, ReportDTO reportDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Report>.ErrorResponse("Invalid request.", 400, errors));
            }

            if (reportDTO.AppUserId is not null)
            {
                var user = await _userRepository.GetByIdAsync(reportDTO.AppUserId);
                if (user is null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Report>.ErrorResponse("Invalid user id.", 400));
                }
            }

            var reportCheck = await _reportRepository.GetByIdAsync(id);
            if (reportCheck is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<Report>.ErrorResponse("There is no report with such id.", 404));
            }

            var report = new Report
            {
                Id = id,
                FirstName = reportDTO.FirstName,
                LastName = reportDTO.LastName,
                Email = reportDTO.Email,
                PhoneNumber = reportDTO.PhoneNumber,
                Text = reportDTO.Text,
                IsAnswered = reportDTO.IsAnswered,
                Answer = reportDTO.Answer,
                AppUserId = reportDTO.AppUserId,
            };

            await _reportRepository.UpdateAsync(report);

            var updatedReport = await _reportRepository.GetByIdAsync(id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Report>.SuccessResponse(updatedReport));
        }
    }
}
