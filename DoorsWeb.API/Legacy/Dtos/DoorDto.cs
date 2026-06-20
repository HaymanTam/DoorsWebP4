namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadDoors -> FillDoorStatus (LegacyDoorsClient\frmMain.frm).
    // Doors are grouped per site:
    //
    //   <Site Code="..." Name="...">
    //     <Doors>
    //       <Door Code="..." Door1="..." Door2="..." ... />
    //     </Doors>
    //   </Site>
    //
    // The door status grid columns are property-driven: each "Door{PropertyId}"
    // attribute is matched against the configurable display properties
    // (T_Display / T_DisplayTypes), so the visible columns vary by configuration.
    // This DTO carries only the stable identity fields; the dynamic status values
    // are intentionally not modelled here.
    //
    // Scaffolded entities: Doors (Door, Name, Site, ...), Sites.

    /// <summary>One door's core identity, joined to its owning site.</summary>
    public class DoorDto
    {
        /// <summary>Door code. Source: Door/@Code (Doors.Door).</summary>
        public int Code { get; set; }

        /// <summary>Door name. Source: Doors.Name (shown as the first status column).</summary>
        public string? Name { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (Sites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (Sites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
