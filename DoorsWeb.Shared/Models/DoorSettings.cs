using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoorsWeb.Shared.Models
{
    public class DoorSettings
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("DoorHeader")]
        public required DoorHeader Header { get; set; }

        //Controller Settings
        public bool AutoRelockEnable { get; set; }
        public byte RelayA_Delay { get; set; }
        public byte RelayA_Time { get; set; }
        public bool RelayA_TZOverideEnable { get; set; }
        public byte RelayB_Mode { get; set; }
        public byte RelayB_Delay { get; set; }
        public byte RelayB_Time { get; set; }
        public byte PDO_Alarm_Time { get; set; }
        public byte LockDriveMode { get; set; }
        public TimeOnly ValidFrom { get; set; }
        public TimeOnly ValidTo { get; set; }
        public string? Notes { get; set; }

        //Keypad Settings
        public byte Keypad_VCardLength { get; set; }
        public int? Keypad_AccessCode { get; set; }
        public bool Keypad_StarModeEnable { get; set; }

        //Reader A Settings
        public required string ReaderA_Name { get; set; }
        public byte ReaderA_TechId { get; set; }
        public byte ReaderA_Volume { get; set; }
        public byte ReaderA_Brightness { get; set; }
        public byte ReaderA_MFA_Sequence { get; set; }
        //Reader B Settings
        public required string ReaderB_Name { get; set; }
        public byte ReaderB_TechId { get; set; }
        public byte ReaderB_Volume { get; set; }
        public byte ReaderB_Brightness { get; set; }
        public byte ReaderB_MFA_Sequence { get; set; }
        public DateTime LastUpdatedAt { get; set; }

    }
}
