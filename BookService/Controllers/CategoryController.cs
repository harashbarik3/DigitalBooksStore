using BookService.Repositories;
using CommonLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserServiceLib;

namespace BookService.Controllers
{
    [Route("api/v1/digitalbooks/category")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllPublisher()
        {
            var books = await _categoryRepository.GetAllCategoryAsync();

            return Ok(books);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            var categoryRes = new Category()
            {
                CategoryName = category.CategoryName
            };
            var categoryResResult = await _categoryRepository.AddCategoryAsync(categoryRes);

            return Ok(categoryResResult);
        }
    }
}
