using System.Collections.Generic;
using System.Threading.Tasks;
using FotoStorio.Server.Data;
using FotoStorio.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FotoStorio.Server.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly ApplicationDbContext _context;
        public CategoriesController(ILogger<CategoriesController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Category>> GetCategories()
        {
            var categories = await _context.Categories
                .ToListAsync();

            return categories;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _context.Categories
                .SingleOrDefaultAsync(c => c.Id == id);

            return Ok(category);
        }
    }
}