using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Dto;
using ProductService.Models;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController(ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _dbContext.Products.Include(p => p.Category).ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var product = await _dbContext.Products
                                   .Include(p => p.Category)
                                   .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound($"Product with ID {id} not found.");

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductDTO createProduct)
        {
            var category = await _dbContext.Categories.FindAsync(createProduct.CategoryId);
            if (category == null)
                return NotFound("Category not found.");

            var newProduct = new Product
            {
                Name = createProduct.Name,
                Price = createProduct.Price,
                Quantity = createProduct.Quantity,
                Category = category
            };

            await _dbContext.Products.AddAsync(newProduct);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDTO updatedProduct)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound($"Product with ID {id} not found.");

            var category = await _dbContext.Categories.FindAsync(updatedProduct.CategoryId);
            if (category == null)
                return NotFound("Category not found.");

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;
            product.Category = category;

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound($"Product with ID {id} not found.");

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return Ok($"Product '{product.Name}' deleted successfully.");
        }
    }
}
