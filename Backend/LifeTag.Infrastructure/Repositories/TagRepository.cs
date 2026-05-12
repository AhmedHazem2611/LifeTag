using LifeTag.Core.Entities;
using LifeTag.Core.Interfaces;
using LifeTag.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LifeTag.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly LifeTagDbContext _context;

        public TagRepository(LifeTagDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetByGuidAsync(Guid guid)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Guid == guid);
        }

        public async Task<Tag?> GetByUserIdAsync(int userId)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
