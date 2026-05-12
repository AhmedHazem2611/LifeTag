using LifeTag.Contracts.DTOs;
using LifeTag.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LifeTag.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] int userId)
        {
            var result = await _profileService.GetProfileByUserIdAsync(userId);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("medical-data/{userId}")]
        public async Task<IActionResult> GetMedicalData(int userId)
        {
            var result = await _profileService.GetProfileByUserIdAsync(userId);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("save-medical-data")]
        public async Task<IActionResult> SaveMedicalData([FromBody] ProfileDto dto)
        {
            var result = await _profileService.SaveProfileAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("public-profile/{tagGuid}")]
        public async Task<IActionResult> GetPublicProfile(Guid tagGuid, [FromQuery] string? pin = null)
        {
            var result = await _profileService.GetPublicProfileAsync(tagGuid, pin);
            if (!result.Success)
            {
                // If PIN is required, return a specific status or message
                if (result.Message == "PIN verification required")
                {
                    return Unauthorized(result);
                }
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("profile/{userId}/section")]
        public async Task<IActionResult> AddSection(int userId, [FromBody] UpdateSectionDto dto)
        {
            var result = await _profileService.AddSectionAsync(userId, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("profile/{userId}/section/{sectionId}")]
        public async Task<IActionResult> UpdateSection(int userId, int sectionId, [FromBody] UpdateSectionDto dto)
        {
            var result = await _profileService.UpdateSectionAsync(userId, sectionId, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("profile/{userId}/section/{sectionId}")]
        public async Task<IActionResult> DeleteSection(int userId, int sectionId)
        {
            var result = await _profileService.DeleteSectionAsync(userId, sectionId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("profile/{userId}/sections/reorder")]
        public async Task<IActionResult> ReorderSections(int userId, [FromBody] System.Collections.Generic.List<int> orderedSectionIds)
        {
            var result = await _profileService.ReorderSectionsAsync(userId, orderedSectionIds);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
