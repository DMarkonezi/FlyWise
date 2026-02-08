using RedisDataLayer;
using System;
using Microsoft.AspNetCore.Mvc;
using Api.Models.Auth;
using Api.Models.Redis;
using Api.Repositories;
using StackExchange.Redis;
using Utilities;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private RedisAuthManager _authManager;

        public AuthController(
            IConnectionMultiplexer redis,
            UserRepository userRepo,
            JwtService jwtService)
        {
            _authManager = new RedisAuthManager(redis, userRepo, jwtService);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                Console.WriteLine($"Received login request: Email={request?.Email}, Password={(request?.Password != null ? "***" : "null")}");

                if (request == null || string.IsNullOrEmpty(request.Email))
                    return BadRequest("Email is required");

                var response = await _authManager.LoginAsync(request.Email, request.Password);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] dynamic request)
        {
            try
            {
                string? userId = request.GetProperty("userId").GetString();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest("UserId required");

                await _authManager.LogoutAsync(userId);
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateToken([FromBody] dynamic request)
        {
            try
            {
                // string? userId = request?.userId;
                // string? token = request?.token;

                string? userId = request.GetProperty("userId").GetString();
                string? token = request.GetProperty("token").GetString();

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                    return BadRequest("UserId and token required");

                var isValid = await _authManager.ValidateTokenAsync(userId, token);
                return Ok(new { valid = isValid });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}