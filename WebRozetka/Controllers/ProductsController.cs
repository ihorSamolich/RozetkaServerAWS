using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using WebRozetka.Data;
using WebRozetka.Data.Entities.Photo;
using WebRozetka.Data.Entities.Product;
using WebRozetka.Helpers;
using WebRozetka.Interfaces.Repo;
using WebRozetka.Models;
using WebRozetka.Models.Product;
using WebRozetka.Repository;

namespace WebRozetka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IPhotoRepository _photoRepository;

        private readonly AppEFContext _context;
        private readonly IMapper _mapper;

        public ProductsController(AppEFContext context, IMapper mapper, IProductRepository productRepository, IPhotoRepository photoRepository)
        {
            _context = context;
            _mapper = mapper;
            _productRepository = productRepository;
            _photoRepository = photoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var count = await _productRepository.GetCountAsync(queryParameters);
                var items = _mapper.Map<List<ProductViewModel>>(_productRepository.GetAll(queryParameters));

                return Ok(new { count, items });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Сталася помилка при створенні продукту: " + ex.Message);
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var productViewModels = _mapper.Map<List<ProductViewModel>>(_productRepository.GetByCategory(categoryId));

                if (productViewModels == null)
                {
                    return NotFound("No products found.");
                }

                return Ok(productViewModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Сталася помилка при створенні продукту: " + ex.Message);
            }
        }


        [HttpGet("hot-sales")]
        public async Task<IActionResult> GetHotSalesProducts()
        {
            try
            {
                var productViewModels = _mapper.Map<List<ProductViewModel>>(_productRepository.GetAll());

                var res = productViewModels.OrderByDescending(x => x.Discount).Where(x => x.Discount > 0 && x.Quantity > 1).Take(20);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Сталася помилка при створенні продукту: " + ex.Message);
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetProductsCount()
        {
            try
            {
                var count = await _productRepository.GetCountAsync(new QueryParameters());
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Помилка сервера: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = _mapper.Map<ProductViewModel>(await _productRepository.GetByIdAsync(id));

                if (product == null)
                {
                    return NotFound("No product found.");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Сталася помилка при створенні продукту: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Новий продукт є пустим!");
            }

            try
            {
                var product = _productRepository.AddAsync(_mapper.Map<ProductEntity>(model));
                await _productRepository.Save();

                foreach (var image in model.Images)
                {
                    var imagePath = await ImageWorker.SaveImageAsync(image);

                    _photoRepository.AddAsync(new PhotoEntity { FilePath = imagePath, ProductId = product.Id });
                }

                await _photoRepository.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Сталася помилка при створенні продукту: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductEditViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Новий продукт є пустим!");
            }

            try
            {
                var product = _productRepository.Update(_mapper.Map<ProductEntity>(model));
                await _productRepository.Save();

                var oldPhotos = _photoRepository.GetAllByProduct(model.Id);


                if (oldPhotos != null)
                {
                    foreach (var photo in oldPhotos)
                    {
                        if (!model.OldPhotos.Any(x => x.Photo == photo.FilePath))
                        {
                            _photoRepository.DeleteAsync(photo.Id);
                            ImageWorker.RemoveImage(photo.FilePath);
                        }
                        else
                        {
                            photo.Priority = model.OldPhotos.SingleOrDefault(x => x.Photo == photo.FilePath).Priority;
                        }
                    }
                }
                if (model.NewPhotos != null)
                {
                    foreach (var image in model.NewPhotos)
                    {
                        var imagePath = await ImageWorker.SaveImageAsync(image.Photo);

                        _photoRepository.AddAsync(new PhotoEntity { FilePath = imagePath, ProductId = product.Id, Priority = image.Priority });
                    }
                }

                await _photoRepository.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Сталася помилка при створенні продукту: " + ex.Message);
            }
        }
    }
}
