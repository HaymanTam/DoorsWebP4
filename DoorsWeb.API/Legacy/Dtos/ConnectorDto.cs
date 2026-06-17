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
    // Scaffolded entities: TConnectors (Connector, Site, Name, ...), TSites.

    /// <summary>One connector joined to its owning site.</summary>
    public class ConnectorDto
    {
        /// <summary>Connector code. Source: Connector/@Code (TConnectors.Connector).</summary>
        public int Code { get; set; }

        /// <summary>Connector name. Source: Connector/@Name (TConnectors.Name).</summary>
        public string? Name { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (TSites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (TSites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
