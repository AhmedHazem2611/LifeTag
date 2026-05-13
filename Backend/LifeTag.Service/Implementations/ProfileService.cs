using LifeTag.Contracts.DTOs;
using LifeTag.Contracts.Responses;
using LifeTag.Core.Entities;
using LifeTag.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LifeTag.Core.Constants;

namespace LifeTag.Service.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly IUserProfileRepository _profileRepository;
        private readonly ITagRepository _tagRepository;

        public ProfileService(IUserProfileRepository profileRepository, ITagRepository tagRepository)
        {
            _profileRepository = profileRepository;
            _tagRepository = tagRepository;
        }

        public async Task<ApiResponse<ProfileDto>> GetProfileByUserIdAsync(int userId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null) return ApiResponse<ProfileDto>.ErrorResponse("Profile not found");

            return ApiResponse<ProfileDto>.SuccessResponse(MapToDto(profile));
        }

        public async Task<ApiResponse<ProfileDto>> GetPublicProfileAsync(Guid tagGuid, string? pin = null)
        {
            var tag = await _tagRepository.GetByGuidAsync(tagGuid);
            if (tag == null || !tag.IsActive) return ApiResponse<ProfileDto>.ErrorResponse("Inactive or invalid tag");

            if (tag.IsPinProtected)
            {
                if (string.IsNullOrEmpty(pin) || tag.Pin != pin)
                {
                    return ApiResponse<ProfileDto>.ErrorResponse("PIN verification required");
                }
            }

            var profile = await _profileRepository.GetByUserIdAsync(tag.UserId!.Value);
            return ApiResponse<ProfileDto>.SuccessResponse(MapToDto(profile!));
        }

        public async Task<ApiResponse<ProfileDto>> SaveProfileAsync(ProfileDto dto)
        {
            if (dto.UserId == null) return ApiResponse<ProfileDto>.ErrorResponse("UserId is required");

            var profile = await _profileRepository.GetByUserIdAsync(dto.UserId.Value);
            bool isNew = false;

            if (profile == null)
            {
                profile = new UserProfile { UserId = dto.UserId.Value };
                isNew = true;
            }

            profile.TemplateType = dto.TemplateType ?? profile.TemplateType;

            // Track existing sections to preserve IDs and identify which to delete
            var existingSections = profile.Sections.ToList();
            var visitedSectionIds = new HashSet<int>();

            // Build Sections and Entries
            // Standardizing SectionType: text, list, contact, address, note
            
            // 1. Full Name (note)
            if (!string.IsNullOrEmpty(dto.FullName))
            {
                string nameTitle = dto.TemplateType == "Child" ? "Child Name" : "Identity";
                AddSection(profile, nameTitle, SectionTypes.Note, new { fullName = dto.FullName }, existingSections, visitedSectionIds);
            }

            // 2. DOB/Age (text)
            if (!string.IsNullOrEmpty(dto.Dob))
                AddSection(profile, "Birth Info", SectionTypes.Text, new { dob = dto.Dob }, existingSections, visitedSectionIds);

            // 3. Medical Info (Blood Type, etc.)
            if (!string.IsNullOrEmpty(dto.BloodType))
                AddSection(profile, "Vital Info", SectionTypes.Text, new { bloodType = dto.BloodType }, existingSections, visitedSectionIds);

            if (dto.MedicalConditions?.Length > 0)
            {
                string conditionsTitle = dto.TemplateType == "Medical" ? "Chronic Diseases" : "Medical Conditions";
                AddSection(profile, conditionsTitle, SectionTypes.List, dto.MedicalConditions, existingSections, visitedSectionIds);
            }

            if (dto.Medications?.Length > 0)
                AddSection(profile, "Medications", SectionTypes.List, dto.Medications, existingSections, visitedSectionIds);

            if (dto.Allergies?.Length > 0)
                AddSection(profile, "Allergies", SectionTypes.List, dto.Allergies, existingSections, visitedSectionIds);

            // 4. Emergency Contacts (contact)
            if (dto.EmergencyContacts?.Count > 0)
            {
                string contactTitle = dto.TemplateType == "Child" ? "Parent Contacts" : "Emergency Contact";
                foreach (var contact in dto.EmergencyContacts)
                {
                    // For emergency contacts, we match by title and type, but there can be multiple.
                    // The AddSection logic handles finding the first unvisited one. If the user edits a contact 
                    // from the frontend via SaveProfileAsync, it might map wrong if they changed the order, 
                    // but for legacy onboarding compatibility this is acceptable. Granular PUTs are exact.
                    AddSection(profile, contactTitle, SectionTypes.Contact, contact, existingSections, visitedSectionIds);
                }
            }

            // 5. Notes & Address
            if (!string.IsNullOrEmpty(dto.Notes))
                AddSection(profile, "Additional Notes", SectionTypes.Note, dto.Notes, existingSections, visitedSectionIds);
            
            if (!string.IsNullOrEmpty(dto.Address))
                AddSection(profile, "Address", SectionTypes.Address, dto.Address, existingSections, visitedSectionIds);

            // 6. Custom Sections
            if (dto.CustomSections?.Count > 0)
            {
                foreach (var cs in dto.CustomSections)
                {
                    AddSection(profile, cs.Name, SectionTypes.List, cs.Items, existingSections, visitedSectionIds);
                }
            }

            // Remove sections that were not visited during this save (deleted by user)
            var sectionsToRemove = existingSections.Where(s => !visitedSectionIds.Contains(s.Id)).ToList();
            foreach (var s in sectionsToRemove)
            {
                profile.Sections.Remove(s);
            }

            if (isNew) await _profileRepository.AddAsync(profile);
            else await _profileRepository.UpdateAsync(profile);

            await _profileRepository.SaveChangesAsync();
            return await GetProfileByUserIdAsync(dto.UserId.Value);
        }

        private void AddSection(UserProfile profile, string title, string type, object data, List<ProfileSection> existingSections, HashSet<int> visitedSectionIds)
        {
            if (title.Contains("Emergency Contact", StringComparison.OrdinalIgnoreCase) || 
                title.Equals("Parent Contacts", StringComparison.OrdinalIgnoreCase))
            {
                type = SectionTypes.Contact;
            }

            // Safeguard: Skip adding sections with null or empty data (but allow empty collections for custom sections)
            bool isEmptyString = data is string s && string.IsNullOrWhiteSpace(s);
            if (data == null || isEmptyString) return;

            var json = JsonSerializer.Serialize(data);

            // Try to find an existing unvisited section with matching Title and Type (case-insensitive title)
            var existingSection = existingSections.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase) && s.SectionType == type && !visitedSectionIds.Contains(s.Id));

            if (existingSection != null)
            {
                // Update existing section entry
                var entry = existingSection.Entries.FirstOrDefault();
                if (entry != null)
                {
                    entry.DataJson = json;
                }
                else
                {
                    existingSection.Entries.Add(new ProfileEntry { DataJson = json });
                }
                visitedSectionIds.Add(existingSection.Id);
                existingSection.DisplayOrder = visitedSectionIds.Count - 1;
            }
            else
            {
                // Create new section
                var section = new ProfileSection
                {
                    Title = title,
                    SectionType = type,
                    DisplayOrder = visitedSectionIds.Count
                };
                section.Entries.Add(new ProfileEntry { DataJson = json });
                profile.Sections.Add(section);
                // We don't add to visitedSectionIds because it's new and doesn't have an ID yet, 
                // and it wasn't in existingSections.
            }
        }

        private ProfileDto MapToDto(UserProfile profile)
        {
            var dto = new ProfileDto
            {
                UserId = profile.UserId,
                TemplateType = profile.TemplateType,
                EmergencyContacts = new List<EmergencyContactDto>(),
                CustomSections = new List<CustomSectionDto>(),
                MedicalConditions = Array.Empty<string>(),
                Medications = Array.Empty<string>(),
                Allergies = Array.Empty<string>()
            };

            foreach (var section in profile.Sections.OrderBy(s => s.DisplayOrder))
            {
                var entry = section.Entries.FirstOrDefault();
                if (entry == null) continue;

                // Map stable ID for scalar/list types
                if (section.SectionType != SectionTypes.Contact && !dto.CustomSections.Any(c => c.Name == section.Title))
                {
                    dto.SectionIds[section.Title] = section.Id;
                }

                var json = entry.DataJson;
                var title = section.Title;

                // 1. Title-based mapping (Highest priority for stable scalar fields)
                if (title.Equals("Identity", StringComparison.OrdinalIgnoreCase) || title.Equals("Child Name", StringComparison.OrdinalIgnoreCase))
                {
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    dto.FullName = data.TryGetProperty("fullName", out var fn) ? fn.GetString() : null;
                    continue;
                }
                
                if (title.Equals("Address", StringComparison.OrdinalIgnoreCase))
                {
                    dto.Address = JsonSerializer.Deserialize<string>(json);
                    continue;
                }

                if (title.Equals("Additional Notes", StringComparison.OrdinalIgnoreCase))
                {
                    dto.Notes = JsonSerializer.Deserialize<string>(json);
                    continue;
                }

                if (title.Equals("Birth Info", StringComparison.OrdinalIgnoreCase))
                {
                    var textData = JsonSerializer.Deserialize<JsonElement>(json);
                    dto.Dob = textData.TryGetProperty("dob", out var d) ? d.GetString() : null;
                    continue;
                }

                if (title.Equals("Vital Info", StringComparison.OrdinalIgnoreCase))
                {
                    var textData = JsonSerializer.Deserialize<JsonElement>(json);
                    dto.BloodType = textData.TryGetProperty("bloodType", out var bt) ? bt.GetString() : null;
                    continue;
                }

                // 2. Type-based mapping (For lists and contacts)
                switch (section.SectionType)
                {
                    case SectionTypes.List:
                        var items = JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
                        if (title.Equals("Medical Conditions", StringComparison.OrdinalIgnoreCase) || title.Equals("Chronic Diseases", StringComparison.OrdinalIgnoreCase)) dto.MedicalConditions = items;
                        else if (title.Equals("Medications", StringComparison.OrdinalIgnoreCase)) dto.Medications = items;
                        else if (title.Equals("Allergies", StringComparison.OrdinalIgnoreCase)) dto.Allergies = items;
                        else 
                        {
                            dto.CustomSections.Add(new CustomSectionDto { Id = section.Id, Name = section.Title, Items = items.ToList() });
                            dto.SectionIds.Remove(section.Title); // Custom sections use Id field, not SectionIds map
                        }
                        break;

                    case SectionTypes.Contact:
                        try
                        {
                            var contact = JsonSerializer.Deserialize<EmergencyContactDto>(json);
                            if (contact != null) 
                            {
                                contact.Id = section.Id;
                                dto.EmergencyContacts.Add(contact);
                            }
                        }
                        catch
                        {
                            // Fallback for list-based custom contacts
                            try
                            {
                                var stringItems = JsonSerializer.Deserialize<string[]>(json);
                                if (stringItems != null)
                                    dto.CustomSections.Add(new CustomSectionDto { Name = section.Title, Items = stringItems.ToList() });
                            }
                            catch { /* Ignore */ }
                        }
                        break;
                }
            }

            return dto;
        }
        public async Task<ApiResponse<int>> AddSectionAsync(int userId, UpdateSectionDto dto)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null) return ApiResponse<int>.ErrorResponse("Profile not found");

            var section = new ProfileSection
            {
                Title = dto.Title,
                SectionType = dto.SectionType,
                DisplayOrder = profile.Sections.Any() ? profile.Sections.Max(s => s.DisplayOrder) + 1 : 0
            };
            section.Entries.Add(new ProfileEntry { DataJson = JsonSerializer.Serialize(dto.Data) });
            
            profile.Sections.Add(section);
            await _profileRepository.SaveChangesAsync();

            return ApiResponse<int>.SuccessResponse(section.Id, "Section added successfully");
        }

        public async Task<ApiResponse<bool>> UpdateSectionAsync(int userId, int sectionId, UpdateSectionDto dto)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null) return ApiResponse<bool>.ErrorResponse("Profile not found");

            var section = profile.Sections.FirstOrDefault(s => s.Id == sectionId);
            if (section == null) return ApiResponse<bool>.ErrorResponse("Section not found");

            section.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.SectionType)) section.SectionType = dto.SectionType;

            var entry = section.Entries.FirstOrDefault();
            var json = JsonSerializer.Serialize(dto.Data);

            if (entry != null)
            {
                entry.DataJson = json;
            }
            else
            {
                section.Entries.Add(new ProfileEntry { DataJson = json });
            }

            await _profileRepository.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResponse(true, "Section updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteSectionAsync(int userId, int sectionId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null) return ApiResponse<bool>.ErrorResponse("Profile not found");

            var section = profile.Sections.FirstOrDefault(s => s.Id == sectionId);
            if (section == null) return ApiResponse<bool>.ErrorResponse("Section not found");

            profile.Sections.Remove(section);
            
            // Normalize DisplayOrder to prevent gaps
            int order = 0;
            foreach (var s in profile.Sections.OrderBy(x => x.DisplayOrder))
            {
                s.DisplayOrder = order++;
            }

            await _profileRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Section deleted successfully");
        }

        public async Task<ApiResponse<bool>> ReorderSectionsAsync(int userId, System.Collections.Generic.List<int> orderedSectionIds)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null) return ApiResponse<bool>.ErrorResponse("Profile not found");

            // Validate that we aren't duplicating or skipping existing sections
            // Or reordering sections that don't belong to this profile
            var profileSectionIds = profile.Sections.Select(s => s.Id).ToList();
            
            var invalidIds = orderedSectionIds.Except(profileSectionIds).ToList();
            if (invalidIds.Any())
            {
                return ApiResponse<bool>.ErrorResponse($"Invalid section IDs provided: {string.Join(", ", invalidIds)}");
            }

            var duplicateIds = orderedSectionIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
            if (duplicateIds.Any())
            {
                return ApiResponse<bool>.ErrorResponse($"Duplicate section IDs in payload: {string.Join(", ", duplicateIds)}");
            }

            // Update DisplayOrder based on the index in the orderedSectionIds array
            for (int i = 0; i < orderedSectionIds.Count; i++)
            {
                var sectionId = orderedSectionIds[i];
                var section = profile.Sections.First(s => s.Id == sectionId);
                section.DisplayOrder = i;
            }

            // Any sections omitted from the payload get pushed to the end safely
            var omittedSections = profile.Sections.Where(s => !orderedSectionIds.Contains(s.Id)).OrderBy(s => s.DisplayOrder).ToList();
            int nextOrder = orderedSectionIds.Count;
            foreach (var omitted in omittedSections)
            {
                omitted.DisplayOrder = nextOrder++;
            }

            await _profileRepository.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResponse(true, "Sections reordered successfully");
        }
    }
}
