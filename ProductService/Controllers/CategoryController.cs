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
    public class CategoryController(ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var findCategory = await _dbContext.Categories.FindAsync(id);
            return Ok(findCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CategoryDto categoryDto)
        {
            if (string.IsNullOrWhiteSpace(categoryDto.Name))
                return BadRequest("Category name cannot be empty.");

            var category = new Category { Name = categoryDto.Name };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto categoryDto)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return NotFound($"Category with ID {id} not found.");

            if (string.IsNullOrWhiteSpace(categoryDto.Name))
                return BadRequest("Category name cannot be empty.");

            category.Name = categoryDto.Name;

            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();

            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return NotFound($"Category with ID {id} not found.");

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return Ok($"Category '{category.Name}' deleted successfully.");
        }
    }
}
