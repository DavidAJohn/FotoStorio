using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FotoStorio.Server.Contracts;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FotoStorio.Server.Controllers
{
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
        /// <returns>OrderDTO</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDTO>> CreateOrder(OrderCreateDTO order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            try
            {
                var email = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var createdOrder = await _orderService.CreateOrderAsync(email, order);

                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateOrder : {ex.Message}");

                return BadRequest();
            }
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderByIdForUser(int id)
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var order = await _orderService.GetOrderByIdAsync(id, email);

                if (order == null)
                {
                    _logger.LogError($"Order with id: {id}, not found");

                    return NotFound();
                }
                else
                {
                    return _mapper.Map<Order, OrderDetailsDTO>(order);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetOrderById, from id {id} : {ex.Message}");

                return StatusCode(500, "Internal server error");
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OrderDetailsDTO>>> GetOrdersForUser()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var orders = await _orderService.GetOrdersForUserAsync(email);

                if (orders == null)
                {
                    _logger.LogError($"Orders for user: {email}, not found");

                    return NotFound();
                }
                else
                {
                    return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderDetailsDTO>>(orders));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetOrdersForUser: {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }
    }
}