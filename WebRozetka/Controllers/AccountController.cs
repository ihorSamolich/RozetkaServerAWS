using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebRozetka.Constants;
using WebRozetka.Data.Entities;
using WebRozetka.Data.Entities.Identity;
using WebRozetka.Helpers;
using WebRozetka.Interfaces;
using WebRozetka.Models;
using WebRozetka.Models.Account;

namespace WebRozetka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private IJwtTokenService _jwtTokenService;

        public AccountController(UserManager<UserEntity> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return BadRequest("Невірно введені дані! Спробуйте ще раз!");
                };

                var roles = await _userManager.GetRolesAsync(user);

                var token = await _jwtTokenService.CreateTokenAsync(user, roles);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest("Помилка авторизації: " + ex);

            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            try
            {
                string imageName = string.Empty;
                if (!string.IsNullOrEmpty(model.ImageBase64))
                {
                    imageName = await ImageWorker.SaveImageAsync(model.ImageBase64);
                }
                var user = new UserEntity
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,
                    Image = imageName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, Roles.User);
                }
                else
                {
                    return BadRequest("Помилка реєстрації!");
                }

                var roles = await _userManager.GetRolesAsync(user);

                var token = await _jwtTokenService.CreateTokenAsync(user, roles);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest("Помилка авторизації: " + ex);
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetUsersCount()
        {
            try
            {
                var count = _userManager.Users.Count();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Помилка сервера: " + ex.Message);
            }
        }

    }
}
