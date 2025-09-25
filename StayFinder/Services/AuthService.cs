using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using StayFinder.Interfaces;
using StayFinder.Models;

namespace StayFinder.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _refreshTokens; // In production, use Redis or database

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _refreshTokens = new Dictionary<string, string>();
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(loginRequest.Email);
                
                if (user == null || !user.IsActive)
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }

                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }

                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();
                
                // Store refresh token (in production, use proper storage)
                _refreshTokens[refreshToken] = user.Id.ToString();

                var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");
                
                return new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                    User = UserDto.FromEntity(user)
                };
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                // Check if user already exists
                if (await _userRepository.EmailExistsAsync(registerRequest.Email))
                {
                    throw new InvalidOperationException("User with this email already exists");
                }

                // Validate role
                if (!new[] { "customer", "host" }.Contains(registerRequest.Role.ToLower()))
                {
                    registerRequest.Role = "customer";
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

                var user = new User
                {
                    FirstName = registerRequest.FirstName,
                    LastName = registerRequest.LastName,
                    Email = registerRequest.Email,
                    Phone = registerRequest.Phone,
                    PasswordHash = passwordHash,
                    Role = registerRequest.Role.ToLower(),
                    Address = registerRequest.Address,
                    Bio = registerRequest.Bio,
                    IsActive = true,
                    IsHostApproved = registerRequest.Role.ToLower() == "customer", // Auto-approve customers
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);

                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();
                
                // Store refresh token
                _refreshTokens[refreshToken] = user.Id.ToString();

                var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");

                return new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                    User = UserDto.FromEntity(user)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                if (!ValidateRefreshToken(request.RefreshToken))
                {
                    throw new UnauthorizedAccessException("Invalid refresh token");
                }

                var userId = _refreshTokens[request.RefreshToken];
                var user = await _userRepository.GetByIdAsync(int.Parse(userId));

                if (user == null || !user.IsActive)
                {
                    throw new UnauthorizedAccessException("User not found or inactive");
                }

                var newToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                // Remove old refresh token and add new one
                _refreshTokens.Remove(request.RefreshToken);
                _refreshTokens[newRefreshToken] = user.Id.ToString();

                var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");

                return new AuthResponse
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                    User = UserDto.FromEntity(user)
                };
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                
                if (user == null)
                {
                    return false;
                }

                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                {
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);
                
                if (user == null)
                {
                    return true; // Don't reveal whether email exists
                }

                // In a real application, you would:
                // 1. Generate a reset token
                // 2. Store it with expiration time
                // 3. Send email with reset link
                
                // For demo purposes, just return true
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                // In a real application, you would:
                // 1. Validate the reset token
                // 2. Check if it's not expired
                // 3. Update the password
                
                var user = await _userRepository.GetByEmailAsync(request.Email);
                
                if (user == null)
                {
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync(int userId)
        {
            // Remove all refresh tokens for this user
            var tokensToRemove = _refreshTokens
                .Where(kvp => kvp.Value == userId.ToString())
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var token in tokensToRemove)
            {
                _refreshTokens.Remove(token);
            }

            await Task.CompletedTask;
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
            var issuer = jwtSettings["Issuer"] ?? "StayFinder";
            var audience = jwtSettings["Audience"] ?? "StayFinderUsers";
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("userId", user.Id.ToString()),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("isActive", user.IsActive.ToString()),
                new Claim("isHostApproved", user.IsHostApproved.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            return _refreshTokens.ContainsKey(refreshToken);
        }
    }
}