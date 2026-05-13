using LifeTag.Contracts.DTOs;
using LifeTag.Contracts.Responses;
using LifeTag.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace LifeTag.Service.Implementations
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<ApiResponse<object>> VerifyTagPinAsync(Guid guid, string pin)
        {
            var tag = await _tagRepository.GetByGuidAsync(guid);
            if (tag == null) return ApiResponse<object>.ErrorResponse("Tag not found");

            if (tag.Pin != pin)
            {
                return ApiResponse<object>.ErrorResponse("Invalid PIN");
            }

            return ApiResponse<object>.SuccessResponse(new { IsLinked = tag.UserId.HasValue }, "PIN verified");
        }

        public async Task<ApiResponse<bool>> LinkTagToUserAsync(int userId, Guid guid, string pin)
        {
            // 1. Verify PIN first
            var verifyResult = await VerifyTagPinAsync(guid, pin);
            if (!verifyResult.Success) return ApiResponse<bool>.ErrorResponse(verifyResult.Message);

            // 2. Handle Old Tag Reassignment
            var oldTag = await _tagRepository.GetByUserIdAsync(userId);
            if (oldTag != null)
            {
                oldTag.UserId = null;
                oldTag.IsActive = false;
                await _tagRepository.UpdateAsync(oldTag);
            }

            // 3. Link New Tag
            var newTag = await _tagRepository.GetByGuidAsync(guid);
            if (newTag == null) return ApiResponse<bool>.ErrorResponse("Tag not found");

            newTag.UserId = userId;
            newTag.IsActive = true;
            await _tagRepository.UpdateAsync(newTag);
            await _tagRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Tag linked successfully");
        }

        public async Task<ApiResponse<TagResponseDto>> GetActiveTagAsync(int userId)
        {
            var tag = await _tagRepository.GetByUserIdAsync(userId);
            if (tag == null) return ApiResponse<TagResponseDto>.ErrorResponse("No active tag linked");

            return ApiResponse<TagResponseDto>.SuccessResponse(new TagResponseDto
            {
                Guid = tag.Guid,
                IsActive = tag.IsActive,
                IsPinProtected = tag.IsPinProtected
            });
        }

        public async Task<ApiResponse<TagResponseDto>> GetPinStatusAsync(Guid guid)
        {
            var tag = await _tagRepository.GetByGuidAsync(guid);
            if (tag == null) return ApiResponse<TagResponseDto>.ErrorResponse("Tag not found");

            return ApiResponse<TagResponseDto>.SuccessResponse(new TagResponseDto
            {
                Guid = tag.Guid,
                IsActive = tag.IsActive,
                IsPinProtected = tag.IsPinProtected
            });
        }

        public async Task<ApiResponse<bool>> CheckOwnershipAsync(Guid guid, int userId)
        {
            var tag = await _tagRepository.GetByGuidAsync(guid);
            if (tag == null) return ApiResponse<bool>.ErrorResponse("Tag not found");

            if (tag.UserId == userId)
            {
                return ApiResponse<bool>.SuccessResponse(true, "Ownership verified");
            }

            return ApiResponse<bool>.SuccessResponse(false, "Not the owner");
        }

        public async Task<ApiResponse<bool>> UpdatePinSettingsAsync(int userId, bool isPinProtected, string? pin)
        {
            var tag = await _tagRepository.GetByUserIdAsync(userId);
            if (tag == null) return ApiResponse<bool>.ErrorResponse("No active tag linked to this user");

            tag.IsPinProtected = isPinProtected;
            
            // Only update the PIN if a new one is explicitly provided
            // This prevents overwriting the permanent bracelet PIN with null/empty values
            if (!string.IsNullOrWhiteSpace(pin))
            {
                tag.Pin = pin;
            }

            await _tagRepository.UpdateAsync(tag);
            await _tagRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "PIN settings updated successfully");
        }
    }
}
