namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors the data surfaced by frmMain.LoadSites (LegacyDoorsClient\frmMain.frm).
    // The site list/tree is built from the server XML, whose site nodes are:
    //
    //   <XMLData><Sites>
    //     <Site Code="..." Name="..."> ... </Site>
    //   </Sites></XMLData>
    //
    // Scaffolded entity: TSites (Site, Name, Key, Inuse, Status).

    /// <summary>One site (the top level of the legacy system tree).</summary>
    public class SiteDto
    {
        /// <summary>Site code. Source: Site/@Code (TSites.Site).</summary>
        public int Code { get; set; }

        /// <summary>Site name. Source: Site/@Name (TSites.Name).</summary>
        public string? Name { get; set; }
    }
}
