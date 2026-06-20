using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

// -----------------------------------------------------------------------------
// Navigation properties for the scaffolded legacy entities.
//
// The legacy DoorsEnterprise SQL Server database has NO foreign-key constraints,
// so the database-first scaffold produced flat POCOs with no navigations. These
// partial classes restore the relationships in EF code only (no DB changes),
// reconstructed from the stored-procedure JOINs used by the old VB6 server.
//
// All Fluent configuration (HasOne/WithMany/HasForeignKey/OnDelete) lives in
// DoorsWeb.API/Legacy/DoorsEnterpriseContext.Relationships.cs. Every navigation
// here is explicitly configured there, because the legacy column names do not
// follow EF conventions (e.g. "Site" not "SiteId") and several entity pairs have
// more than one relationship between them.
//
// Naming convention:
//   * Reference navigations append "Navigation" to the FK column name when a
//     scalar property of that name already exists (e.g. SiteNavigation for the
//     "Site" FK column). Header/detail and other unambiguous references use a
//     descriptive principal-type name (e.g. Calendar, Cardholder).
//   * Collection navigations are plural and initialised to new List<>().
// -----------------------------------------------------------------------------

public partial class Sites
{
    public ICollection<Doors> Doors { get; set; } = new List<Doors>();
    public ICollection<AccessLevels> AccessLevels { get; set; } = new List<AccessLevels>();
    public ICollection<ApbZone> ApbZones { get; set; } = new List<ApbZone>();
    public ICollection<SpaceZone> SpaceZones { get; set; } = new List<SpaceZone>();
    public ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();
    public ICollection<TimeZones> TimeZones { get; set; } = new List<TimeZones>();
    public ICollection<Alarms> Alarms { get; set; } = new List<Alarms>();
    public ICollection<FloorPlans> FloorPlans { get; set; } = new List<FloorPlans>();
    public ICollection<Trigger> Triggers { get; set; } = new List<Trigger>();
    public ICollection<CardPackSite> CardPackSites { get; set; } = new List<CardPackSite>();
    public ICollection<CardholderAccessLevel> CardholderAccessLevels { get; set; } = new List<CardholderAccessLevel>();
    public ICollection<SpaceZoneDoor> SpaceZoneDoors { get; set; } = new List<SpaceZoneDoor>();
    public ICollection<UserSites> UserSites { get; set; } = new List<UserSites>();
}

public partial class Doors
{
    public Sites? SiteNavigation { get; set; }
    public DoorTechnology? TechnologyANavigation { get; set; }
    public DoorTechnology? TechnologyBNavigation { get; set; }
    public DoorTechnology? KeyboardTechNavigation { get; set; }
    public FloorPlans? FloorPlanNavigation { get; set; }

    public ICollection<AccessLevelDoor> AccessLevelDoors { get; set; } = new List<AccessLevelDoor>();
    public ICollection<ApbZoneDoor> ApbZoneDoors { get; set; } = new List<ApbZoneDoor>();
    public ICollection<Events> Events { get; set; } = new List<Events>();
    public ICollection<SpaceZoneDoor> SpaceZoneDoors { get; set; } = new List<SpaceZoneDoor>();
}

public partial class IoController
{
    public ICollection<IoControllerInput> IoControllerInputs { get; set; } = new List<IoControllerInput>();
}

public partial class IoControllerInput
{
    public IoController? IoController { get; set; }
    public FloorPlans? FloorPlanNavigation { get; set; }
}

public partial class Cardholder
{
    public Doors? LastDoorNavigation { get; set; }
    public EventTypes? LastEventNavigation { get; set; }
    public ApbZone? ApbzoneNavigation { get; set; }
    public CardholderCustomFields? CustomFields { get; set; }

    public ICollection<CardholderAccessLevel> CardholderAccessLevels { get; set; } = new List<CardholderAccessLevel>();
    public ICollection<Events> Events { get; set; } = new List<Events>();
    public ICollection<SpaceZoneAttendance> SpaceZoneAttendances { get; set; } = new List<SpaceZoneAttendance>();
    public ICollection<SpaceZoneCardholder> SpaceZoneCardholders { get; set; } = new List<SpaceZoneCardholder>();
}

public partial class CardholderCustomFields
{
    public Cardholder? Cardholder { get; set; }
}

public partial class CardholderAccessLevel
{
    public Sites? SiteNavigation { get; set; }
    public Cardholder? Cardholder { get; set; }
    public AccessLevels? AccessLevel { get; set; }
}

public partial class Events
{
    public Doors? DoorNavigation { get; set; }
    public Cardholder? CardNumberNavigation { get; set; }
    public EventTypes? EventTypeNavigation { get; set; }
}

public partial class EventTypes
{
    public ICollection<Events> Events { get; set; } = new List<Events>();
}

public partial class AccessLevels
{
    public Sites? SiteNavigation { get; set; }

    public ICollection<CardholderAccessLevel> CardholderAccessLevels { get; set; } = new List<CardholderAccessLevel>();
}

public partial class AccessLevelDoor
{
    public Doors? DoorNavigation { get; set; }
}

public partial class ApbZone
{
    public Sites? SiteNavigation { get; set; }

    public ICollection<ApbZoneDoor> ApbZoneDoors { get; set; } = new List<ApbZoneDoor>();
}

public partial class ApbZoneDoor
{
    public ApbZone? ApbZone { get; set; }
    public Doors? DoorNavigation { get; set; }
}

public partial class SpaceZone
{
    public Sites? SiteNavigation { get; set; }

    public ICollection<SpaceZoneDoor> SpaceZoneDoors { get; set; } = new List<SpaceZoneDoor>();
    public ICollection<SpaceZoneAttendance> SpaceZoneAttendances { get; set; } = new List<SpaceZoneAttendance>();
    public ICollection<SpaceZoneCardholder> SpaceZoneCardholders { get; set; } = new List<SpaceZoneCardholder>();
    public ICollection<TimeSheetZone> TimeSheetZones { get; set; } = new List<TimeSheetZone>();
}

public partial class SpaceZoneDoor
{
    public Sites? SiteNavigation { get; set; }
    public Doors? DoorNavigation { get; set; }
    public SpaceZone? ZoneNavigation { get; set; }
}

public partial class SpaceZoneAttendance
{
    public Cardholder? CardIndexNavigation { get; set; }
    public SpaceZone? ZoneNumberNavigation { get; set; }
}

public partial class SpaceZoneCardholder
{
    public Cardholder? Cardholder { get; set; }
    public SpaceZone? SpaceZoneNavigation { get; set; }
}

public partial class Calendar
{
    public Sites? SiteNavigation { get; set; }

    public ICollection<CalendarException> CalendarExceptions { get; set; } = new List<CalendarException>();
}

public partial class CalendarException
{
    public Calendar? Calendar { get; set; }
}

public partial class TimeZones
{
    public Sites? SiteNavigation { get; set; }
    public Calendar? CalendarNavigation { get; set; }
}

public partial class TimeZoneInterval
{
    public Calendar? CalendarNavigation { get; set; }
}

public partial class Alarms
{
    public Sites? SiteNavigation { get; set; }
    public EventTypes? EventTypeNavigation { get; set; }
}

public partial class FloorPlans
{
    public Sites? SiteNavigation { get; set; }
}

public partial class Trigger
{
    public Sites? SiteNavigation { get; set; }

    public ICollection<TriggerController> TriggerControllers { get; set; } = new List<TriggerController>();
    public ICollection<TriggerEvent> TriggerEvents { get; set; } = new List<TriggerEvent>();
}

public partial class TriggerController
{
    public Trigger? Trigger { get; set; }
}

public partial class TriggerEvent
{
    public Trigger? Trigger { get; set; }
    public EventTypes? EventTypeNavigation { get; set; }
}

public partial class CardManager
{
    public ICollection<CardManagerDefault> CardManagerDefaults { get; set; } = new List<CardManagerDefault>();
    public ICollection<CardManagerOrderByField> OrderByFields { get; set; } = new List<CardManagerOrderByField>();
    public ICollection<CardManagerSelectField> SelectFields { get; set; } = new List<CardManagerSelectField>();
    public ICollection<CardManagerWhereField> WhereFields { get; set; } = new List<CardManagerWhereField>();
}

public partial class CardManagerDefault
{
    public CardManager? CardManager { get; set; }
}

public partial class CardManagerOrderByField
{
    public CardManager? CardManager { get; set; }
}

public partial class CardManagerSelectField
{
    public CardManager? CardManager { get; set; }
}

public partial class CardManagerWhereField
{
    public CardManager? CardManager { get; set; }
}

public partial class CustomFieldTypes
{
    public ICollection<Custom> Customs { get; set; } = new List<Custom>();
}

public partial class Custom
{
    public CustomFieldTypes? CustomFieldType { get; set; }
}

public partial class DisplayTypes
{
    public ICollection<Display> Displays { get; set; } = new List<Display>();
}

public partial class Display
{
    public DisplayTypes? DisplayType { get; set; }
}

public partial class CardPack
{
    public ICollection<CardPackSite> CardPackSites { get; set; } = new List<CardPackSite>();
}

public partial class CardPackSite
{
    public Sites? SiteNavigation { get; set; }
    public CardPack? CardPackNavigation { get; set; }
}

public partial class TimeSheet
{
    public ICollection<TimeSheetZone> TimeSheetZones { get; set; } = new List<TimeSheetZone>();
}

public partial class TimeSheetZone
{
    public TimeSheet? TimeSheet { get; set; }
    public SpaceZone? ZoneNavigation { get; set; }
}

public partial class UserSites
{
    public Sites? SiteNavigation { get; set; }
}
