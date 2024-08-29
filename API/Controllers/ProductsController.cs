using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository productRepository) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
            string? brand, string? type, string? sortBy, string? sortDirection)
        {
            return Ok(await productRepository.GetProductsAsync(brand, type, sortBy, sortDirection));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await productRepository.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            productRepository.AddProduct(product);
            if (await productRepository.SaveChangesAsync())
            {
                return CreatedAtAction("CreateProduct", new { id = product.Id }, product);
            }
            return BadRequest("Bad Request");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (product.Id != id || !ProductExists(id))
                return BadRequest("Bad Request");

            productRepository.UpdateProduct(product);
            if (await productRepository.SaveChangesAsync())
            {
                return NoContent();
            }
            return BadRequest("Bad Request");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await productRepository.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            productRepository.DeleteProduct(product);
            if (await productRepository.SaveChangesAsync())
            {
                return NoContent();
            }
            return BadRequest("Bad Request");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetProductBrands()
        {
            return Ok(await productRepository.GetBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetProductTypes()
        {
            return Ok(await productRepository.GetTypesAsync());
        }

        private bool ProductExists(int id)
        {
            return productRepository.ProductExists(id);
        }
    }
}
