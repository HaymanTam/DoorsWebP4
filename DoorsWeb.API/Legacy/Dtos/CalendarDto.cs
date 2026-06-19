namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadCalendars (LegacyDoorsClient\frmMain.frm). Calendars are
    // grouped per site in the server XML:
    //
    //   <Site Code="..." Name="...">
    //     <Calendars>
    //       <Calendar Code="..." Name="..." />
    //     </Calendars>
    //   </Site>
    //
    // Scaffolded entities: Calendar (Code, Site, Description, ...), Sites.

    /// <summary>One calendar joined to its owning site.</summary>
    public class CalendarDto
    {
        /// <summary>Calendar code. Source: Calendar/@Code (Calendar.Code).</summary>
        public int Code { get; set; }

        /// <summary>Calendar name. Source: Calendar/@Name (Calendar.Description).</summary>
        public string? Name { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (Sites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (Sites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
