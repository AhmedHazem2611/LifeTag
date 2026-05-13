using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LifeTag.Infrastructure.Data;
using LifeTag.Contracts.DTOs;
using LifeTag.Core.Entities;

namespace LifeTag.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly LifeTagDbContext _context;

        public AdminController(LifeTagDbContext context)
        {
            _context = context;
        }

        // --- Tags ---

        [HttpGet("tags")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _context.Tags
                .Include(t => t.User)
                    .ThenInclude(u => u.Profile)
                .Select(t => new AdminTagDto
                {
                    Id = t.Id,
                    Guid = t.Guid.ToString(),
                    Pin = t.Pin,
                    IsActive = t.IsActive,
                    IsPinProtected = t.IsPinProtected,
                    LinkedUserId = t.UserId,
                    LinkedUserName = t.User != null ? t.User.FullName : null,
                    TemplateType = t.User != null && t.User.Profile != null ? t.User.Profile.TemplateType : null
                })
                .ToListAsync();

            return Ok(tags);
        }

        [HttpPost("tags")]
        public async Task<IActionResult> CreateTag(AdminTagDto dto)
        {
            var tag = new Tag
            {
                Guid = Guid.TryParse(dto.Guid, out var g) ? g : Guid.NewGuid(),
                Pin = string.IsNullOrEmpty(dto.Pin) ? "1234" : dto.Pin,
                IsPinProtected = true,
                IsActive = false
            };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return Ok(tag);
        }

        [HttpPut("tags/{id}")]
        public async Task<IActionResult> UpdateTag(int id, AdminTagDto dto)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return NotFound();

            tag.Pin = dto.Pin;
            tag.IsPinProtected = dto.IsPinProtected;
            
            // Lifecycle Logic: Handle Link/Unlink
            if (dto.LinkedUserId != tag.UserId)
            {
                tag.UserId = dto.LinkedUserId;
                tag.IsActive = dto.LinkedUserId.HasValue;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("tags/{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return NotFound();

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // --- Users ---

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.ActiveTag)
                .Include(u => u.Profile)
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    LinkedTagGuid = u.ActiveTag != null ? u.ActiveTag.Guid.ToString() : null,
                    TemplateType = u.Profile != null ? u.Profile.TemplateType : null
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser(AdminUserDto dto)
        {
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = "password123" // Default password for admin-created users
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.Include(u => u.ActiveTag).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            // Lifecycle: Unlink tag before deleting user
            if (user.ActiveTag != null)
            {
                user.ActiveTag.UserId = null;
                user.ActiveTag.IsActive = false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // --- System Reset ---

        [HttpPost("reset-system")]
        public async Task<IActionResult> ResetSystem()
        {
            // 1. Delete all users (cascades to profiles, sections, entries)
            var users = await _context.Users.ToListAsync();
            _context.Users.RemoveRange(users);

            // 2. Reset all tags to unlinked, inactive, and PIN protected state
            var tags = await _context.Tags.ToListAsync();
            foreach (var tag in tags)
            {
                tag.UserId = null;
                tag.IsActive = false;
                tag.IsPinProtected = true;
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "System reset complete. All users deleted and tags unlinked." });
        }
    }
}
