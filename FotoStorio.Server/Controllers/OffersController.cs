using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FotoStorio.Server.Contracts;
using FotoStorio.Server.Extensions;
using FotoStorio.Server.Helpers;
using FotoStorio.Server.Specifications;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FotoStorio.Server.Controllers
{
    public class OffersController : BaseApiController
    {
        private readonly ILogger<OffersController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OffersController(ILogger<OffersController> logger, IProductRepository productRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _productRepository = productRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET api/offers
        /// <summary>
        /// Get products on special offer (SalePrice < Price)
        /// </summary>
        /// <returns>List of ProductDTO</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductSpecialOffers([FromQuery] ProductSpecificationParams productParams)
        {
            try
            {
                var spec = new ProductsOnSpecialOfferSpecification(productParams);
                var countSpec = new ProductsOnSpecialOfferCountSpecification(productParams); // gets a count after filtering
                var totalItems = await _productRepository.CountAsync(countSpec);

                // add pagination response headers
                _httpContextAccessor.HttpContext.AddPaginationResponseHeaders(totalItems, productParams.PageSize, productParams.PageIndex);

                var products = await _productRepository.ListWithSpecificationAsync(spec);

                return Ok(_mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(products));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in OffersController.GetProductSpecialOffers : {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }
    }
}