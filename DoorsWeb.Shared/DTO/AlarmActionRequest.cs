namespace DoorsWeb.Shared.DTO
{
    /// <summary>Body of a "mark this alarm as actioned" request (the operator's optional note).</summary>
    public class AlarmActionRequest
    {
        /// <summary>Free-text note recorded against the alarm (shown as "Details" on the Actioned tab).</summary>
        public string? Note { get; set; }
    }
}
