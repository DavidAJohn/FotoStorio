using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FotoStorio.Server.Controllers;

public class OffersControllerTests : BaseApiController
{
    private readonly ILogger<OffersController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DefaultHttpContext _context;

    public OffersControllerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _logger = Substitute.For<ILogger<OffersController>>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _context = new DefaultHttpContext();
        _httpContextAccessor.HttpContext = _context;
    }

    [Fact]
    public async Task GetProductSpecialOffers_ShouldReturnListOfProductDTOs_WhenProductSpecialOffersExist()
    {
        //Arrange
        Fixture fixture = new();
        var productList = fixture.Create<IEnumerable<Product>>();

        _productRepository
            .ListAllAsync()
            .Returns(productList);

        var _productSpecificationParams = fixture.Create<ProductSpecificationParams>();

        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>());
        var mapper = mapperConfig.CreateMapper();

        //Act
        var sut = new OffersController(_logger, _productRepository, mapper, _httpContextAccessor);
        var result = (OkObjectResult)await sut.GetProductSpecialOffers(_productSpecificationParams);

        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<ProductDTO>>();
    }
}
