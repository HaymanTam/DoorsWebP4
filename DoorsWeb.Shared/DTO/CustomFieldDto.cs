using System.Collections.Generic;

namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// Data type of a custom field, mirroring the legacy CustomFieldTypeConstants
    /// (T_CustomFieldTypes.DataType). The numeric values are the ones already stored
    /// in the live database, so they must not be renumbered.
    /// </summary>
    public enum CustomFieldDataType
    {
        Checkbox = 1,
        Combobox = 2,          // "Drop down list" — value stored is the option Code (see T_Custom)
        Date = 3,
        Text = 4,
        Number = 5,
        CheckboxAndDate = 6,
        CheckboxAndNumber = 7
    }

    /// <summary>
    /// A global custom-field definition (one row of T_CustomFieldTypes): which slot it
    /// occupies (1..25), its label and data type. These definitions are shared across every
    /// card record, so they persist independently of any single cardholder. For combobox
    /// fields, <see cref="Options"/> carries the selectable values (from T_Custom).
    /// </summary>
    public class CustomFieldDefinitionDto
    {
        /// <summary>T_CustomFieldTypes.CustomField — the 1..25 slot, also the suffix of the
        /// matching T_Name_CustomFields.Custom{Slot} value column.</summary>
        public int Slot { get; set; }

        /// <summary>T_CustomFieldTypes.Literal — the field's display label (e.g. "Department").</summary>
        public string? Name { get; set; }

        /// <summary>T_CustomFieldTypes.DataType.</summary>
        public int DataType { get; set; }

        /// <summary>Selectable options for a combobox field (empty for other types).</summary>
        public List<CustomFieldOptionDto> Options { get; set; } = new();
    }

    /// <summary>A selectable combobox option (one row of T_Custom): the persisted
    /// <see cref="Code"/> and its display <see cref="Description"/>.</summary>
    public class CustomFieldOptionDto
    {
        public int Code { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// One custom field as it appears on a card record: the global definition plus this card's
    /// stored value. For a combobox the <see cref="Value"/> is the option <c>Code</c> (as text);
    /// for text/number it's the raw value; for a checkbox it's "1"/"0"; for a date it's "yyyy-MM-dd".
    /// </summary>
    public class CardCustomFieldDto
    {
        public int Slot { get; set; }
        public string? Name { get; set; }
        public int DataType { get; set; }
        public string? Value { get; set; }
        public List<CustomFieldOptionDto> Options { get; set; } = new();
    }

    /// <summary>A slot/value pair sent back to persist a single card's custom-field value.</summary>
    public class CardCustomFieldValueDto
    {
        public int Slot { get; set; }
        public string? Value { get; set; }
    }
}
