using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebRozetka.Data.Entities.Category;
using WebRozetka.Helpers;
using WebRozetka.Interfaces.Repo;
using WebRozetka.Models;
using WebRozetka.Models.Category;

namespace WebRozetka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var count = await _categoryRepository.GetCountAsync(queryParameters);
                var items = _categoryRepository.GetAll(queryParameters);

                return Ok(new { count, items });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Помилка сервера: " + ex.Message);
            }
        }

        [HttpGet("names")]
        public async Task<IActionResult> GetCategoriesNames()
        {
            try
            {
                var items = _categoryRepository.GetAll();

                var result = items.Select(item => new { item.Name, item.Id });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Помилка сервера: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var item = await _categoryRepository.GetByIdAsync(id);

                if (item == null)
                {
                    return NotFound("Категорію не знайдено!");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Помилка сервера: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryCreateViewModel model)
        {
            try
            {
                var item = _mapper.Map<CategoryEntity>(model);
                var newEntity = _categoryRepository.AddAsync(item);
                await _categoryRepository.Save();

                if (newEntity == null)
                {
                    return BadRequest("Помилка створення категорії!");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Помилка сервера: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] CategoryEditViewModel model)
        {
            try
            {
                var item = await _categoryRepository.GetByIdAsync(model.Id);

                if (item == null)
                {
                    return NotFound("Категорію не знайдено!");
                }

                if (model.Image != null)
                {
                    ImageWorker.RemoveImage(item.Image);
                    item.Image = await ImageWorker.SaveImageAsync(model.Image);
                }

                item.Name = model.Name;
                item.Description = model.Description;

                var updatedItem = _categoryRepository.Update(item);
                await _categoryRepository.Save();

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Помилка сервера: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _categoryRepository.DeleteAsync(id);

                var result = await _categoryRepository.Save();

                if (!result)
                {
                    return BadRequest("Помилка видалення категорії!");
                }

                return Ok("Категорія успішно видалена!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Помилка сервера: " + ex.Message);

            }
        }
    }
}
