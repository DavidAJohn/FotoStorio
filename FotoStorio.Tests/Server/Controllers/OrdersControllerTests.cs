using AutoMapper;
using FotoStorio.Shared.Models.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FotoStorio.Server.Controllers;

public class OrdersControllerTests
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private OrdersController _sut;

    public OrdersControllerTests()
    {
        _orderService = Substitute.For<IOrderService>();
        _logger = Substitute.For<ILogger<OrdersController>>();
        _mapper = BuildMapper();

        var mockClaimsPrincipal = Substitute.For<ClaimsPrincipal>();
        mockClaimsPrincipal.Claims.Returns(new List<Claim>
        {
            new(ClaimTypes.Email, "test@test.com")
        });
        var context = Substitute.For<HttpContext>();
        context.User.Returns(mockClaimsPrincipal);

        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _httpContextAccessor.HttpContext.Returns(context);

        _sut = new OrdersController(_orderService, _logger, _mapper, _httpContextAccessor);
    }

    private static IMapper BuildMapper()
    {
        var config = new MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapperProfiles());
        });

        return config.CreateMapper();
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnBadRequest_WhenOrderIsNull()
    {
        // Arrange
        var orderCreateDTO = null as OrderCreateDTO;

        // Act
        var result = (BadRequestResult)await _sut.CreateOrder(orderCreateDTO);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateOrder_ShouldReturnBadRequest_WhenPartOfAddressIsIncomplete(string postCode)
    {
        // Arrange
        Fixture fixture = new();
        var orderCreateDTO = fixture.Create<OrderCreateDTO>();
        orderCreateDTO.SendToAddress.PostCode = postCode;

        // Act
        var result = (BadRequestResult)await _sut.CreateOrder(orderCreateDTO);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnOk_WhenOrderIsValid()
    {
        // Arrange
        Fixture fixture = new();
        var orderCreateDTO = fixture.Create<OrderCreateDTO>();

        var createdOrder = fixture.Create<Order>();

        _orderService.CreateOrderAsync(Arg.Any<string>(), Arg.Any<OrderCreateDTO>())
            .Returns(createdOrder);

        // Act
        var result = (OkObjectResult)await _sut.CreateOrder(orderCreateDTO);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<Order>();
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnBadRequest_WhenClaimsPrincipalEmailIsEmpty()
    {
        // Arrange
        Fixture fixture = new();
        var orderCreateDTO = fixture.Create<OrderCreateDTO>();
        var createdOrder = fixture.Create<Order>();

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var defaultContext = new DefaultHttpContext(); // create a default context with no claims
        httpContextAccessor.HttpContext = defaultContext;

        _orderService.CreateOrderAsync(Arg.Any<string>(), Arg.Any<OrderCreateDTO>())
            .Returns(createdOrder);

        // Act
        var sut = new OrdersController(_orderService, _logger, _mapper, httpContextAccessor);
        var result = (BadRequestResult)await sut.CreateOrder(orderCreateDTO);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetOrderByIdForUser_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = 1;

        _orderService.GetOrderByIdAsync(Arg.Any<int>(), Arg.Any<string>())
            .Returns((Order)null);

        // Act
        var result = (NotFoundResult)await _sut.GetOrderByIdForUser(orderId);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetOrderByIdForUser_ShouldReturnOrderDetailsDTO_WhenOrderExists()
    {
        // Arrange
        var orderId = 1;

        Fixture fixture = new();
        var order = fixture.Create<Order>();

        _orderService.GetOrderByIdAsync(Arg.Any<int>(), Arg.Any<string>())
            .Returns(order);

        // Act
        var result = (OkObjectResult)await _sut.GetOrderByIdForUser(orderId);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<OrderDetailsDTO>();
        result.Value.Should().BeEquivalentTo(_mapper.Map<OrderDetailsDTO>(order));
    }

    [Fact]
    public async Task GetOrderByIdForUser_ShouldReturnNotFound_WhenClaimsPrincipalEmailIsEmpty()
    {
        // Arrange
        Fixture fixture = new();
        var createdOrder = fixture.Create<Order>();

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var defaultContext = new DefaultHttpContext(); // create a default context with no claims
        httpContextAccessor.HttpContext = defaultContext;

        _orderService.GetOrderByIdAsync(Arg.Any<int>(), Arg.Any<string>())
            .Returns(createdOrder);

        // Act
        var sut = new OrdersController(_orderService, _logger, _mapper, httpContextAccessor);
        var result = (NotFoundResult)await sut.GetOrderByIdForUser(1);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetOrdersForUser_ShouldReturnOk_WhenOrdersExist()
    {
        // Arrange
        Fixture fixture = new();
        var orders = fixture.Create<List<Order>>();

        _orderService.GetOrdersForUserAsync(Arg.Any<string>())
            .Returns(orders);

        // Act
        var result = (OkObjectResult)await _sut.GetOrdersForUser();

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<OrderDetailsDTO>>();
        result.Value.Should().BeEquivalentTo(_mapper.Map<List<OrderDetailsDTO>>(orders));
    }

    [Fact]
    public async Task GetOrdersForUser_ShouldReturnNotFound_WhenClaimsPrincipalEmailIsEmpty()
    {
        // Arrange
        Fixture fixture = new();
        var orders = fixture.Create<List<Order>>();

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var defaultContext = new DefaultHttpContext(); // create a default context with no claims
        httpContextAccessor.HttpContext = defaultContext;

        _orderService.GetOrdersForUserAsync(Arg.Any<string>())
            .Returns(orders);

        // Act
        var sut = new OrdersController(_orderService, _logger, _mapper, httpContextAccessor);
        var result = (NotFoundResult)await sut.GetOrdersForUser();

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetOrdersForUser_ShouldReturnNotFound_WhenNoOrdersExist()
    {
        // Arrange
        _orderService.GetOrdersForUserAsync(Arg.Any<string>())
            .ReturnsNull();

        // Act
        var result = (NotFoundResult)await _sut.GetOrdersForUser();

        // Assert
        result.StatusCode.Should().Be(404);
    }
}
