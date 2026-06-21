using System.Collections.Generic;

namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// The optional spatial layout for one site's floorplan: a background image plus the
    /// placed door markers. Persisted as a JSON file on the API settings volume (one per site).
    /// When no image/placements exist the view falls back to a zero-setup zone board, so this
    /// is purely the "pretty uploaded plan" overlay.
    /// </summary>
    public class FloorPlanLayoutDto
    {
        /// <summary>Site this layout belongs to (T_Sites.Site).</summary>
        public int Site { get; set; }

        /// <summary>
        /// File name of the uploaded background image, served from /media/floorplan/{ImageFileName}.
        /// Null when no plan image has been uploaded (view uses the zone board instead).
        /// </summary>
        public string? ImageFileName { get; set; }

        /// <summary>Doors placed on the image. Doors not listed here are still shown on the zone board.</summary>
        public List<DoorPlacementDto> Doors { get; set; } = new();
    }

    /// <summary>One door pinned to a position on the floorplan image, in resolution-independent percentages.</summary>
    public class DoorPlacementDto
    {
        /// <summary>Door number (T_Doors.Door).</summary>
        public int Door { get; set; }

        /// <summary>Horizontal position as a percentage (0..100) of the image width.</summary>
        public double X { get; set; }

        /// <summary>Vertical position as a percentage (0..100) of the image height.</summary>
        public double Y { get; set; }
    }
}
