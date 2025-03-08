using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Dto;
using ProductService.Messaging.Events;
using ProductService.Models;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController(ProductDbContext _dbContext, IBus _bus) : ControllerBase
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
        public async Task UpdateProductAsync(int id, ProductDTO updatedProduct)
        {
            var existingProduct = await _dbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct == null)
                throw new Exception("Product not found");

            if (existingProduct.Category.Id != updatedProduct.CategoryId)
            {
                var newCategory = await _dbContext.Categories.FindAsync(updatedProduct.CategoryId);
                if (newCategory == null)
                    throw new Exception("New category not found");

                existingProduct.Category = newCategory;
            }

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Quantity = updatedProduct.Quantity;

            await _dbContext.SaveChangesAsync();

            // Publish ProductUpdatedEvent to RabbitMQ
            var productUpdatedEvent = new ProductUpdatedEvent
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Price = existingProduct.Price,
                Quantity = existingProduct.Quantity,
                CategoryId = existingProduct.Category.Id
            };

            await _bus.PubSub.PublishAsync(productUpdatedEvent);
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
