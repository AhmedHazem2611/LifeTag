using LifeTag.Contracts.DTOs;
using LifeTag.Contracts.Responses;
using LifeTag.Core.Entities;
using LifeTag.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace LifeTag.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<AuthResponseDto>> SignUpAsync(SignUpDto signUpDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(signUpDto.Email);
            if (existingUser != null)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("User already exists");
            }

            var user = new User
            {
                FullName = signUpDto.FullName,
                Email = signUpDto.Email,
                PasswordHash = signUpDto.Password
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return ApiResponse<AuthResponseDto>.SuccessResponse(new AuthResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Token = Guid.NewGuid().ToString() // Session token for frontend compatibility
            }, "Signup successful");
        }

        public async Task<ApiResponse<AuthResponseDto>> SignInAsync(SignInDto signInDto)
        {
            var user = await _userRepository.GetByEmailAsync(signInDto.Email);
            if (user == null || user.PasswordHash != signInDto.Password)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid credentials");
            }

            return ApiResponse<AuthResponseDto>.SuccessResponse(new AuthResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Token = Guid.NewGuid().ToString() // Session token for frontend compatibility
            }, "Signin successful");
        }
    }
}
