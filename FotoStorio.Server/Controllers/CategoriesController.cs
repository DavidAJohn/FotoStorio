using FotoStorio.Server.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FotoStorio.Server.Controllers;

public class CategoriesController : BaseApiController
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ILogger<CategoriesController> logger, ICategoryRepository categoryRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
    }

    // GET api/categories
    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryRepository.ListAllAsync();

        return Ok(categories);
    }

    // GET api/categories/{id}
    [HttpGet("{id}")]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] {"id"})]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            _logger.LogError("Category with id: {id}, not found", id);

            return NotFound();
        }
        else
        {
            return Ok(category);
        }
    }
}