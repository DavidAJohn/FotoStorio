using AutoMapper;
using FotoStorio.Server.Contracts;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FotoStorio.Server.Controllers;

[Authorize]
public class OrdersController : BaseApiController
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _logger = logger;
        _orderService = orderService;
    }

    /// POST api/orders
    /// <summary>
    /// Creates a new customer order
    /// </summary>
    /// <returns>Order</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder(OrderCreateDTO order)
    {
        if (order == null)
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(order.SendToAddress.PostCode) ||
            string.IsNullOrWhiteSpace(order.SendToAddress.City) ||
            string.IsNullOrWhiteSpace(order.SendToAddress.County) ||
            string.IsNullOrWhiteSpace(order.SendToAddress.Street) ||
            string.IsNullOrWhiteSpace(order.SendToAddress.FirstName) ||
            string.IsNullOrWhiteSpace(order.SendToAddress.LastName)
            )
        {
            return BadRequest();
        }

        var email = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email == null)
        {
            return BadRequest();
        }

        var createdOrder = await _orderService.CreateOrderAsync(email, order);

        return Ok(createdOrder);
    }

    /// GET api/orders/{id}
    /// <summary>
    /// Get an order by Id for an authenticated user
    /// </summary>
    /// <param name="id"></param>
    /// <returns>OrderDetailsDTO</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderByIdForUser(int id)
    {
        var email = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email == null)
        {
            return NotFound();
        }

        var order = await _orderService.GetOrderByIdAsync(id, email);

        if (order == null)
        {
            _logger.LogError("Order with id: {id} not found", id);

            return NotFound();
        }
        else
        {
            return Ok(_mapper.Map<Order, OrderDetailsDTO>(order));
        }
    }

    /// GET api/orders/
    /// <summary>
    /// Get existing orders for an authenticated user
    /// </summary>
    /// <returns>IEnumerable<OrderDetailsDTO></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrdersForUser()
    {
        var email = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email == null)
        {
            return NotFound();
        }

        var orders = await _orderService.GetOrdersForUserAsync(email);

        if (orders == null)
        {
            _logger.LogError("Orders for user: '{email}' not found", email);

            return NotFound();
        }
        else
        {
            return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderDetailsDTO>>(orders));
        }
    }
}