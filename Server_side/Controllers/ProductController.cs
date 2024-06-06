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
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> _productRepository;

        public ProductController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Worker")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Product>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepository.ListAllAsync();
            return StatusCode(StatusCodes.Status200OK, ApiResponse<IReadOnlyList<Product>>.SuccessResponse(products));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Manager, Worker")]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<Product>.ErrorResponse("There is no product with such id.", 404));
            }

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Product>.SuccessResponse(product));
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProduct(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Product>.ErrorResponse("Invalid request.", 400, errors));
            }

            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Amount = productDTO.Amount,
                ReorderLevel = productDTO.ReorderLevel,
            };

            product = await _productRepository.AddAsync(product);
            var newProduct = await _productRepository.GetByIdAsync(product.Id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Product>.SuccessResponse(newProduct));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<string>.ErrorResponse("There is no product with such id.", 404));
            }

            await _productRepository.DeleteAsync(product);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<string>.SuccessResponse("Success."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(int id, ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponse<Product>.ErrorResponse("Invalid request.", 400, errors));
            }

            var productCheck = await _productRepository.GetByIdAsync(id);
            if (productCheck is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ApiResponse<Product>.ErrorResponse("There is no product with such id.", 404));
            }

            var product = new Product
            {
                Id = id,
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Amount = productDTO.Amount,
                ReorderLevel = productDTO.ReorderLevel,
            };

            await _productRepository.UpdateAsync(product);

            var updatedProduct = await _productRepository.GetByIdAsync(id);

            return StatusCode(StatusCodes.Status200OK, ApiResponse<Product>.SuccessResponse(updatedProduct));
        }
    }
}
