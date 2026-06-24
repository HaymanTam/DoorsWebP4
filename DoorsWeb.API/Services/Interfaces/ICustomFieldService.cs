using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>
    /// Global custom-field definitions (T_CustomFieldTypes), their combobox option lists
    /// (T_Custom) and per-card values (T_Name_CustomFields). Definitions are shared by every
    /// card record, which is what makes a custom field "persistent" across the card modal.
    /// </summary>
    public interface ICustomFieldService
    {
        /// <summary>All global field definitions, ordered by slot, each with its combobox options.</summary>
        Task<List<CustomFieldDefinitionDto>> GetDefinitions();

        /// <summary>The global definitions merged with one card's stored values, ready for editing.</summary>
        Task<List<CardCustomFieldDto>> GetForCard(int cardNumber);

        /// <summary>Upserts the card's value row, mapping each slot to its Custom{slot} column.</summary>
        Task SaveForCard(int cardNumber, List<CardCustomFieldValueDto> values);

        /// <summary>Appends a new option to a combobox field's list (T_Custom) and returns it.</summary>
        Task<CustomFieldOptionDto> AddOption(int slot, string description);

        /// <summary>Removes a single combobox option (T_Custom). False if it doesn't exist.</summary>
        Task<bool> DeleteOption(int slot, int code);

        /// <summary>Creates a new field definition in the next free slot (1..25). Null if all slots are used.</summary>
        Task<CustomFieldDefinitionDto?> CreateDefinition(CustomFieldDefinitionDto dto);

        /// <summary>Renames/re-types an existing field definition. Null if the slot doesn't exist.</summary>
        Task<CustomFieldDefinitionDto?> UpdateDefinition(int slot, CustomFieldDefinitionDto dto);

        /// <summary>Removes a field definition (and any combobox options). False if it doesn't exist.</summary>
        Task<bool> DeleteDefinition(int slot);
    }
}
