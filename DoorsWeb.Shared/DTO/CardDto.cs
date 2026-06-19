using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.DTO
{
    /// <summary>A cardholder row for the Card Manager grid, projected from T_Name_Header
    /// together with the names of the access levels the card is tied to (via T_Name_AccessLevels).</summary>
    public class CardDto
    {
        public int CardNumber { get; set; }                 // T_Name_Header.CardNumber (PK / system id, used for edit & delete)
        public string? CardId { get; set; }                 // printed card number, shown as "Card Number"
        public string? Surname { get; set; }                // Last name
        public string? Forname { get; set; }                // First name
        public bool? Enabled { get; set; }                  // card enabled flag
        public bool? Void { get; set; }                     // card voided flag
        public DateTime? ValidFrom { get; set; }            // start of card validity
        public DateTime? ValidTo { get; set; }              // end of card validity

        // Names of the access levels this card is tied to (joined through T_Name_AccessLevels).
        public List<string> AccessLevels { get; set; } = new();
    }
}
