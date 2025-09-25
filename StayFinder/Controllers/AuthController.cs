using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StayFinder.Interfaces;
using StayFinder.Models;
using System.Security.Claims;

namespace StayFinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Login user with email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(loginRequest);
                return Ok(new { success = true, data = result, message = "Login successful" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", loginRequest.Email);
                return StatusCode(500, new { success = false, message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Register new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterAsync(registerRequest);
                return Ok(new { success = true, data = result, message = "Registration successful" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", registerRequest.Email);
                return StatusCode(500, new { success = false, message = "An error occurred during registration" });
            }
        }

        /// <summary>
        /// Refresh JWT token using refresh token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RefreshTokenAsync(request);
                return Ok(new { success = true, data = result, message = "Token refreshed successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new { success = false, message = "An error occurred during token refresh" });
            }
        }

        /// <summary>
        /// Change user password (requires authentication)
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                
                if (userId == 0)
                {
                    return Unauthorized(new { success = false, message = "Invalid user" });
                }

                var result = await _authService.ChangePasswordAsync(userId, request);
                
                if (result)
                {
                    return Ok(new { success = true, message = "Password changed successfully" });
                }
                
                return BadRequest(new { success = false, message = "Current password is incorrect" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change for user: {UserId}", User.FindFirst("userId")?.Value);
                return StatusCode(500, new { success = false, message = "An error occurred while changing password" });
            }
        }

        /// <summary>
        /// Request password reset (send reset email)
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.ForgotPasswordAsync(request);
                
                // Always return success to prevent email enumeration
                return Ok(new { success = true, message = "If the email exists, a password reset link has been sent" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for email: {Email}", request.Email);
                return StatusCode(500, new { success = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Reset password using reset token
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.ResetPasswordAsync(request);
                
                if (result)
                {
                    return Ok(new { success = true, message = "Password reset successfully" });
                }
                
                return BadRequest(new { success = false, message = "Invalid or expired reset token" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for email: {Email}", request.Email);
                return StatusCode(500, new { success = false, message = "An error occurred while resetting password" });
            }
        }

        /// <summary>
        /// Logout user (invalidate refresh tokens)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                
                if (userId == 0)
                {
                    return Unauthorized(new { success = false, message = "Invalid user" });
                }

                await _authService.LogoutAsync(userId);
                
                return Ok(new { success = true, message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user: {UserId}", User.FindFirst("userId")?.Value);
                return StatusCode(500, new { success = false, message = "An error occurred during logout" });
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
                
                var currentUser = new
                {
                    id = int.Parse(userClaims.GetValueOrDefault("userId", "0")),
                    email = userClaims.GetValueOrDefault(ClaimTypes.Email, ""),
                    firstName = userClaims.GetValueOrDefault("firstName", ""),
                    lastName = userClaims.GetValueOrDefault("lastName", ""),
                    role = userClaims.GetValueOrDefault(ClaimTypes.Role, ""),
                    isActive = bool.Parse(userClaims.GetValueOrDefault("isActive", "false")),
                    isHostApproved = bool.Parse(userClaims.GetValueOrDefault("isHostApproved", "false"))
                };

                return Ok(new { success = true, data = currentUser });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user profile");
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving user profile" });
            }
        }

        /// <summary>
        /// Validate JWT token
        /// </summary>
        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            return Ok(new { success = true, message = "Token is valid", userId = User.FindFirst("userId")?.Value });
        }
    }
}