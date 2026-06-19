namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadAPBZones (LegacyDoorsClient\frmMain.frm). Anti-passback
    // zones are grouped per site in the server XML:
    //
    //   <Site Code="..." Name="...">
    //     <Zones>
    //       <Zone Code="..." Name="..." />
    //     </Zones>
    //   </Site>
    //
    // Scaffolded entities: ApbZone (Apbnumber, Site, Name, ...), Sites.

    /// <summary>One anti-passback zone joined to its owning site.</summary>
    public class ApbZoneDto
    {
        /// <summary>APB-zone code. Source: Zone/@Code (ApbZone.Apbnumber).</summary>
        public int Code { get; set; }

        /// <summary>APB-zone name. Source: Zone/@Name (ApbZone.Name).</summary>
        public string? Name { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (Sites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (Sites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
