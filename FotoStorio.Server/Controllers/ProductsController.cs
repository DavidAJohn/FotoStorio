using AutoMapper;
using FotoStorio.Server.Contracts;
using FotoStorio.Server.Extensions;
using FotoStorio.Server.Helpers;
using FotoStorio.Server.Specifications;
using FotoStorio.Shared.Auth;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FotoStorio.Server.Controllers;

public class ProductsController : BaseApiController
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductsController(ILogger<ProductsController> logger, IProductRepository productRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _productRepository = productRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    // GET api/products
    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>List of ProductDTO</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] ProductSpecificationParams productParams)
    {
        var spec = new ProductsWithBrandsAndCategoriesSpecification(productParams);
        var countSpec = new ProductsWithFiltersForCountSpecification(productParams); // gets a count after filtering
        var totalItems = await _productRepository.CountAsync(countSpec);

        // add pagination response headers
        _httpContextAccessor.HttpContext.AddPaginationResponseHeaders(totalItems, productParams.PageSize, productParams.PageIndex);

        var products = await _productRepository.ListWithSpecificationAsync(spec);

        return Ok(_mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(products));
    }

    // GET api/products/{id}
    /// <summary>
    /// Get a product by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>ProductDTO</returns>
    [HttpGet("{id}", Name="GetProductById")]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] {"id"})]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(int id)
    {
        var spec = new ProductsWithBrandsAndCategoriesSpecification(id);
        var product = await _productRepository.GetEntityWithSpecification(spec);

        if (product == null)
        {
            _logger.LogError("Product with id: {id} not found", id);

            return NotFound();
        }
        else
        {
            return Ok(_mapper.Map<Product, ProductDTO>(product));
        }
    }

    // POST api/products
    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <returns>ProductDTO</returns>
    [Authorize(Policy = Policies.IsAdmin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO productCreateDTO)
    {
        if (productCreateDTO == null)
        {
            return BadRequest();
        }

        var product = _mapper.Map<Product>(productCreateDTO);
        var createdProduct = await _productRepository.Create(product);

        if (createdProduct == null)
        {
            return BadRequest();
        }

        var productDTO = _mapper.Map<Product, ProductDTO>(product);

        return CreatedAtAction(nameof(GetProductById), new { Id = productDTO.Id }, productDTO);
    }

    // PUT api/products/{id}
    /// <summary>
    /// Update a product
    /// </summary>
    /// <param name="id"></param>
    [Authorize(Policy = Policies.IsAdmin)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDTO productUpdateDTO)
    {
        if (productUpdateDTO == null)
        {
            _logger.LogWarning("Product Updated Failed: Attempt to update a product when supplied ProductUpdateDTO was null");
            return BadRequest();
        }

        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        _mapper.Map(productUpdateDTO, product);

        var updated = await _productRepository.Update(product);

        if (!updated)
        {
            _logger.LogWarning("Product Update Failed: Supplied product could not be updated. Id: {id}", id);
            return BadRequest();
        }

        return NoContent();
    }

    // DELETE api/products/{id}
    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id"></param>
    [Authorize(Policy = Policies.IsAdmin)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var deleted = await _productRepository.Delete(product);

        if (!deleted)
        {
            _logger.LogWarning("Product Deletion Failed: Supplied product could not be deleted. Id: {id}", id);
            return BadRequest();
        }

        return NoContent();
    }
}