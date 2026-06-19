namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadSpaceZones (LegacyDoorsClient\frmMain.frm). Space zones are
    // grouped per site in the server XML:
    //
    //   <Site Code="..." Name="...">
    //     <Zones>
    //       <Zone Code="..." Name="..." />
    //     </Zones>
    //   </Site>
    //
    // Scaffolded entities: SpaceZone (ZoneNumber, Site, Name, ...), Sites.

    /// <summary>One space zone joined to its owning site.</summary>
    public class SpaceZoneDto
    {
        /// <summary>Space-zone code. Source: Zone/@Code (SpaceZone.ZoneNumber).</summary>
        public int Code { get; set; }

        /// <summary>Space-zone name. Source: Zone/@Name (SpaceZone.Name).</summary>
        public string? Name { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (Sites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (Sites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
