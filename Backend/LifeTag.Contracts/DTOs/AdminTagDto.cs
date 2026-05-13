namespace LifeTag.Contracts.DTOs
{
    public class AdminTagDto
    {
        public int Id { get; set; }
        public string Guid { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsPinProtected { get; set; }
        public int? LinkedUserId { get; set; }
        public string? LinkedUserName { get; set; }
        public string? TemplateType { get; set; }
    }
}
