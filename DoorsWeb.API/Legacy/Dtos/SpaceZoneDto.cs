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
    // Scaffolded entities: TSpaceZoneHeader (ZoneNumber, Site, Name, ...), TSites.

    /// <summary>One space zone joined to its owning site.</summary>
    public class SpaceZoneDto
    {
        /// <summary>Space-zone code. Source: Zone/@Code (TSpaceZoneHeader.ZoneNumber).</summary>
        public int Code { get; set; }

        /// <summary>Space-zone name. Source: Zone/@Name (TSpaceZoneHeader.Name).</summary>
        public string? Name { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (TSites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (TSites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
