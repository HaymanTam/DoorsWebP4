namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// A door control command that has been sent to a controller but not yet acknowledged.
    /// The API keeps these in memory and resends them until the controller acks (or an operator
    /// clears them); the list is pushed to clients over the EventHub as "PendingCommandsChanged"
    /// so the Door Manager can show what is still outstanding.
    /// </summary>
    public class PendingCommandDto
    {
        /// <summary>Stable identifier for this pending command (used to clear a single entry).</summary>
        public Guid Id { get; set; }

        /// <summary>The door this command targets.</summary>
        public int Door { get; set; }

        /// <summary>The door's display name (resolved when the command was enqueued).</summary>
        public string? DoorName { get; set; }

        /// <summary>The action being attempted.</summary>
        public DoorCommandAction Action { get; set; }

        /// <summary>Which relay the command drives.</summary>
        public DoorRelay Relay { get; set; }

        /// <summary>Short human-readable summary of the command (e.g. "Unlock Relay A").</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>How many times the command has been transmitted so far.</summary>
        public int Attempts { get; set; }

        /// <summary>When the command was first enqueued (UTC).</summary>
        public DateTime CreatedUtc { get; set; }

        /// <summary>When the command was last (re)transmitted (UTC), or null before the first send.</summary>
        public DateTime? LastAttemptUtc { get; set; }
    }
}
