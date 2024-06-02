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
    public class CeremonyController(IRepository<Ceremony> _ceremonyRepository) : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "Manager, Worker")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Ceremony>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCeremonies()
        {
            var ceremonies = await _ceremonyRepository.ListAllAsync();
            return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<Ceremony>>.SuccessResponse(ceremonies));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Manager, Worker")]
        [ProducesResponseType(typeof(ApiResponse<Ceremony>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Ceremony>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCeremony(int id)
        {
            var ceremony = await _ceremonyRepository.GetByIdAsync(id);

            if (ceremony is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<Ceremony>.ErrorResponse("There is no ceremony with such id.", 404));
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Ceremony>.SuccessResponse(ceremony));
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<Ceremony>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Ceremony>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCeremony(CeremonyDTO ceremonyDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Ceremony>.ErrorResponse("Invalid request.", 400, errors));
            }

            var ceremony = new Ceremony
            {
                Name = ceremonyDTO.Name,
                Description = ceremonyDTO.Description,
                Price = ceremonyDTO.Price,
            };

            ceremony = await _ceremonyRepository.AddAsync(ceremony);
            var newCeremony = await _ceremonyRepository.GetByIdAsync(ceremony.Id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Ceremony>.SuccessResponse(newCeremony));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCeremony(int id)
        {
            var ceremony = await _ceremonyRepository.GetByIdAsync(id);

            if (ceremony is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<string>.ErrorResponse("There is no ceremony with such id.", 404));
            }

            await _ceremonyRepository.DeleteAsync(ceremony);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<string>.SuccessResponse("Success."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<Ceremony>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Ceremony>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<Ceremony>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCeremony(int id, CeremonyDTO ceremonyDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Ceremony>.ErrorResponse("Invalid request.", 400, errors));
            }

            var ceremonyCheck = await _ceremonyRepository.GetByIdAsync(id);

            if (ceremonyCheck is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<Ceremony>.ErrorResponse("There is no ceremony with such id.", 404));
            }

            var ceremony = new Ceremony
            {
                Id = id,
                Name = ceremonyDTO.Name,
                Description = ceremonyDTO.Description,
                Price = ceremonyDTO.Price,
            };

            await _ceremonyRepository.UpdateAsync(ceremony);

            var updatedCeremony = await _ceremonyRepository.GetByIdAsync(id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Ceremony>.SuccessResponse(updatedCeremony));
        }
    }
}
