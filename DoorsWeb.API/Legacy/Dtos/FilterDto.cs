namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadFilters (LegacyDoorsClient\frmMain.frm), which populates the
    // Card Manager "Filters" tree from the server XML:
    //
    //   <XMLData><Filters>
    //     <Filter Code="..." Description="..." ViewType="..." />
    //   </Filters></XMLData>
    //
    // ViewType matches the legacy FilterTypeConstants enum (mdlMain.bas):
    //   0 = All, 1 = Standard (built-in, protected; description is translated),
    //   2 = My, 3 = Public.
    //
    // Scaffolded entity: CardManager (Code, Description, ViewType, ...).

    /// <summary>One Card Manager filter (saved view).</summary>
    public class FilterDto
    {
        /// <summary>Filter code. Source: Filter/@Code (CardManager.Code).</summary>
        public int Code { get; set; }

        /// <summary>Filter description. Source: Filter/@Description (CardManager.Description).</summary>
        public string? Description { get; set; }

        /// <summary>
        /// View type. Source: Filter/@ViewType (CardManager.ViewType).
        /// 1 = Standard (built-in), 2 = My, 3 = Public.
        /// </summary>
        public int ViewType { get; set; }
    }
}
