using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using Server_side.Repositories;
using System.Net;

namespace Server_side.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IRepository<Client> _clientRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepository;

        public ClientController(IRepository<Client> clientRepository, UserManager<AppUser> userManager, IUserRepository userRepository)
        {
            _clientRepository = clientRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Worker")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Client>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClients()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserRole = (await _userManager.GetRolesAsync(currentUser)).FirstOrDefault();

            if (currentUserRole == "Manager")
            {
                var clients = await _clientRepository.ListAllAsync();
                return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<Client>>.SuccessResponse(clients));
            }

            var clientsOfWorker = await _clientRepository.ListAsync(c => c.AppUserId == currentUser.Id);

            var clientsWorker = new List<Client>();
            foreach (var worker in clientsOfWorker)
            {
                clientsWorker.Add(new Client
                {
                    Id = worker.Id,
                    Address = worker.Address,
                    AppUserId = worker.AppUserId,
                    Email = worker.Email,
                    FirstName = worker.FirstName,
                    LastName = worker.LastName,
                    IsProcessed = worker.IsProcessed,
                    PhoneNumber = worker.PhoneNumber,
                });
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<Client>>.SuccessResponse(clientsWorker));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Manager, Worker")]
        [ProducesResponseType(typeof(ApiResponse<Client>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Client>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClient(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<Client>.ErrorResponse("There is no client with such id.", 404));
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Client>.SuccessResponse(client));
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<Client>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Client>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddClient(ClientDTO clientDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Client>.ErrorResponse("Invalid request.", 400, errors));
            }

            if (clientDTO.AppUserId is not null) 
            {
                var user = await _userRepository.GetByIdAsync(clientDTO.AppUserId);
                if (user is null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Client>.ErrorResponse("Invalid user id.", 400));
                }
            }

            var client = new Client
            {
                FirstName = clientDTO.FirstName,
                LastName = clientDTO.LastName,
                PhoneNumber = clientDTO.PhoneNumber,
                Email = clientDTO.Email,
                Address = clientDTO.Address,
                IsProcessed = clientDTO.IsProcessed,
                AppUserId = clientDTO.AppUserId,
            };

            client = await _clientRepository.AddAsync(client);
            var newClient = await _clientRepository.GetByIdAsync(client.Id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Client>.SuccessResponse(newClient));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            
            if (client is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<string>.ErrorResponse("There is no client with such id.", 404));
            }

            await _clientRepository.DeleteAsync(client);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<string>.SuccessResponse("Success."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager, Worker")]
        [ProducesResponseType(typeof(ApiResponse<Client>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Client>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<Client>), StatusCodes.Status400BadRequest)]
        public async Task <IActionResult> UpdateClient(int id, ClientDTO clientDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Client>.ErrorResponse("Invalid request.", 400, errors));
            }

            if (clientDTO.AppUserId is not null)
            {
                var user = await _userRepository.GetByIdAsync(clientDTO.AppUserId);
                if (user is null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Client>.ErrorResponse("Invalid user id.", 400));
                }
            }

            var clientCheck = await _clientRepository.GetByIdAsync(id);

            if (clientCheck is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<Client>.ErrorResponse("There is no client with such id.", 404));
            }

            var client = new Client
            {
                Id = id,
                FirstName = clientDTO.FirstName,
                LastName = clientDTO.LastName,
                Address = clientDTO.Address,
                Email = clientDTO.Email,
                PhoneNumber = clientDTO.PhoneNumber,
                IsProcessed = clientDTO.IsProcessed,
                AppUserId = clientDTO.AppUserId,
            };

            await _clientRepository.UpdateAsync(client);

            var updatedClient = await _clientRepository.GetByIdAsync(id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Client>.SuccessResponse(updatedClient));
        }
    }
}
