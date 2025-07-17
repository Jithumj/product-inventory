using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductInventoryAPI.Dtos.Product;
using ProductInventoryAPI.Services.Product;

namespace ProductInventoryAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductService _service;
        public ProductsController(IProductService service) => _service = service;
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ProductName) || dto.Variants == null || !dto.Variants.Any())
                return BadRequest("Product name and variants are required.");

            var id = await _service.CreateProductAsync(dto);
            var product = await _service.GetProductByIdAsync(id);
            return CreatedAtAction(nameof(GetProduct), new { id }, product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }


        [HttpGet]
        public async Task<IActionResult> ListProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var products = await _service.GetProductsAsync(page, pageSize);
            return Ok(products);
        }


        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock([FromBody] StockUpdateDto dto)
        {
            if (dto.Quantity <= 0) return BadRequest("Invalid quantity");
            var result = await _service.AddStockAsync(dto);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpPost("remove-stock")]
        public async Task<IActionResult> RemoveStock([FromBody] StockUpdateDto dto)
        {
            if (dto.Quantity <= 0) return BadRequest("Invalid quantity");
            var result = await _service.RemoveStockAsync(dto);
            if (!result) return BadRequest("Insufficient stock or invalid combination");
            return Ok();
        }
    }
}
