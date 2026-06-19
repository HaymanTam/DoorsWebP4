namespace DoorsWeb.API.Legacy.Dtos
{
    // Mirrors frmMain.LoadCardPacks (LegacyDoorsClient\frmMain.frm). Card packs are a
    // flat (non-site) list in the server XML:
    //
    //   <XMLData><Records>
    //     <Record Code="..." Name="..." FirstCardID="..." LastCardID="..." Qty="..." />
    //   </Records></XMLData>
    //
    // The client displays FirstCardID/LastCardID zero-padded to 8 digits. LastCardID
    // is server-computed (there is no such column); FirstCardID and Qty come from
    // CardPack.
    //
    // Scaffolded entity: CardPack (Code, Name, FirstCardId, Qty).

    /// <summary>One card pack.</summary>
    public class CardPackDto
    {
        /// <summary>Card-pack code. Source: Record/@Code (CardPack.Code).</summary>
        public int Code { get; set; }

        /// <summary>Card-pack name. Source: Record/@Name (CardPack.Name).</summary>
        public string? Name { get; set; }

        /// <summary>First card ID in the pack. Source: Record/@FirstCardID (CardPack.FirstCardId).</summary>
        public string? FirstCardId { get; set; }

        /// <summary>Last card ID in the pack. Source: Record/@LastCardID (server-computed from FirstCardId + Qty).</summary>
        public string? LastCardId { get; set; }

        /// <summary>Number of cards in the pack. Source: Record/@Qty (CardPack.Qty).</summary>
        public int? Qty { get; set; }
    }
}
