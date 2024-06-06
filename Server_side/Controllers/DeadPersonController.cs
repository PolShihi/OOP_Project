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
    public class DeadPersonController : ControllerBase
    {
        private readonly IRepository<DeadPerson> _deadPersonRepository;
        private readonly IRepository<Order> _orderRepository;

        public DeadPersonController(IRepository<DeadPerson> deadPersonRepository, IRepository<Order> orderRepository)
        {
            _deadPersonRepository = deadPersonRepository;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Worker, Manager")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DeadPerson>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDeadPersons()
        {
            var deadPersons = await _deadPersonRepository.ListAllAsync();
            return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<DeadPerson>>.SuccessResponse(deadPersons));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<DeadPerson>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DeadPerson>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDeadPerson(int id)
        {
            var deadPerson = await _deadPersonRepository.GetByIdAsync(id);

            if (deadPerson is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<DeadPerson>.ErrorResponse("There is no dead person with such id.", 404));
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<DeadPerson>.SuccessResponse(deadPerson));
        }

        [HttpPost]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<DeadPerson>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DeadPerson>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddDeadPerson(DeadPersonDTO deadPersonDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<DeadPerson>.ErrorResponse("Invalid request.", 400, errors));
            }

            var order = await _orderRepository.GetByIdAsync(deadPersonDTO.OrderId);
            if (order is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<DeadPerson>.ErrorResponse("Invalid order id.", 400));
            }

            var deadPerson = new DeadPerson
            {
                FirstName = deadPersonDTO.FirstName,
                LastName = deadPersonDTO.LastName,
                DateOfBirth = deadPersonDTO.DateOfBirth,
                DateOfDeath = deadPersonDTO.DateOfDeath,
                OrderId = deadPersonDTO.OrderId,
                Details = deadPersonDTO.Details,
            };

            deadPerson = await _deadPersonRepository.AddAsync(deadPerson);
            var newDeadPerson = await _deadPersonRepository.GetByIdAsync(deadPerson.Id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<DeadPerson>.SuccessResponse(newDeadPerson));
        }

        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Worker")]
        //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> DeleteDeadPerson(int id)
        //{
        //    var deadPerson = await _deadPersonRepository.GetByIdAsync(id);

        //    if (deadPerson is null)
        //    {
        //        return StatusCode(StatusCodes.Status404NotFound, ApiResponse<string>.ErrorResponse("There is no dead person with such id.", 404));
        //    }

        //    await _deadPersonRepository.DeleteAsync(deadPerson);

        //    return StatusCode(StatusCodes.Status200OK, ApiResponse<string>.SuccessResponse("Success."));
        //}

        [HttpPut("{id}")]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<DeadPerson>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DeadPerson>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<DeadPerson>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDeadPerson(int id, DeadPersonDTO deadPersonDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<DeadPerson>.ErrorResponse("Invalid request.", 400, errors));
            }

            var order = await _orderRepository.GetByIdAsync(deadPersonDTO.OrderId);
            if (order is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<DeadPerson>.ErrorResponse("Invalid order id.", 400));
            }

            var deadPersonCheck = await _deadPersonRepository.GetByIdAsync(id);
            if (deadPersonCheck is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<DeadPerson>.ErrorResponse("There is no dead person with such id.", 404));
            }

            var deadPerson = new DeadPerson
            {
                Id = id,
                FirstName = deadPersonDTO.FirstName,
                LastName = deadPersonDTO.LastName,
                DateOfBirth = deadPersonDTO.DateOfBirth,
                DateOfDeath = deadPersonDTO.DateOfDeath,
                Details = deadPersonDTO.Details,
                OrderId = deadPersonDTO.OrderId,
            };

            await _deadPersonRepository.UpdateAsync(deadPerson);

            var updatedDeadPerson = await _deadPersonRepository.GetByIdAsync(id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<DeadPerson>.SuccessResponse(updatedDeadPerson));
        }
    }
}
