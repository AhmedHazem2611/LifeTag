using LifeTag.Contracts.DTOs;
using LifeTag.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LifeTag.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpPost("verify-pin")]
        public async Task<IActionResult> VerifyPin([FromBody] TagVerifyDto dto)
        {
            if (!Guid.TryParse(dto.Guid, out var guid))
            {
                // If the frontend only sends PIN initially (old behavior), 
                // we might need a way to find the tag. 
                // However, per instructions, we are moving to GUID-based scans.
                return BadRequest(new { success = false, message = "Invalid Tag GUID" });
            }

            var result = await _tagService.VerifyTagPinAsync(guid, dto.Pin);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("link-tag")]
        public async Task<IActionResult> LinkTag([FromBody] TagVerifyDto dto, [FromQuery] int userId)
        {
            if (!Guid.TryParse(dto.Guid, out var guid))
                return BadRequest(new { success = false, message = "Invalid Tag GUID" });

            var result = await _tagService.LinkTagToUserAsync(userId, guid, dto.Pin);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("active-tag/{userId}")]
        public async Task<IActionResult> GetActiveTag(int userId)
        {
            var result = await _tagService.GetActiveTagAsync(userId);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("tag/{tagGuid}/pin-status")]
        public async Task<IActionResult> GetPinStatus(Guid tagGuid)
        {
            var result = await _tagService.GetPinStatusAsync(tagGuid);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("tag/{tagGuid}/owner")]
        public async Task<IActionResult> GetOwnerStatus(Guid tagGuid, [FromQuery] int userId)
        {
            var result = await _tagService.CheckOwnershipAsync(tagGuid, userId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("tag/{userId}/pin-settings")]
        public async Task<IActionResult> UpdatePinSettings(int userId, [FromBody] TagPinSettingsDto dto)
        {
            var result = await _tagService.UpdatePinSettingsAsync(userId, dto.IsPinProtected, dto.Pin);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
