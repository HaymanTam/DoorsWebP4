namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadTimeSheets (LegacyDoorsClient\frmMain.frm). Time sheets are
    // a flat (non-site) list in the server XML:
    //
    //   <XMLData><Records>
    //     <Record Code="..." Name="..." />
    //   </Records></XMLData>
    //
    // Scaffolded entity: TTimeSheetHeader (Code, Name, ...).

    /// <summary>One time-sheet definition.</summary>
    public class TimeSheetDto
    {
        /// <summary>Time-sheet code. Source: Record/@Code (TTimeSheetHeader.Code).</summary>
        public int Code { get; set; }

        /// <summary>Time-sheet name. Source: Record/@Name (TTimeSheetHeader.Name).</summary>
        public string? Name { get; set; }
    }
}
