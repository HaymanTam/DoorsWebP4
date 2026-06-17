namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadDoors -> FillDoorStatus (LegacyDoorsClient\frmMain.frm).
    // Doors are grouped per site (and optionally filtered by connector):
    //
    //   <Site Code="..." Name="...">
    //     <Doors>
    //       <Door Code="..." Connector="..." Door1="..." Door2="..." ... />
    //     </Doors>
    //   </Site>
    //
    // The door status grid columns are property-driven: each "Door{PropertyId}"
    // attribute is matched against the configurable display properties
    // (T_Display / T_DisplayTypes), so the visible columns vary by configuration.
    // This DTO carries only the stable identity fields; the dynamic status values
    // are intentionally not modelled here.
    //
    // Scaffolded entities: TDoors (Door, Name, Connector, Site, ...), TSites.

    /// <summary>One door's core identity, joined to its owning site.</summary>
    public class DoorDto
    {
        /// <summary>Door code. Source: Door/@Code (TDoors.Door).</summary>
        public int Code { get; set; }

        /// <summary>Door name. Source: TDoors.Name (shown as the first status column).</summary>
        public string? Name { get; set; }

        /// <summary>Owning connector code. Source: Door/@Connector (TDoors.Connector).</summary>
        public int? Connector { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (TSites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (TSites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
