using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FotoStorio.Server.Contracts;
using FotoStorio.Shared.DTOs;
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
                var address = order.SendToAddress;

                var createdOrder = await _orderService.CreateOrderAsync(email, order, address);

                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateOrder : {ex.Message}");

                return BadRequest();
            }
        }

    }
}