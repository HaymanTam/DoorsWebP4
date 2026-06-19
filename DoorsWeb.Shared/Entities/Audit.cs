using System;

namespace DoorsWeb.Shared.Entities;

/// <summary>
/// A single change-audit record. One row is written every time a user creates, updates or
/// deletes a record through the web app (cardholders, access levels, time zones, calendars, etc.).
/// Replaces the legacy card-only audit table.
/// </summary>
public partial class Audit
{
    public int Id { get; set; }

    /// <summary>When the change happened (UTC).</summary>
    public DateTime Timestamp { get; set; }

    /// <summary>Login name of the user who made the change.</summary>
    public string UserName { get; set; } = null!;

    /// <summary>IP address the change request came from (IPv4 or IPv6); null if it could not be resolved.</summary>
    public string? ClientIp { get; set; }

    /// <summary>"Create", "Update" or "Delete".</summary>
    public string Action { get; set; } = null!;

    /// <summary>Kind of record changed, e.g. "Cardholder", "Access Level", "Time Zone".</summary>
    public string EntityType { get; set; } = null!;

    /// <summary>Key of the changed record: the card number for cardholders, otherwise the record's id/code.</summary>
    public string? EntityKey { get; set; }

    /// <summary>Human-readable name/description of the changed record (e.g. cardholder name, zone name).</summary>
    public string? EntityName { get; set; }
}
