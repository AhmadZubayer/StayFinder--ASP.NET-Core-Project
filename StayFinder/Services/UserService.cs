using AutoMapper;
using StayFinder.Interfaces;
using StayFinder.Models;

namespace StayFinder.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<UserDto>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("User not found");
                }

                var userDto = _mapper.Map<UserDto>(user);
                return ApiResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error retrieving user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("User not found");
                }

                var userDto = _mapper.Map<UserDto>(user);
                return ApiResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error retrieving user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
                return ApiResponse<IEnumerable<UserDto>>.SuccessResponse(userDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<UserDto>>.ErrorResponse($"Error retrieving users: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetUsersByRoleAsync(string role)
        {
            try
            {
                var users = await _userRepository.GetUsersByRoleAsync(role);
                var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
                return ApiResponse<IEnumerable<UserDto>>.SuccessResponse(userDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<UserDto>>.ErrorResponse($"Error retrieving users by role: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UserCreateDto userDto)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("User not found");
                }

                // Check if email is being changed and if new email already exists
                if (!string.Equals(existingUser.Email, userDto.Email, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _userRepository.EmailExistsAsync(userDto.Email))
                    {
                        return ApiResponse<UserDto>.ErrorResponse("Email already exists");
                    }
                }

                // Update user properties
                existingUser.FirstName = userDto.FirstName;
                existingUser.LastName = userDto.LastName;
                existingUser.Email = userDto.Email;
                existingUser.Phone = userDto.Phone;
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(existingUser);

                var updatedUserDto = _mapper.Map<UserDto>(existingUser);
                return ApiResponse<UserDto>.SuccessResponse(updatedUserDto, "User updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error updating user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("User not found");
                }

                // Soft delete - deactivate user instead of deleting
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                return ApiResponse<bool>.SuccessResponse(true, "User deactivated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ActivateUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("User not found");
                }

                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                return ApiResponse<bool>.SuccessResponse(true, "User activated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error activating user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeactivateUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("User not found");
                }

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                return ApiResponse<bool>.SuccessResponse(true, "User deactivated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deactivating user: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ApproveHostAsync(int hostId)
        {
            try
            {
                var host = await _userRepository.GetByIdAsync(hostId);
                if (host == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Host not found");
                }

                if (host.Role != "host")
                {
                    return ApiResponse<bool>.ErrorResponse("User is not a host");
                }

                host.IsHostApproved = true;
                host.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(host);

                return ApiResponse<bool>.SuccessResponse(true, "Host approved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error approving host: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RejectHostAsync(int hostId)
        {
            try
            {
                var host = await _userRepository.GetByIdAsync(hostId);
                if (host == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Host not found");
                }

                if (host.Role != "host")
                {
                    return ApiResponse<bool>.ErrorResponse("User is not a host");
                }

                host.IsHostApproved = false;
                host.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(host);

                return ApiResponse<bool>.SuccessResponse(true, "Host rejected successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error rejecting host: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetPendingHostsAsync()
        {
            try
            {
                var hosts = await _userRepository.GetUsersByRoleAsync("host");
                var pendingHosts = hosts.Where(h => !h.IsHostApproved);
                var hostDtos = _mapper.Map<IEnumerable<UserDto>>(pendingHosts);
                return ApiResponse<IEnumerable<UserDto>>.SuccessResponse(hostDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<UserDto>>.ErrorResponse($"Error retrieving pending hosts: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserWithBookingsAsync(userId);
                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("User not found");
                }

                var userDto = _mapper.Map<UserDto>(user);
                return ApiResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error retrieving user profile: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> UpdateUserProfileAsync(int userId, UserCreateDto userDto)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(userId);
                if (existingUser == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("User not found");
                }

                // Check if email is being changed and if new email already exists
                if (!string.Equals(existingUser.Email, userDto.Email, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _userRepository.EmailExistsAsync(userDto.Email))
                    {
                        return ApiResponse<UserDto>.ErrorResponse("Email already exists");
                    }
                }

                // Update user properties
                existingUser.FirstName = userDto.FirstName;
                existingUser.LastName = userDto.LastName;
                existingUser.Email = userDto.Email;
                existingUser.Phone = userDto.Phone;
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(existingUser);

                var updatedUserDto = _mapper.Map<UserDto>(existingUser);
                return ApiResponse<UserDto>.SuccessResponse(updatedUserDto, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error updating profile: {ex.Message}");
            }
        }
    }
}