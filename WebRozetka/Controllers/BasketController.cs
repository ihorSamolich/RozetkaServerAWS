using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebRozetka.Data.Entities.Identity;
using WebRozetka.Data;
using Microsoft.EntityFrameworkCore;
using WebRozetka.Data.Entities.Order;
using WebRozetka.Models.Basket;

namespace WebRozetka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly AppEFContext _context;
        private readonly IMapper _mapper;

        public BasketController(AppEFContext context, UserManager<UserEntity> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToBasket([FromBody] BasketAddViewModel model)
        {
            string userEmail = User.Claims.First().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return BadRequest("User not found!");
            }

            var basketItem = _context.Baskets
                .Include(x => x.Product)
                    .ThenInclude(p => p.Photos)
                .FirstOrDefault(x => x.UserId == user.Id && x.ProductId == model.ProductId);

            if (basketItem == null)
            {
                var product = await _context.Products
                    .Include(p => p.Photos)
                    .FirstOrDefaultAsync(p => p.Id == model.ProductId);

                if (product == null)
                {
                    return BadRequest("Product not found!");
                }

                basketItem = new BasketEntity
                {
                    ProductId = model.ProductId,
                    UserId = user.Id,
                    Count = model.Count,
                    DateCreated = DateTime.UtcNow,
                    Product = product
                };

                _context.Baskets.Add(basketItem);
            }
            else
            {
                basketItem.Count += model.Count;
            }

            await _context.SaveChangesAsync();

            var result = _mapper.Map<BasketItemViewModel>(basketItem);

            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> ListBasket()
        {
            string userEmail = User.Claims.First().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return BadRequest("User not found!");
            }

            var listBasketItem = _context.Baskets
                .Include(x => x.Product)
                    .ThenInclude(p => p.Photos)
                .Where(x => x.UserId == user.Id)
                .Select(x => _mapper.Map<BasketItemViewModel>(x));

            return Ok(listBasketItem);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearBasket()
        {
            string userEmail = User.Claims.First().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return BadRequest("User not found!");
            }

            var listBasketItem = _context.Baskets
                .Where(x => x.UserId == user.Id);

            _context.Baskets.RemoveRange(listBasketItem);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromBasket(int productId)
        {
            string userEmail = User.Claims.First().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return BadRequest("User not found!");
            }

            var basketItem = _context.Baskets
                .Where(x => x.UserId == user.Id && x.ProductId == productId)
                .FirstOrDefault();

            if (basketItem != null)
            {
                _context.Baskets.Remove(basketItem);
                await _context.SaveChangesAsync();

                return Ok(basketItem.ProductId);
            }
            else
            {
                return NotFound("Product with id not found!");
            }
        }
    }
}
