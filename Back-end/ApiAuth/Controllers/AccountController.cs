using ApiAuth.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebWorker.Data.Entities.Identity;
using WebWorker.Models.Account;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ApiAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<UserEntity> userManager,
            IConfiguration configuration,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestModel model)
        {
            try
            {
                // 1. Валідація вхідних даних
                if (string.IsNullOrEmpty(model.Token))
                {
                    _logger.LogWarning("Отримано порожній Google токен");
                    return BadRequest("Токен обов'язковий.");
                }

                // 2. Валідація Google токена
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] }
                };

                GoogleJsonWebSignature.Payload payload;
                try
                {
                    payload = await GoogleJsonWebSignature.ValidateAsync(model.Token, validationSettings);
                    _logger.LogInformation($"Валідний Google токен для {payload.Email}");
                }
                catch (InvalidJwtException ex)
                {
                    _logger.LogError(ex, "Невірний Google токен");
                    return Unauthorized("Невірний Google токен.");
                }

                // 3. Пошук/створення користувача
                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new UserEntity
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        _logger.LogError("Помилка створення користувача: {Помилки}",
                            string.Join(", ", createResult.Errors.Select(e => e.Description)));
                        return BadRequest(createResult.Errors);
                    }
                }

                // 4. Повернення безпечної відповіді
                return Ok(new
                {
                    user.Id,
                    user.Email,
                    EmailConfirmed = true // Користувачі Google завжди підтверджені
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неочікувана помилка під час входу через Google");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync(); 
            return Ok(users);
        }
    }
}