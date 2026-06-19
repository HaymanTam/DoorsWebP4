using DoorsWeb.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoorsWeb.API.Legacy;

// -----------------------------------------------------------------------------
// EF-only relationship configuration for the legacy DoorsEnterprise database.
//
// The live database has NO foreign-key constraints; these relationships are
// reconstructed from the JOINs used by the old VB6 server's stored procedures
// (LegacyDoorsServer). NO database schema is changed - this only teaches the EF
// model how the tables relate so navigations and Include() work.
//
// Every relationship is configured explicitly because the legacy column names do
// not follow EF conventions (e.g. "Site", "Connector", "Apbnumber" rather than
// "...Id"), and several entity pairs have multiple relationships (e.g. T_Doors
// has three FKs into T_DoorTechnology). Explicit config also prevents EF's
// by-convention discovery from inventing shadow FK columns.
//
// DeleteBehavior.NoAction is used everywhere: the legacy DB never cascaded, and
// many tables sit on multiple required relationships, which would otherwise trip
// EF's multiple-cascade-path validation.
//
// Lookup-style references (technology, floor plan, calendar, event-type lookups,
// "last door/event", card design, APB-zone-from-name) are configured with
// WithMany() and no inverse collection, to avoid cluttering lookup entities with
// large back-reference collections.
// -----------------------------------------------------------------------------

public partial class DoorsEnterpriseContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // ---- T_Doors ----------------------------------------------------------
        modelBuilder.Entity<Doors>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.Doors)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.ConnectorNavigation)
                .WithMany(p => p.Doors)
                .HasForeignKey(d => d.Connector)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.TechnologyANavigation)
                .WithMany()
                .HasForeignKey(d => d.TechnologyA)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.TechnologyBNavigation)
                .WithMany()
                .HasForeignKey(d => d.TechnologyB)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.KeyboardTechNavigation)
                .WithMany()
                .HasForeignKey(d => d.KeyboardTech)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.FloorPlanNavigation)
                .WithMany()
                .HasForeignKey(d => d.FloorPlan)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Connectors -----------------------------------------------------
        modelBuilder.Entity<Connectors>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.Connectors)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_IOController_Header -------------------------------------------
        modelBuilder.Entity<IoController>(entity =>
        {
            entity.HasOne(d => d.ConnectorNavigation)
                .WithMany(p => p.IoControllers)
                .HasForeignKey(d => d.Connector)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_IOController_Details ------------------------------------------
        modelBuilder.Entity<IoControllerInput>(entity =>
        {
            entity.HasOne(d => d.IoController)
                .WithMany(p => p.IoControllerInputs)
                .HasForeignKey(d => d.ControllerId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.FloorPlanNavigation)
                .WithMany()
                .HasForeignKey(d => d.FloorPlan)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Name_Header ----------------------------------------------------
        modelBuilder.Entity<Cardholder>(entity =>
        {
            entity.HasOne(d => d.LastDoorNavigation)
                .WithMany()
                .HasForeignKey(d => d.LastDoor)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.LastEventNavigation)
                .WithMany()
                .HasForeignKey(d => d.LastEvent)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.ApbzoneNavigation)
                .WithMany()
                .HasForeignKey(d => d.Apbnumber)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.CardDesignNavigation)
                .WithMany()
                .HasForeignKey(d => d.CardDesign)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Name_CustomFields (one-to-one, shared PK) ---------------------
        modelBuilder.Entity<CardholderCustomFields>(entity =>
        {
            entity.HasOne(d => d.Cardholder)
                .WithOne(p => p.CustomFields)
                .HasForeignKey<CardholderCustomFields>(d => d.CardNumber)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Name_AccessLevels ---------------------------------------------
        modelBuilder.Entity<CardholderAccessLevel>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.CardholderAccessLevels)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.Cardholder)
                .WithMany(p => p.CardholderAccessLevels)
                .HasForeignKey(d => d.CardNumber)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.AccessLevel)
                .WithMany(p => p.CardholderAccessLevels)
                .HasForeignKey(d => new { d.Level, d.Site })
                .HasPrincipalKey(p => new { p.AccessLevel, p.Site })
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Events ---------------------------------------------------------
        modelBuilder.Entity<Events>(entity =>
        {
            entity.HasOne(d => d.DoorNavigation)
                .WithMany(p => p.Events)
                .HasForeignKey(d => d.DoorNumber)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.CardNumberNavigation)
                .WithMany(p => p.Events)
                .HasForeignKey(d => d.CardNumber)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.EventTypeNavigation)
                .WithMany(p => p.Events)
                .HasForeignKey(d => d.EventType)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_AccessLevel_Header --------------------------------------------
        modelBuilder.Entity<AccessLevels>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.AccessLevels)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_AccessLevel_Details -------------------------------------------
        modelBuilder.Entity<AccessLevelDoor>(entity =>
        {
            entity.HasOne(d => d.DoorNavigation)
                .WithMany(p => p.AccessLevelDoors)
                .HasForeignKey(d => d.Door)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_APBZone_Header -------------------------------------------------
        modelBuilder.Entity<ApbZone>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.ApbZones)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_APBZone_Details ------------------------------------------------
        modelBuilder.Entity<ApbZoneDoor>(entity =>
        {
            entity.HasOne(d => d.ApbZone)
                .WithMany(p => p.ApbZoneDoors)
                .HasForeignKey(d => d.Apbnumber)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.DoorNavigation)
                .WithMany(p => p.ApbZoneDoors)
                .HasForeignKey(d => d.DoorNumber)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_SpaceZone_Header ----------------------------------------------
        modelBuilder.Entity<SpaceZone>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.SpaceZones)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_SpaceZone_Details ---------------------------------------------
        modelBuilder.Entity<SpaceZoneDoor>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.SpaceZoneDoors)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.DoorNavigation)
                .WithMany(p => p.SpaceZoneDoors)
                .HasForeignKey(d => d.Door)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.ZoneNavigation)
                .WithMany(p => p.SpaceZoneDoors)
                .HasForeignKey(d => d.Zone)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_SpaceZone_Attendance ------------------------------------------
        modelBuilder.Entity<SpaceZoneAttendance>(entity =>
        {
            entity.HasOne(d => d.CardIndexNavigation)
                .WithMany(p => p.SpaceZoneAttendances)
                .HasForeignKey(d => d.CardIndex)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.ZoneNumberNavigation)
                .WithMany(p => p.SpaceZoneAttendances)
                .HasForeignKey(d => d.ZoneNumber)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_SpaceZone_Cardholders -----------------------------------------
        modelBuilder.Entity<SpaceZoneCardholder>(entity =>
        {
            entity.HasOne(d => d.Cardholder)
                .WithMany(p => p.SpaceZoneCardholders)
                .HasForeignKey(d => d.CardNumber)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.SpaceZoneNavigation)
                .WithMany(p => p.SpaceZoneCardholders)
                .HasForeignKey(d => d.SpaceZone)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Calendar_Header -----------------------------------------------
        modelBuilder.Entity<Calendar>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.Calendars)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Calendar_Details ----------------------------------------------
        modelBuilder.Entity<CalendarException>(entity =>
        {
            entity.HasOne(d => d.Calendar)
                .WithMany(p => p.CalendarExceptions)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_TimeZone_Header -----------------------------------------------
        modelBuilder.Entity<TimeZones>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.TimeZones)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.CalendarNavigation)
                .WithMany()
                .HasForeignKey(d => d.Calendar)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_TimeZone_Details ----------------------------------------------
        modelBuilder.Entity<TimeZoneInterval>(entity =>
        {
            entity.HasOne(d => d.CalendarNavigation)
                .WithMany()
                .HasForeignKey(d => d.Calendar)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Alarms ---------------------------------------------------------
        modelBuilder.Entity<Alarms>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.Alarms)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.EventTypeNavigation)
                .WithMany()
                .HasForeignKey(d => d.EventType)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_FloorPlans -----------------------------------------------------
        modelBuilder.Entity<FloorPlans>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.FloorPlans)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Triggers_Header -----------------------------------------------
        modelBuilder.Entity<Trigger>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.Triggers)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Triggers_Controllers ------------------------------------------
        modelBuilder.Entity<TriggerController>(entity =>
        {
            entity.HasOne(d => d.Trigger)
                .WithMany(p => p.TriggerControllers)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Triggers_Events -----------------------------------------------
        modelBuilder.Entity<TriggerEvent>(entity =>
        {
            entity.HasOne(d => d.Trigger)
                .WithMany(p => p.TriggerEvents)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.EventTypeNavigation)
                .WithMany()
                .HasForeignKey(d => d.EventType)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Commands -------------------------------------------------------
        modelBuilder.Entity<Commands>(entity =>
        {
            entity.HasOne(d => d.ConnectorNavigation)
                .WithMany(p => p.Commands)
                .HasForeignKey(d => d.Connector)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardManager_Default -------------------------------------------
        modelBuilder.Entity<CardManagerDefault>(entity =>
        {
            entity.HasOne(d => d.CardManager)
                .WithMany(p => p.CardManagerDefaults)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardManager_OrderByFields -------------------------------------
        modelBuilder.Entity<CardManagerOrderByField>(entity =>
        {
            entity.HasOne(d => d.CardManager)
                .WithMany(p => p.OrderByFields)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardManager_SelectFields --------------------------------------
        modelBuilder.Entity<CardManagerSelectField>(entity =>
        {
            entity.HasOne(d => d.CardManager)
                .WithMany(p => p.SelectFields)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardManager_WhereFields ---------------------------------------
        modelBuilder.Entity<CardManagerWhereField>(entity =>
        {
            entity.HasOne(d => d.CardManager)
                .WithMany(p => p.WhereFields)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Custom ---------------------------------------------------------
        modelBuilder.Entity<Custom>(entity =>
        {
            entity.HasOne(d => d.CustomFieldType)
                .WithMany(p => p.Customs)
                .HasForeignKey(d => d.CustomFieldCode)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Display (composite FK -> T_DisplayTypes) ----------------------
        modelBuilder.Entity<Display>(entity =>
        {
            entity.HasOne(d => d.DisplayType)
                .WithMany(p => p.Displays)
                .HasForeignKey(d => new { d.Code, d.PropertyId })
                .HasPrincipalKey(p => new { p.Code, p.PropertyId })
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardDesign_Details --------------------------------------------
        modelBuilder.Entity<CardDesignField>(entity =>
        {
            entity.HasOne(d => d.CardDesign)
                .WithMany(p => p.CardDesignFields)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardPack_Details ----------------------------------------------
        modelBuilder.Entity<CardPackSite>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.CardPackSites)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.CardPackNavigation)
                .WithMany(p => p.CardPackSites)
                .HasForeignKey(d => d.CardPack)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_TimeSheet_Zones -----------------------------------------------
        modelBuilder.Entity<TimeSheetZone>(entity =>
        {
            entity.HasOne(d => d.TimeSheet)
                .WithMany(p => p.TimeSheetZones)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.ZoneNavigation)
                .WithMany(p => p.TimeSheetZones)
                .HasForeignKey(d => d.Zone)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_UserSites ------------------------------------------------------
        modelBuilder.Entity<UserSites>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.UserSites)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Users (default seed) ------------------------------------------
        // Seeds the default "admin" administrator. T_Users is scaffolded keyless,
        // so a key on Code is required before HasData can be applied.
        //
        // Password is a bcrypt hash of the default "654321" (see AuthService.
        // DefaultPassword). A precomputed constant is used because a fresh
        // BCrypt.HashPassword() call produces a new salt each model build, which
        // would generate spurious migrations. Users on this default are forced to
        // change it on first sign-in.
        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.HasData(new Users
            {
                Code = 1,
                Description = "admin",
                Password = "$2a$11$USGWvPjw8RXKz9LjMWWgr.IV5uCz6Zufb2zTBjSBG9fneY.JY0UDW",
                Administrator = true
            });
        });

        // ---- T_Sites (default seed) ------------------------------------------
        // Seeds a single "Default Site" so a fresh database always has at least
        // one site. The System Settings dialog (and SiteService.Delete) prevent
        // removing the final remaining site, so this guarantees there is always
        // somewhere for doors / zones / triggers / etc. to belong to.
        modelBuilder.Entity<Sites>().HasData(new Sites
        {
            Site = 1,
            Name = "Default Site",
            Inuse = true
        });
    }
}
