using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using Server_side.Data;
using Server_side.Repositories;
using System.Threading;

namespace Server_side.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IRepository<Order> _orderRepository, IRepository<Client> _clientRepository, IRepository<Ceremony> _ceremonyRepository, AppDbContext _dbContext) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Worker, Manager")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Order>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderRepository.ListAllAsync();
            return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<Order>>.SuccessResponse(orders));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<Order>.ErrorResponse("There is no order with such id.", 404));
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Order>.SuccessResponse(order));
        }

        [HttpPost]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddOrder(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Order>.ErrorResponse("Invalid request.", 400, errors));
            }

            if (orderDTO.CeremonyId is not null)
            {
                var ceremony = await _ceremonyRepository.GetByIdAsync(orderDTO.CeremonyId.Value);
                if (ceremony is null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Order>.ErrorResponse("Invalid ceremony id.", 400));
                }
            }

            var client = await _clientRepository.GetByIdAsync(orderDTO.ClientId);
            if (client is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Order>.ErrorResponse("Invalid client id.", 400));
            }

            var order = new Order
            {
                Address = orderDTO.Adress,
                Date = orderDTO.Date,
                CeremonyId = orderDTO.CeremonyId,
                ClientId = orderDTO.ClientId,
            };

            order = await _orderRepository.AddAsync(order);
            var newOrder = await _orderRepository.GetByIdAsync(order.Id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Order>.SuccessResponse(newOrder));
        }

        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Worker")]
        //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> DeleteOrder(int id)
        //{
        //    var order = await _orderRepository.GetByIdAsync(id);

        //    if (order is null)
        //    {
        //        return StatusCode(StatusCodes.Status404NotFound, ApiResponse<string>.ErrorResponse("There is no order with such id.", 404));
        //    }

        //    await _orderRepository.DeleteAsync(order);

        //    return StatusCode(StatusCodes.Status200OK, ApiResponse<string>.SuccessResponse("Success."));
        //}

        [HttpPut("{id}")]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrder(int id, OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Order>.ErrorResponse("Invalid request.", 400, errors));
            }

            var orderCheck = await _orderRepository.GetByIdAsync(id);

            if (orderCheck is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<Order>.ErrorResponse("There is no order with such id.", 404));
            }

            if (orderDTO.CeremonyId is not null)
            {
                var ceremony = await _ceremonyRepository.GetByIdAsync(orderDTO.CeremonyId.Value);
                if (ceremony is null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Order>.ErrorResponse("Invalid ceremony id.", 400));
                }
            }

            var client = await _clientRepository.GetByIdAsync(orderDTO.ClientId);
            if (client is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Order>.ErrorResponse("Invalid client id.", 400));
            }

            var order = new Order
            {
                Id = id,
                Address = orderDTO.Adress,
                Date = orderDTO.Date,
                CeremonyId = orderDTO.CeremonyId,
                ClientId = orderDTO.ClientId,
            };

            await _orderRepository.UpdateAsync(order);

            var updatedOrder = await _orderRepository.GetByIdAsync(id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Order>.SuccessResponse(updatedOrder));
        }

        [HttpGet("{id}/Price")]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTotalCost(int id)
        {

            IQueryable<Order> query = _dbContext.Set<Order>()
                .Include(o => o.Ceremony)
                .Include(o => o.ProductOrders).ThenInclude(p => p.Product);

            var entity = await query.FirstOrDefaultAsync(e => e.Id == id);

            if (entity is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<decimal>.ErrorResponse("There is no order with such id.", 404));
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<decimal>.SuccessResponse(entity.GetTotalPrice()));
        }
    }
}
