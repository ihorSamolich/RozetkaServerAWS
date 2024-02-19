using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebRozetka.Data;
using WebRozetka.Data.Entities.Identity;
using WebRozetka.Data.Entities.Order;
using WebRozetka.Models.Basket;
using WebRozetka.Models.Order;

namespace WebRozetka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly AppEFContext _context;
        private readonly IMapper _mapper;

        public OrderController(AppEFContext context, UserManager<UserEntity> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderViewModel model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    string userEmail = User.Claims.First().Value;
                    var user = await _userManager.FindByEmailAsync(userEmail);

                    var order = new OrderEntity
                    {
                        UserId = user.Id,
                        DateCreated = DateTime.UtcNow,
                        OrderStatusId = 1,
                    };

                    _context.Orders.Add(order);
                    _context.SaveChanges();

                    var orderContactInfo = new OrderContactInfoEntity
                    {
                        OrderId = order.Id,
                        FirstName = model.CustomerPersonalData.FirstName,
                        LastName = model.CustomerPersonalData.LastName,
                        Phone = model.CustomerPersonalData.Phone,
                        WarehousesId = model.DepartmentData.WarehouseId
                    };

                    _context.OrderContactInfos.Add(orderContactInfo);
                    _context.SaveChanges();

                    var baskets = _context.Baskets
                        .Include(x => x.Product)
                        .Where(x => x.UserId == user.Id);

                    foreach (var itemBasket in baskets)
                    {
                        var orderItem = new OrderItemsEntity
                        {
                            OrderId = order.Id,
                            ProductId = itemBasket.ProductId,
                            Count = itemBasket.Count,
                            Price = itemBasket.Product.Price,
                            DateCreated = DateTime.UtcNow
                        };

                        itemBasket.Product.Quantity -= itemBasket.Count;

                        _context.OrdersItems.Add(orderItem);
                    }
                    _context.SaveChanges();

                    transaction.Commit();

                    return Ok();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return BadRequest("Помилка при створенні замовлення.");
                }
            }
        }

    }
}
