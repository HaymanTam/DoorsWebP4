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
//     descriptive principal-type name (e.g. CalendarHeader, NameHeader).
//   * Collection navigations are plural and initialised to new List<>().
// -----------------------------------------------------------------------------

public partial class TSites
{
    public ICollection<TConnectors> Connectors { get; set; } = new List<TConnectors>();
    public ICollection<TDoors> Doors { get; set; } = new List<TDoors>();
    public ICollection<TAccessLevelHeader> AccessLevelHeaders { get; set; } = new List<TAccessLevelHeader>();
    public ICollection<TApbzoneHeader> ApbzoneHeaders { get; set; } = new List<TApbzoneHeader>();
    public ICollection<TSpaceZoneHeader> SpaceZoneHeaders { get; set; } = new List<TSpaceZoneHeader>();
    public ICollection<TCalendarHeader> CalendarHeaders { get; set; } = new List<TCalendarHeader>();
    public ICollection<TTimeZoneHeader> TimeZoneHeaders { get; set; } = new List<TTimeZoneHeader>();
    public ICollection<TAlarms> Alarms { get; set; } = new List<TAlarms>();
    public ICollection<TFloorPlans> FloorPlans { get; set; } = new List<TFloorPlans>();
    public ICollection<TTriggersHeader> TriggersHeaders { get; set; } = new List<TTriggersHeader>();
    public ICollection<TCardPackDetails> CardPackDetails { get; set; } = new List<TCardPackDetails>();
    public ICollection<TNameAccessLevels> NameAccessLevels { get; set; } = new List<TNameAccessLevels>();
    public ICollection<TSpaceZoneDetails> SpaceZoneDetails { get; set; } = new List<TSpaceZoneDetails>();
    public ICollection<TUserSites> UserSites { get; set; } = new List<TUserSites>();
}

public partial class TDoors
{
    public TSites? SiteNavigation { get; set; }
    public TConnectors? ConnectorNavigation { get; set; }
    public TDoorTechnology? TechnologyANavigation { get; set; }
    public TDoorTechnology? TechnologyBNavigation { get; set; }
    public TDoorTechnology? KeyboardTechNavigation { get; set; }
    public TFloorPlans? FloorPlanNavigation { get; set; }

    public ICollection<TAccessLevelDetails> AccessLevelDetails { get; set; } = new List<TAccessLevelDetails>();
    public ICollection<TApbzoneDetails> ApbzoneDetails { get; set; } = new List<TApbzoneDetails>();
    public ICollection<TEvents> Events { get; set; } = new List<TEvents>();
    public ICollection<TSpaceZoneDetails> SpaceZoneDetails { get; set; } = new List<TSpaceZoneDetails>();
}

public partial class TConnectors
{
    public TSites? SiteNavigation { get; set; }

    public ICollection<TDoors> Doors { get; set; } = new List<TDoors>();
    public ICollection<TIocontrollerHeader> IocontrollerHeaders { get; set; } = new List<TIocontrollerHeader>();
    public ICollection<TCommands> Commands { get; set; } = new List<TCommands>();
}

public partial class TIocontrollerHeader
{
    public TConnectors? ConnectorNavigation { get; set; }

    public ICollection<TIocontrollerDetails> IocontrollerDetails { get; set; } = new List<TIocontrollerDetails>();
}

public partial class TIocontrollerDetails
{
    public TIocontrollerHeader? IocontrollerHeader { get; set; }
    public TFloorPlans? FloorPlanNavigation { get; set; }
}

public partial class TNameHeader
{
    public TDoors? LastDoorNavigation { get; set; }
    public TEventTypes? LastEventNavigation { get; set; }
    public TApbzoneHeader? ApbzoneNavigation { get; set; }
    public TCardDesignHeader? CardDesignNavigation { get; set; }
    public TNameCustomFields? CustomFields { get; set; }

    public ICollection<TNameAccessLevels> NameAccessLevels { get; set; } = new List<TNameAccessLevels>();
    public ICollection<TEvents> Events { get; set; } = new List<TEvents>();
    public ICollection<TSpaceZoneAttendance> SpaceZoneAttendances { get; set; } = new List<TSpaceZoneAttendance>();
    public ICollection<TSpaceZoneCardholders> SpaceZoneCardholders { get; set; } = new List<TSpaceZoneCardholders>();
}

public partial class TNameCustomFields
{
    public TNameHeader? NameHeader { get; set; }
}

public partial class TNameAccessLevels
{
    public TSites? SiteNavigation { get; set; }
    public TNameHeader? NameHeader { get; set; }
    public TAccessLevelHeader? AccessLevelHeader { get; set; }
}

public partial class TEvents
{
    public TDoors? DoorNavigation { get; set; }
    public TNameHeader? CardNumberNavigation { get; set; }
    public TEventTypes? EventTypeNavigation { get; set; }
}

public partial class TEventTypes
{
    public ICollection<TEvents> Events { get; set; } = new List<TEvents>();
}

public partial class TAccessLevelHeader
{
    public TSites? SiteNavigation { get; set; }

    public ICollection<TNameAccessLevels> NameAccessLevels { get; set; } = new List<TNameAccessLevels>();
}

public partial class TAccessLevelDetails
{
    public TDoors? DoorNavigation { get; set; }
}

public partial class TApbzoneHeader
{
    public TSites? SiteNavigation { get; set; }

    public ICollection<TApbzoneDetails> ApbzoneDetails { get; set; } = new List<TApbzoneDetails>();
}

public partial class TApbzoneDetails
{
    public TApbzoneHeader? ApbzoneHeader { get; set; }
    public TDoors? DoorNavigation { get; set; }
}

public partial class TSpaceZoneHeader
{
    public TSites? SiteNavigation { get; set; }

    public ICollection<TSpaceZoneDetails> SpaceZoneDetails { get; set; } = new List<TSpaceZoneDetails>();
    public ICollection<TSpaceZoneAttendance> SpaceZoneAttendances { get; set; } = new List<TSpaceZoneAttendance>();
    public ICollection<TSpaceZoneCardholders> SpaceZoneCardholders { get; set; } = new List<TSpaceZoneCardholders>();
    public ICollection<TTimeSheetZones> TimeSheetZones { get; set; } = new List<TTimeSheetZones>();
}

public partial class TSpaceZoneDetails
{
    public TSites? SiteNavigation { get; set; }
    public TDoors? DoorNavigation { get; set; }
    public TSpaceZoneHeader? ZoneNavigation { get; set; }
}

public partial class TSpaceZoneAttendance
{
    public TNameHeader? CardIndexNavigation { get; set; }
    public TSpaceZoneHeader? ZoneNumberNavigation { get; set; }
}

public partial class TSpaceZoneCardholders
{
    public TNameHeader? NameHeader { get; set; }
    public TSpaceZoneHeader? SpaceZoneNavigation { get; set; }
}

public partial class TCalendarHeader
{
    public TSites? SiteNavigation { get; set; }

    public ICollection<TCalendarDetails> CalendarDetails { get; set; } = new List<TCalendarDetails>();
}

public partial class TCalendarDetails
{
    public TCalendarHeader? CalendarHeader { get; set; }
}

public partial class TTimeZoneHeader
{
    public TSites? SiteNavigation { get; set; }
    public TCalendarHeader? CalendarNavigation { get; set; }
}

public partial class TTimeZoneDetails
{
    public TCalendarHeader? CalendarNavigation { get; set; }
}

public partial class TAlarms
{
    public TSites? SiteNavigation { get; set; }
    public TEventTypes? EventTypeNavigation { get; set; }
}

public partial class TFloorPlans
{
    public TSites? SiteNavigation { get; set; }
}

public partial class TTriggersHeader
{
    public TSites? SiteNavigation { get; set; }

    public ICollection<TTriggersControllers> TriggersControllers { get; set; } = new List<TTriggersControllers>();
    public ICollection<TTriggersEvents> TriggersEvents { get; set; } = new List<TTriggersEvents>();
}

public partial class TTriggersControllers
{
    public TTriggersHeader? TriggersHeader { get; set; }
}

public partial class TTriggersEvents
{
    public TTriggersHeader? TriggersHeader { get; set; }
    public TEventTypes? EventTypeNavigation { get; set; }
}

public partial class TCommands
{
    public TConnectors? ConnectorNavigation { get; set; }
}

public partial class TCardManagerHeader
{
    public ICollection<TCardManagerDefault> CardManagerDefaults { get; set; } = new List<TCardManagerDefault>();
    public ICollection<TCardManagerOrderByFields> OrderByFields { get; set; } = new List<TCardManagerOrderByFields>();
    public ICollection<TCardManagerSelectFields> SelectFields { get; set; } = new List<TCardManagerSelectFields>();
    public ICollection<TCardManagerWhereFields> WhereFields { get; set; } = new List<TCardManagerWhereFields>();
}

public partial class TCardManagerDefault
{
    public TCardManagerHeader? CardManagerHeader { get; set; }
}

public partial class TCardManagerOrderByFields
{
    public TCardManagerHeader? CardManagerHeader { get; set; }
}

public partial class TCardManagerSelectFields
{
    public TCardManagerHeader? CardManagerHeader { get; set; }
}

public partial class TCardManagerWhereFields
{
    public TCardManagerHeader? CardManagerHeader { get; set; }
}

public partial class TCustomFieldTypes
{
    public ICollection<TCustom> Customs { get; set; } = new List<TCustom>();
}

public partial class TCustom
{
    public TCustomFieldTypes? CustomFieldType { get; set; }
}

public partial class TDisplayTypes
{
    public ICollection<TDisplay> Displays { get; set; } = new List<TDisplay>();
}

public partial class TDisplay
{
    public TDisplayTypes? DisplayType { get; set; }
}

public partial class TCardDesignHeader
{
    public ICollection<TCardDesignDetails> CardDesignDetails { get; set; } = new List<TCardDesignDetails>();
}

public partial class TCardDesignDetails
{
    public TCardDesignHeader? CardDesignHeader { get; set; }
}

public partial class TCardPackHeader
{
    public ICollection<TCardPackDetails> CardPackDetails { get; set; } = new List<TCardPackDetails>();
}

public partial class TCardPackDetails
{
    public TSites? SiteNavigation { get; set; }
    public TCardPackHeader? CardPackNavigation { get; set; }
}

public partial class TTimeSheetHeader
{
    public ICollection<TTimeSheetZones> TimeSheetZones { get; set; } = new List<TTimeSheetZones>();
}

public partial class TTimeSheetZones
{
    public TTimeSheetHeader? TimeSheetHeader { get; set; }
    public TSpaceZoneHeader? ZoneNavigation { get; set; }
}

public partial class TUserSites
{
    public TSites? SiteNavigation { get; set; }
}
