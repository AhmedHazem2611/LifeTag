using LifeTag.Contracts.DTOs;
using LifeTag.Contracts.Responses;
using System;
using System.Threading.Tasks;

namespace LifeTag.Core.Interfaces
{
    public interface IProfileService
    {
        Task<ApiResponse<ProfileDto>> GetProfileByUserIdAsync(int userId);
        Task<ApiResponse<ProfileDto>> GetPublicProfileAsync(Guid tagGuid, string? pin = null);
        Task<ApiResponse<ProfileDto>> SaveProfileAsync(ProfileDto profileDto);
        Task<ApiResponse<int>> AddSectionAsync(int userId, UpdateSectionDto dto);
        Task<ApiResponse<bool>> UpdateSectionAsync(int userId, int sectionId, UpdateSectionDto dto);
        Task<ApiResponse<bool>> DeleteSectionAsync(int userId, int sectionId);
        Task<ApiResponse<bool>> ReorderSectionsAsync(int userId, System.Collections.Generic.List<int> orderedSectionIds);
    }
}
