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
using WebRozetka.Models.Product;

namespace WebRozetka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
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

        [HttpGet]
        public IActionResult GetLastOrders()
        {
            var orders = _context.Orders
            .Include(x => x.OrderContactInfo)
                .ThenInclude(o => o.Warehouses)
                    .ThenInclude(o => o.Settlement)
            .Include(x => x.OrderStatus)
            .Include(x => x.OrderItems)
                .ThenInclude(p => p.Product)
            .OrderByDescending(x => x.DateCreated)
            .Take(10)
            .AsQueryable();

            List<OrderViewModel> orderViews = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                OrderViewModel orderView = new OrderViewModel()
                {
                    CustomerName = order.OrderContactInfo.FirstName + " " + order.OrderContactInfo.LastName,
                    CustomerPhone = order.OrderContactInfo.Phone,
                    PostAddress = order.OrderContactInfo.Warehouses.Settlement.Description + ", " + order.OrderContactInfo.Warehouses.Description,
                    OrderStatus = order.OrderStatus.Name,
                    Products = order.OrderItems.Select(x => x.Product.Name).ToList(),
                };

                orderViews.Add(orderView);
            }

            return Ok(orderViews);
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

        [HttpGet("popular-products")]
        public IActionResult GetTopSoldProducts()
        {
            var topSoldProducts = _context.OrdersItems
                .GroupBy(oi => oi.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    TotalSoldCount = group.Sum(oi => oi.Count)
                })
                .OrderByDescending(result => result.TotalSoldCount)
                .Take(10)
                .ToList();

            var topSoldProductsViewModel = topSoldProducts
                .Select(result => new TopSoldViewModel
                {
                    Id = result.ProductId,
                    Name = _context.Products.Where(x => x.Id == result.ProductId).SingleOrDefault().Name,
                    Count = result.TotalSoldCount,
                })
                .ToList();

            return Ok(topSoldProductsViewModel);
        }

        [HttpGet("popular-categories")]
        public IActionResult GetSalesByCategories()
        {
            var salesByCategories = _context.OrdersItems
                .Include(oi => oi.Product)
                    .ThenInclude(p => p.Category)
                .GroupBy(oi => oi.Product.CategoryId)
                .Select(group => new
                {
                    CategoryId = group.Key,
                    CategoryName = group.First().Product.Category.Name,
                    TotalSoldCount = group.Sum(oi => oi.Count)
                })
                .OrderByDescending(result => result.TotalSoldCount)
                .Take(10)
                .ToList();

            var salesByCategoriesViewModel = salesByCategories
                .Select(result => new TopSoldViewModel
                {
                    Id = result.CategoryId,
                    Name = result.CategoryName,
                    Count = result.TotalSoldCount,
                })
                .ToList();

            return Ok(salesByCategoriesViewModel);
        }

    }
}
