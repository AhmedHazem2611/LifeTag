using LifeTag.Contracts.DTOs;
using LifeTag.Contracts.Responses;
using System;
using System.Threading.Tasks;

namespace LifeTag.Core.Interfaces
{
    public interface ITagService
    {
        Task<ApiResponse<object>> VerifyTagPinAsync(Guid guid, string pin);
        Task<ApiResponse<bool>> LinkTagToUserAsync(int userId, Guid guid, string pin);
        Task<ApiResponse<TagResponseDto>> GetActiveTagAsync(int userId);
        Task<ApiResponse<TagResponseDto>> GetPinStatusAsync(Guid guid);
        Task<ApiResponse<bool>> CheckOwnershipAsync(Guid guid, int userId);
        Task<ApiResponse<bool>> UpdatePinSettingsAsync(int userId, bool isPinProtected, string? pin);
    }

    public class TagResponseDto
    {
        public Guid Guid { get; set; }
        public bool IsActive { get; set; }
        public bool IsPinProtected { get; set; }
    }
}
