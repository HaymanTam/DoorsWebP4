using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace DoorsWeb.Shared.DTO
{
    public class DoorDetailDto
    {
        //Header
        public Guid Id { get; set; }
        [Required]
        public int ControllerId { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [RegularExpression(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$",
    ErrorMessage = "Invalid IPv4 Address")]
        public string IPAddressString { get; set; } = null!;
        //Controller Settings
        public bool AutoRelockEnable { get; set; } = false;
        public byte RelayA_Delay { get; set; } = 3;
        public byte RelayA_Time { get; set; } = 3;
        public bool RelayA_TZOverideEnable { get; set; } = false;
        public byte RelayB_Mode { get; set; } = 0;
        public byte RelayB_Delay { get; set; } = 0;
        public byte RelayB_Time { get; set; } = 3;
        public byte PDO_Alarm_Time { get; set; } = 0;
        public byte LockDriveMode { get; set; } = 0;
        public TimeOnly ValidFrom { get; set; } = TimeOnly.MinValue; 
        public TimeOnly ValidTo { get; set; } = TimeOnly.MinValue;
        public string? Notes { get; set; }

        //Keypad Settings
        public byte Keypad_VCardLength { get; set; } = 0;
        public int? Keypad_AccessCode { get; set; }
        public bool Keypad_StarModeEnable { get; set; } = false;

        //Reader A Settings
        [Required]
        public string ReaderA_Name { get; set; } = "In";
        public byte ReaderA_TechId { get; set; } = 0;
        public byte ReaderA_Volume { get; set; } = 15;
        public byte ReaderA_Brightness { get; set; } = 5;
        public byte ReaderA_MFA_Sequence { get; set; } = 0;

        //Reader B Settings
        [Required]
        public string ReaderB_Name { get; set; } = "Out";
        public byte ReaderB_TechId { get; set; } = 0;
        public byte ReaderB_Volume { get; set; } = 15;
        public byte ReaderB_Brightness { get; set; } = 5;
        public byte ReaderB_MFA_Sequence { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
