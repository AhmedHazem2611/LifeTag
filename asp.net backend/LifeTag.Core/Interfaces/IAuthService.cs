using LifeTag.Contracts.DTOs;
using LifeTag.Contracts.Responses;
using System.Threading.Tasks;

namespace LifeTag.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> SignUpAsync(SignUpDto signUpDto);
        Task<ApiResponse<AuthResponseDto>> SignInAsync(SignInDto signInDto);
    }
}
