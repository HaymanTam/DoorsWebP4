using System.Reflection;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Backs the global custom-field definitions (T_CustomFieldTypes), their combobox option
    /// lists (T_Custom) and per-card values (T_Name_CustomFields). The definitions live in their
    /// own table independent of any cardholder, which is what lets a field added on one card
    /// persist into every other card record. Value columns are Custom1..Custom25, addressed
    /// positionally by the definition's slot.
    /// </summary>
    public class CustomFieldService : ICustomFieldService
    {
        // The 25 fixed value columns on T_Name_CustomFields, indexed by slot-1 (Custom1..Custom25).
        private static readonly PropertyInfo[] ValueColumns =
            Enumerable.Range(1, 25)
                .Select(n => typeof(CardholderCustomFields).GetProperty($"Custom{n}")!)
                .ToArray();

        private const int MaxSlots = 25;
        private const int ValueMaxLength = 30; // Custom{N} columns are varchar(30)

        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public CustomFieldService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<List<CustomFieldDefinitionDto>> GetDefinitions()
        {
            var defs = await _context.CustomFieldTypes
                .AsNoTracking()
                .OrderBy(t => t.CustomField)
                .Select(t => new CustomFieldDefinitionDto
                {
                    Slot = t.CustomField,
                    Name = t.Literal,
                    DataType = t.DataType ?? (int)CustomFieldDataType.Text
                })
                .ToListAsync();

            await AttachOptions(defs);
            return defs;
        }

        public async Task<List<CardCustomFieldDto>> GetForCard(int cardNumber)
        {
            var defs = await GetDefinitions();

            // The card may not have a value row yet (most don't until first save).
            var row = await _context.CardholderCustomFields.AsNoTracking()
                .FirstOrDefaultAsync(r => r.CardNumber == cardNumber);

            return defs.Select(d => new CardCustomFieldDto
            {
                Slot = d.Slot,
                Name = d.Name,
                DataType = d.DataType,
                Options = d.Options,
                Value = row is null ? null : GetColumn(row, d.Slot)
            }).ToList();
        }

        public async Task SaveForCard(int cardNumber, List<CardCustomFieldValueDto> values)
        {
            var row = await _context.CardholderCustomFields
                .FirstOrDefaultAsync(r => r.CardNumber == cardNumber);

            var isNew = row is null;
            if (isNew)
            {
                row = new CardholderCustomFields { CardNumber = cardNumber };
                _context.CardholderCustomFields.Add(row);
            }

            foreach (var v in values)
                SetColumn(row!, v.Slot, v.Value);

            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Cardholder Custom Fields",
                cardNumber.ToString(), $"Card {cardNumber}");
        }

        public async Task<CustomFieldOptionDto> AddOption(int slot, string description)
        {
            description = description.Trim();
            if (description.Length > ValueMaxLength)
                description = description[..ValueMaxLength];

            // Codes are per-field (composite key CustomFieldCode+Code); take the next free one.
            var maxCode = await _context.Custom
                .Where(c => c.CustomFieldCode == slot)
                .Select(c => (int?)c.Code)
                .MaxAsync() ?? 0;

            var option = new Custom
            {
                CustomFieldCode = slot,
                Code = maxCode + 1,
                Description = description
            };
            _context.Custom.Add(option);
            await _context.SaveChangesAsync();

            await _audit.LogAsync(AuditAction.Update, "Custom Field Option",
                $"{slot}/{option.Code}", description);

            return new CustomFieldOptionDto { Code = option.Code, Description = option.Description };
        }

        public async Task<bool> DeleteOption(int slot, int code)
        {
            var option = await _context.Custom
                .FirstOrDefaultAsync(c => c.CustomFieldCode == slot && c.Code == code);
            if (option is null) return false;

            _context.Custom.Remove(option);
            await _context.SaveChangesAsync();

            await _audit.LogAsync(AuditAction.Delete, "Custom Field Option",
                $"{slot}/{code}", option.Description);
            return true;
        }

        public async Task<CustomFieldDefinitionDto?> CreateDefinition(CustomFieldDefinitionDto dto)
        {
            var used = await _context.CustomFieldTypes.Select(t => t.CustomField).ToListAsync();
            var slot = Enumerable.Range(1, MaxSlots).FirstOrDefault(n => !used.Contains(n));
            if (slot == 0) return null; // all 25 slots are taken

            var entity = new CustomFieldTypes
            {
                CustomField = slot,
                Literal = Truncate(dto.Name, ValueMaxLength),
                DataType = dto.DataType
            };
            _context.CustomFieldTypes.Add(entity);
            await _context.SaveChangesAsync();

            await _audit.LogAsync(AuditAction.Create, "Custom Field", slot.ToString(), entity.Literal);

            return new CustomFieldDefinitionDto
            {
                Slot = entity.CustomField,
                Name = entity.Literal,
                DataType = entity.DataType ?? (int)CustomFieldDataType.Text
            };
        }

        public async Task<CustomFieldDefinitionDto?> UpdateDefinition(int slot, CustomFieldDefinitionDto dto)
        {
            var entity = await _context.CustomFieldTypes.FindAsync(slot);
            if (entity is null) return null;

            entity.Literal = Truncate(dto.Name, ValueMaxLength);
            entity.DataType = dto.DataType;
            await _context.SaveChangesAsync();

            await _audit.LogAsync(AuditAction.Update, "Custom Field", slot.ToString(), entity.Literal);

            var result = new CustomFieldDefinitionDto
            {
                Slot = entity.CustomField,
                Name = entity.Literal,
                DataType = entity.DataType ?? (int)CustomFieldDataType.Text
            };
            await AttachOptions(new List<CustomFieldDefinitionDto> { result });
            return result;
        }

        public async Task<bool> DeleteDefinition(int slot)
        {
            var entity = await _context.CustomFieldTypes.FindAsync(slot);
            if (entity is null) return false;

            // Drop the definition and its combobox options. Existing Custom{slot} values on
            // cardholder rows are left in place (harmless once the slot is unused) rather than
            // running a mass update across thousands of rows.
            var options = await _context.Custom.Where(c => c.CustomFieldCode == slot).ToListAsync();
            if (options.Count > 0) _context.Custom.RemoveRange(options);

            _context.CustomFieldTypes.Remove(entity);
            await _context.SaveChangesAsync();

            await _audit.LogAsync(AuditAction.Delete, "Custom Field", slot.ToString(), entity.Literal);
            return true;
        }

        // Loads T_Custom options for every combobox definition in the list, in a single query.
        private async Task AttachOptions(List<CustomFieldDefinitionDto> defs)
        {
            var comboSlots = defs
                .Where(d => d.DataType == (int)CustomFieldDataType.Combobox)
                .Select(d => d.Slot)
                .ToList();
            if (comboSlots.Count == 0) return;

            var options = await _context.Custom.AsNoTracking()
                .Where(c => comboSlots.Contains(c.CustomFieldCode))
                .OrderBy(c => c.Description)
                .Select(c => new { c.CustomFieldCode, c.Code, c.Description })
                .ToListAsync();

            foreach (var def in defs)
            {
                def.Options = options
                    .Where(o => o.CustomFieldCode == def.Slot)
                    .Select(o => new CustomFieldOptionDto { Code = o.Code, Description = o.Description })
                    .ToList();
            }
        }

        private static string? GetColumn(CardholderCustomFields row, int slot) =>
            slot is >= 1 and <= MaxSlots ? (string?)ValueColumns[slot - 1].GetValue(row) : null;

        private static void SetColumn(CardholderCustomFields row, int slot, string? value)
        {
            if (slot is >= 1 and <= MaxSlots)
                ValueColumns[slot - 1].SetValue(row, Truncate(value, ValueMaxLength));
        }

        private static string? Truncate(string? value, int maxLength) =>
            value is { Length: var len } && len > maxLength ? value[..maxLength] : value;
    }
}
