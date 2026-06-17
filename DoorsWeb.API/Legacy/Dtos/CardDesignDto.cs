namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadCardDesigns (LegacyDoorsClient\frmMain.frm). Card designs
    // are a flat (non-site) list in the server XML:
    //
    //   <XMLData><Records>
    //     <Record Code="..." Description="..." />
    //   </Records></XMLData>
    //
    // The legacy client treats designs with Code < 3 as the protected/built-in
    // designs (shown with a locked icon).
    //
    // Scaffolded entity: TCardDesignHeader (Code, Description, Orientation).

    /// <summary>One card design.</summary>
    public class CardDesignDto
    {
        /// <summary>Card-design code. Source: Record/@Code (TCardDesignHeader.Code).</summary>
        public int Code { get; set; }

        /// <summary>Card-design description. Source: Record/@Description (TCardDesignHeader.Description).</summary>
        public string? Description { get; set; }

        /// <summary>True for the built-in, protected designs (legacy rule: Code &lt; 3).</summary>
        public bool IsProtected { get; set; }
    }
}
