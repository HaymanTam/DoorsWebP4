using System;

namespace DoorsWeb.Shared.DTO
{
    /// <summary>A door row for the /doors list, projected from T_Doors.</summary>
    public class DoorListDto
    {
        public int Door { get; set; }                        // T_Doors.Door (PK / door number)
        public int ControllerId { get; set; }                // T_Doors.ControllerID (parsed from string)

        /// <summary>
        /// String form of <see cref="ControllerId"/> for the doors table. The table's search only
        /// matches string/Guid/enum columns, so the Controller ID column binds to this to be searchable.
        /// </summary>
        public string ControllerIdString => ControllerId.ToString();

        public string Name { get; set; } = null!;            // T_Doors.Name
        public string IPAddressString { get; set; } = null!; // T_Doors.DoorIPAddress
        public int? Site { get; set; }                       // T_Doors.Site (FK to T_Sites; used to filter by site)
        public DateTime LastUpdated { get; set; } = DateTime.Now; // T_Doors.Updated
    }
}
