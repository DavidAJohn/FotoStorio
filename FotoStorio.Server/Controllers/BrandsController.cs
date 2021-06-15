using System.Collections.Generic;
using System.Threading.Tasks;
using FotoStorio.Server.Data;
using FotoStorio.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FotoStorio.Server.Controllers
{
    public class BrandsController : BaseApiController
    {
        private readonly ILogger<BrandsController> _logger;
        private readonly ApplicationDbContext _context;
        public BrandsController(ILogger<BrandsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Brand>> GetBrands()
        {
            var brands = await _context.Brands
                .ToListAsync();

            return brands;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _context.Brands
                .SingleOrDefaultAsync(a => a.Id == id);

            return Ok(brand);
        }
    }
}