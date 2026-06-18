using System;

namespace DoorsWeb.Shared.DTO
{
    /// <summary>A door row for the /doors list, projected from T_Doors.</summary>
    public class DoorListDto
    {
        public int Door { get; set; }                        // T_Doors.Door (PK / door number)
        public int ControllerId { get; set; }                // T_Doors.ControllerID (parsed from string)
        public string Name { get; set; } = null!;            // T_Doors.Name
        public string IPAddressString { get; set; } = null!; // T_Doors.DoorIPAddress
        public DateTime LastUpdated { get; set; } = DateTime.Now; // T_Doors.Updated
    }
}
