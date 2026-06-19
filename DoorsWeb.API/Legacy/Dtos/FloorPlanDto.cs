namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadFloorPlans (LegacyDoorsClient\frmMain.frm). Floor plans are
    // grouped per site in the server XML:
    //
    //   <Site Code="..." Name="...">
    //     <FloorPlans>
    //       <FloorPlan Code="..." Name="..." />
    //     </FloorPlans>
    //   </Site>
    //
    // Scaffolded entities: FloorPlans (Code, Site, Name), Sites.

    /// <summary>One floor plan joined to its owning site.</summary>
    public class FloorPlanDto
    {
        /// <summary>Floor-plan code. Source: FloorPlan/@Code (FloorPlans.Code).</summary>
        public int Code { get; set; }

        /// <summary>Floor-plan name. Source: FloorPlan/@Name (FloorPlans.Name).</summary>
        public string? Name { get; set; }

        /// <summary>Owning site code. Source: Site/@Code (Sites.Site).</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: Site/@Name (Sites.Name).</summary>
        public string? SiteName { get; set; }
    }
}
