using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LifeTag.Contracts.DTOs
{
    public class ProfileDto
    {
        public int? UserId { get; set; }
        public string? TemplateType { get; set; } // Medical, Child, Custom
        public string? FullName { get; set; }
        public string? Dob { get; set; }
        public string? BloodType { get; set; }
        public string[]? MedicalConditions { get; set; }
        public string[]? Medications { get; set; }
        public string[]? Allergies { get; set; }
        public List<EmergencyContactDto>? EmergencyContacts { get; set; }
        public string? Notes { get; set; }
        public string? Address { get; set; }
        public List<CustomSectionDto>? CustomSections { get; set; }
        
        // PIN protection settings
        public bool? IsPinProtected { get; set; }
        public string? Pin { get; set; }

        // Stable ID Mappings for frontend synchronization
        public Dictionary<string, int> SectionIds { get; set; } = new Dictionary<string, int>();
    }

    public class EmergencyContactDto
    {
        public int? Id { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("phone")]
        public string Phone { get => PhoneNumber; set => PhoneNumber = value; }

        [System.Text.Json.Serialization.JsonPropertyName("relation")]
        public string Relation { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("type")]
        public string Type { get => Relation; set => Relation = value; }
    }

    public class CustomSectionDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Items { get; set; } = new List<string>();
    }

    public class UpdateSectionDto
    {
        public string Title { get; set; } = string.Empty;
        public string SectionType { get; set; } = string.Empty;
        public object? Data { get; set; }
    }

    public class TagVerifyDto
    {
        [Required]
        public string Guid { get; set; } = string.Empty;
        [Required]
        public string Pin { get; set; } = string.Empty;
    }

    public class TagPinSettingsDto
    {
        public bool IsPinProtected { get; set; }
        public string? Pin { get; set; }
    }
}
