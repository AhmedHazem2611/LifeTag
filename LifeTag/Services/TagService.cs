using LifeTag.Models;

namespace LifeTag.Services
{
    public class TagService
    {
        private readonly LifeTagContext _context;

        public TagService(LifeTagContext context)
        {
            _context = context;
        }

        public bool TagExists(Guid tagId)
        {
            return _context.Tags.Any(t => t.Id == tagId);
        }

        public bool IsTagActive(Guid tagId)
        {
            var tag = _context.Tags.FirstOrDefault(t => t.Id == tagId);
            return tag != null && tag.IsActive;
        }

        public bool ValidatePin(Guid tagId, string enteredPin)
        {
            var tag = _context.Tags.FirstOrDefault(t => t.Id == tagId);

            if (tag == null)
                return false;

            return tag.Pin == enteredPin;
        }

        public void ActivateTag(Guid tagId, Guid userId)
        {
            var tag = GetTag(tagId);
            if (tag == null)
                throw new Exception("Invalid tag");
            if (tag.IsActive)
                throw new Exception("Tag already active");
            tag.UserId = userId;
            tag.IsActive = true;

            _context.SaveChanges();
        }

        public Tag? GetTag(Guid tagId)
        {
            return _context.Tags.FirstOrDefault(t => t.Id == tagId);
        }
    }
}
