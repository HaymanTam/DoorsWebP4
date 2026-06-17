namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadTimeZones (LegacyDoorsClient\frmMain.frm). Time zones are
    // grouped per site in the server XML:
    //
    //   <Site Code="..." Name="...">
    //     <Zones>
    //       <Zone Code="..." Name="..." />
    //     </Zones>
    //   </Site>
    //
    // Scaffolded entities: TTimeZoneHeader (TimeZone, Site, Name, ...), TSites.

    /// <summary>One time zone joined to its owning site.</summary>
    public class TimeZoneDto
    {
        /// <summary>Time-zone code. Source: Zone/@Code (TTimeZoneHeader.TimeZone).</summary>
        public int Code { get; set; }

        /// <summary>Time-zone name. Source: Zone/@Name (TTimeZoneHeader.Name).</summary>
        public string? Name { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (TSites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (TSites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
