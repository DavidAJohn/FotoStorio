using FotoStorio.Server.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FotoStorio.Server.Controllers;

public class BrandsController(ILogger<BrandsController> logger, IBrandRepository brandRepository) : BaseApiController
{
    // GET api/brands
    [HttpGet]
    public async Task<IActionResult> GetBrands()
    {
        var brands = await brandRepository.ListAllAsync();

        return Ok(brands);
    }

    // GET api/brands/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBrandById(int id)
    {
        var brand = await brandRepository.GetByIdAsync(id);

        if (brand == null)
        {
            logger.LogError("Brand with id: {id}, not found", id);

            return NotFound();
        }
        else
        {
            return Ok(brand);
        }
    }
}