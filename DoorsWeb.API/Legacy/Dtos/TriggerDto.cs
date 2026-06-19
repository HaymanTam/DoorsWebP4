namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadTriggers (LegacyDoorsClient\frmMain.frm). Triggers are
    // grouped per site in the server XML and filtered by trigger type:
    //
    //   <Site Code="..." Name="...">
    //     <Triggers>
    //       <Trigger Code="..." Name="..." TriggerType="..." />
    //     </Triggers>
    //   </Site>
    //
    // TriggerType matches the legacy TriggerTypeConstants enum (mdlMain.bas):
    //   0 = Unknown, 1 = Door, 2 = SIM (obsolete), 3 = SpaceZone.
    //
    // Scaffolded entities: Trigger (Code, Site, Name, TriggerType, ...), Sites.

    /// <summary>One trigger joined to its owning site.</summary>
    public class TriggerDto
    {
        /// <summary>Trigger code. Source: Trigger/@Code (Trigger.Code).</summary>
        public int Code { get; set; }

        /// <summary>Trigger name. Source: Trigger/@Name (Trigger.Name).</summary>
        public string? Name { get; set; }

        /// <summary>
        /// Trigger type. Source: Trigger/@TriggerType (Trigger.TriggerType).
        /// 1 = Door, 3 = SpaceZone.
        /// </summary>
        public int TriggerType { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (Sites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (Sites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
