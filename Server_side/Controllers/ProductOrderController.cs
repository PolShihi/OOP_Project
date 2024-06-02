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
    public class ProductOrderController(IRepository<ProductOrder> _productOrderRepository, IRepository<Order> _orderRepository, IRepository<Product> _productRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Worker, Manager")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProductOrder>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductOrders()
        {
            var productOrders = await _productOrderRepository.ListAllAsync();
            return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<ProductOrder>>.SuccessResponse(productOrders));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<ProductOrder>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProductOrder>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductOrder(int id)
        {
            var productOrder = await _productOrderRepository.GetByIdAsync(id);

            if (productOrder is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<ProductOrder>.ErrorResponse("There is no product order with such id.", 404));
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<ProductOrder>.SuccessResponse(productOrder));
        }

        [HttpPost]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<ProductOrder>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProductOrder>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProductOrder(ProductOrderDTO productOrderDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<ProductOrder>.ErrorResponse("Invalid request.", 400, errors));
            }

            var product = await _productRepository.GetByIdAsync(productOrderDTO.ProductId);
            if (product is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<ProductOrder>.ErrorResponse("Invalid product id.", 400));
            }

            var order = await _orderRepository.GetByIdAsync(productOrderDTO.OrderId);
            if (order is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<ProductOrder>.ErrorResponse("Invalid order id.", 400));
            }

            if (product.Amount - productOrderDTO.Amount < 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<ProductOrder>.ErrorResponse($"There is no required quantity of product ({product.Name}) in stock ({product.Amount} left).", 400));
            }

            var productOrder = new ProductOrder
            {
                ProductId = productOrderDTO.ProductId,
                OrderId = productOrderDTO.OrderId,
                Amount = productOrderDTO.Amount,
                Comment = productOrderDTO.Comment,
            };

            productOrder = await _productOrderRepository.AddAsync(productOrder);
            var newProductOrder = await _productOrderRepository.GetByIdAsync(productOrder.Id);

            product.Amount -= newProductOrder.Amount;
            await _productRepository.UpdateAsync(product);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<ProductOrder>.SuccessResponse(newProductOrder));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProductOrder(int id)
        {
            var productOrder = await _productOrderRepository.GetByIdAsync(id);

            if (productOrder is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<string>.ErrorResponse("There is no product order with such id.", 404));
            }

            await _productOrderRepository.DeleteAsync(productOrder);

            var product = await _productRepository.GetByIdAsync(productOrder.ProductId);
            if (product is not null)
            {
                product.Amount += productOrder.Amount;
                await _productRepository.UpdateAsync(product);
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<string>.SuccessResponse("Success."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Worker")]
        [ProducesResponseType(typeof(ApiResponse<ProductOrder>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProductOrder>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<ProductOrder>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductOrder(int id, ProductOrderDTO productOrderDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<ProductOrder>.ErrorResponse("Invalid request.", 400, errors));
            }
            var product = await _productRepository.GetByIdAsync(productOrderDTO.ProductId);
            if (product is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<ProductOrder>.ErrorResponse("Invalid product id.", 400));
            }

            var order = await _orderRepository.GetByIdAsync(productOrderDTO.OrderId);
            if (order is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<ProductOrder>.ErrorResponse("Invalid order id.", 400));
            }

            var productOrderCheck = await _productOrderRepository.GetByIdAsync(id);
            if (productOrderCheck is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<ProductOrder>.ErrorResponse("There is no product order with such id.", 404));
            }

            if (productOrderCheck.ProductId == productOrderDTO.ProductId)
            {
                if (product.Amount + productOrderCheck.Amount - productOrderDTO.Amount < 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<ProductOrder>.ErrorResponse("There is no required quantity of product in stock.", 400));
                }

                product.Amount += productOrderCheck.Amount - productOrderDTO.Amount;
                await _productRepository.UpdateAsync(product);
            }
            else
            {
                if (product.Amount - productOrderDTO.Amount < 0) 
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<ProductOrder>.ErrorResponse("There is no required quantity of product in stock.", 400));
                }

                var pastProduct = await _productRepository.GetByIdAsync(productOrderCheck.ProductId);
                if (pastProduct is null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<ProductOrder>.ErrorResponse("Server Error.", 500));
                }

                product.Amount -= productOrderDTO.Amount;
                await _productRepository.UpdateAsync(product);

                pastProduct.Amount += productOrderCheck.Amount;
                await _productRepository.UpdateAsync(pastProduct);
            }

            var productOrder = new ProductOrder
            {
                Id = id,
                ProductId = productOrderDTO.ProductId,
                OrderId = productOrderDTO.OrderId,
                Amount = productOrderDTO.Amount,
                Comment = productOrderDTO.Comment,
            };

            await _productOrderRepository.UpdateAsync(productOrder);

            var updatedProductOrder = await _productOrderRepository.GetByIdAsync(id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<ProductOrder>.SuccessResponse(updatedProductOrder));
        }
    }
}
