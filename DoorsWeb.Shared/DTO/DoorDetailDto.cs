using System;
using System.ComponentModel.DataAnnotations;

namespace DoorsWeb.Shared.DTO
{
    /// <summary>Full door record for the door editor, mapped to T_Doors columns.</summary>
    public class DoorDetailDto
    {
        //Header
        public int Door { get; set; }                       // T_Doors.Door (PK)
        [Required]
        public int ControllerId { get; set; }               // ControllerID (string in DB)
        [Required]
        public string Name { get; set; } = null!;           // Name
        [Required]
        [RegularExpression(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$",
            ErrorMessage = "Invalid IPv4 Address")]
        public string IPAddressString { get; set; } = null!; // DoorIPAddress

        //Controller Settings
        public bool AutoRelockEnable { get; set; } = false;  // AutoRelock
        public byte RelayA_Delay { get; set; } = 3;          // ReleaseDelay
        public byte RelayA_Time { get; set; } = 3;           // ReleaseTime
        public bool RelayA_TZOverideEnable { get; set; } = false; // TimeLock != 10000
        public int? RelayA_OpenTimeZone { get; set; }        // TimeLock (time-zone value)
        public byte RelayB_Mode { get; set; } = 0;           // RelayBmode
        public byte RelayB_Delay { get; set; } = 0;          // AutoDelayVal
        public byte RelayB_Time { get; set; } = 3;           // ReleaseTimeB
        public int? RelayB_TimeZone { get; set; }            // RelayBTimeZone
        public bool PDO_AlarmEnable { get; set; } = false;   // PDO > 0
        public byte PDO_Alarm_Time { get; set; } = 0;        // PDO
        public byte LockDriveMode { get; set; } = 0;         // Lock_Drive_Mode
        public TimeOnly ValidFrom { get; set; } = TimeOnly.MinValue; // ValidFromTimeHH/MM
        public TimeOnly ValidTo { get; set; } = TimeOnly.MinValue;   // ValidToTimeHH/MM
        public string? Notes { get; set; }                   // Notes
        public int RandomSearchFrequency { get; set; } = 0;  // Random_Search_Freq
        public byte Controller_FeedbackVolume { get; set; } = 15; // CON_FB_Volume
        public byte Controller_AlarmVolume { get; set; } = 15;    // CON_ALM_Volume

        //Keypad Settings
        public byte Keypad_VCardLength { get; set; } = 0;    // AccessCode_Len
        public int? Keypad_AccessCode { get; set; }          // AccessCode_Dig1..8 (composite)
        public bool Keypad_StarModeEnable { get; set; } = false; // Keypad_Star_Mode
        public string? Keypad_Name { get; set; } = "Keyboard";   // KeyboardName
        public byte Keypad_TechId { get; set; } = 0;             // KeyboardTech
        public int? MFA_OverrideTimeZone { get; set; }          // CardandPINTimeZone

        //Reader A Settings
        [Required]
        public string ReaderA_Name { get; set; } = "In";     // ReaderAName
        public byte ReaderA_TechId { get; set; } = 0;        // TechnologyA
        public byte ReaderA_Volume { get; set; } = 15;       // RDR_Volume_A
        public byte ReaderA_Brightness { get; set; } = 5;    // RDR_Brightness_A
        public byte ReaderA_MFA_Sequence { get; set; } = 0;  // ID_Sequence_A

        //Reader B Settings
        [Required]
        public string ReaderB_Name { get; set; } = "Out";    // ReaderBName
        public byte ReaderB_TechId { get; set; } = 0;        // TechnologyB
        public byte ReaderB_Volume { get; set; } = 15;       // RDR_Volume_B
        public byte ReaderB_Brightness { get; set; } = 5;    // RDR_Brightness_B
        public byte ReaderB_MFA_Sequence { get; set; } = 0;  // ID_Sequence_B

        public DateTime LastUpdated { get; set; } = DateTime.Now; // Updated
    }
}
