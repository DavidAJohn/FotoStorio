using FotoStorio.Server.Specifications;
using FotoStorio.Shared.Entities;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Server.Services;

public class OrderServiceTests
{
    private readonly OrderService _sut;
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IPaymentService _paymentService = Substitute.For<IPaymentService>();

    public OrderServiceTests()
    {
        _sut = new OrderService(_productRepository, _orderRepository, _paymentService);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldReturnOrder_WhenOrderCreated()
    {
        // Arrange
        Fixture fixture = new();
        var order = fixture.Create<OrderCreateDTO>();
        var buyerEmail = "test@test.com";

        // Act
        _productRepository.GetByIdAsync(Arg.Any<int>()).Returns(fixture.Create<Product>());
        _orderRepository.GetEntityWithSpecification(Arg.Any<OrderByPaymentIntentIdSpecification>()).Returns(fixture.Create<Order>());
        _orderRepository.Delete(Arg.Any<Order>()).Returns(true);
        _paymentService.CreateOrUpdatePaymentIntent(Arg.Any<PaymentIntentCreateDTO>()).Returns(fixture.Create<PaymentIntentResult>());
        _orderRepository.Create(Arg.Any<Order>()).Returns(fixture.Create<Order>());

        var result = await _sut.CreateOrderAsync(buyerEmail, order);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Order>();
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldReturnNull_WhenOrderWasNotCreated()
    {
        // Arrange
        Fixture fixture = new();
        var order = fixture.Create<OrderCreateDTO>();
        var buyerEmail = "test@test.com";

        // Act
        _productRepository.GetByIdAsync(Arg.Any<int>()).Returns(fixture.Create<Product>());
        _orderRepository.GetEntityWithSpecification(Arg.Any<OrderByPaymentIntentIdSpecification>()).Returns(fixture.Create<Order>());
        _orderRepository.Delete(Arg.Any<Order>()).Returns(true);
        _paymentService.CreateOrUpdatePaymentIntent(Arg.Any<PaymentIntentCreateDTO>()).Returns(fixture.Create<PaymentIntentResult>());
        
        _orderRepository.Create(Arg.Any<Order>()).ReturnsNull();

        var result = await _sut.CreateOrderAsync(buyerEmail, order);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        Fixture fixture = new();
        var order = fixture.Create<Order>();
        var buyerEmail = "test@test.com";
        order.BuyerEmail = buyerEmail;

        // Act
        _orderRepository.GetEntityWithSpecification(Arg.Any<OrdersWithItemsForUserSpecification>()).Returns(order);
        var result = await _sut.GetOrderByIdAsync(order.Id, buyerEmail);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Order>();
        result.Should().Be(order);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        Fixture fixture = new();
        var orders = fixture.Create<List<Order>>();
        var buyerEmail = "test@test.com";

        // Act
        _orderRepository.ListWithSpecificationAsync(Arg.Any<OrdersWithItemsForUserSpecification>()).Returns(orders);
        var result = await _sut.GetOrdersForUserAsync(buyerEmail);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<Order>>();
    }
}
