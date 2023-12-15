using FotoStorio.Server.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FotoStorio.Server.Controllers;

public class CategoriesController(ILogger<CategoriesController> logger, ICategoryRepository categoryRepository) : BaseApiController
{
    // GET api/categories
    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await categoryRepository.ListAllAsync();

        return Ok(categories);
    }

    // GET api/categories/{id}
    [HttpGet("{id}")]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] {"id"})]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            logger.LogError("Category with id: {id}, not found", id);

            return NotFound();
        }
        else
        {
            return Ok(category);
        }
    }
}