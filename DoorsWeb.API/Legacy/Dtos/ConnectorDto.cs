namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadConnectors (LegacyDoorsClient\frmMain.frm). The list is
    // built per site from the server XML:
    //
    //   <Site Code="..." Name="...">
    //     <Connectors>
    //       <Connector Code="..." Name="..." />
    //     </Connectors>
    //   </Site>
    //
    // Scaffolded entities: Connectors (Connector, Site, Name, ...), Sites.

    /// <summary>One connector joined to its owning site.</summary>
    public class ConnectorDto
    {
        /// <summary>Connector code. Source: Connector/@Code (Connectors.Connector).</summary>
        public int Code { get; set; }

        /// <summary>Connector name. Source: Connector/@Name (Connectors.Name).</summary>
        public string? Name { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (Sites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (Sites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
