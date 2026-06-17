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
        modelBuilder.Entity<TDoors>(entity =>
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
        modelBuilder.Entity<TConnectors>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.Connectors)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_IOController_Header -------------------------------------------
        modelBuilder.Entity<TIocontrollerHeader>(entity =>
        {
            entity.HasOne(d => d.ConnectorNavigation)
                .WithMany(p => p.IocontrollerHeaders)
                .HasForeignKey(d => d.Connector)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_IOController_Details ------------------------------------------
        modelBuilder.Entity<TIocontrollerDetails>(entity =>
        {
            entity.HasOne(d => d.IocontrollerHeader)
                .WithMany(p => p.IocontrollerDetails)
                .HasForeignKey(d => d.ControllerId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.FloorPlanNavigation)
                .WithMany()
                .HasForeignKey(d => d.FloorPlan)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Name_Header ----------------------------------------------------
        modelBuilder.Entity<TNameHeader>(entity =>
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
        modelBuilder.Entity<TNameCustomFields>(entity =>
        {
            entity.HasOne(d => d.NameHeader)
                .WithOne(p => p.CustomFields)
                .HasForeignKey<TNameCustomFields>(d => d.CardNumber)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Name_AccessLevels ---------------------------------------------
        modelBuilder.Entity<TNameAccessLevels>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.NameAccessLevels)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.NameHeader)
                .WithMany(p => p.NameAccessLevels)
                .HasForeignKey(d => d.CardNumber)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.AccessLevelHeader)
                .WithMany(p => p.NameAccessLevels)
                .HasForeignKey(d => new { d.Level, d.Site })
                .HasPrincipalKey(p => new { p.AccessLevel, p.Site })
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Events ---------------------------------------------------------
        modelBuilder.Entity<TEvents>(entity =>
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
        modelBuilder.Entity<TAccessLevelHeader>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.AccessLevelHeaders)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_AccessLevel_Details -------------------------------------------
        modelBuilder.Entity<TAccessLevelDetails>(entity =>
        {
            entity.HasOne(d => d.DoorNavigation)
                .WithMany(p => p.AccessLevelDetails)
                .HasForeignKey(d => d.Door)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_APBZone_Header -------------------------------------------------
        modelBuilder.Entity<TApbzoneHeader>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.ApbzoneHeaders)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_APBZone_Details ------------------------------------------------
        modelBuilder.Entity<TApbzoneDetails>(entity =>
        {
            entity.HasOne(d => d.ApbzoneHeader)
                .WithMany(p => p.ApbzoneDetails)
                .HasForeignKey(d => d.Apbnumber)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.DoorNavigation)
                .WithMany(p => p.ApbzoneDetails)
                .HasForeignKey(d => d.DoorNumber)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_SpaceZone_Header ----------------------------------------------
        modelBuilder.Entity<TSpaceZoneHeader>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.SpaceZoneHeaders)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_SpaceZone_Details ---------------------------------------------
        modelBuilder.Entity<TSpaceZoneDetails>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.SpaceZoneDetails)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.DoorNavigation)
                .WithMany(p => p.SpaceZoneDetails)
                .HasForeignKey(d => d.Door)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.ZoneNavigation)
                .WithMany(p => p.SpaceZoneDetails)
                .HasForeignKey(d => d.Zone)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_SpaceZone_Attendance ------------------------------------------
        modelBuilder.Entity<TSpaceZoneAttendance>(entity =>
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
        modelBuilder.Entity<TSpaceZoneCardholders>(entity =>
        {
            entity.HasOne(d => d.NameHeader)
                .WithMany(p => p.SpaceZoneCardholders)
                .HasForeignKey(d => d.CardNumber)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.SpaceZoneNavigation)
                .WithMany(p => p.SpaceZoneCardholders)
                .HasForeignKey(d => d.SpaceZone)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Calendar_Header -----------------------------------------------
        modelBuilder.Entity<TCalendarHeader>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.CalendarHeaders)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Calendar_Details ----------------------------------------------
        modelBuilder.Entity<TCalendarDetails>(entity =>
        {
            entity.HasOne(d => d.CalendarHeader)
                .WithMany(p => p.CalendarDetails)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_TimeZone_Header -----------------------------------------------
        modelBuilder.Entity<TTimeZoneHeader>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.TimeZoneHeaders)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.CalendarNavigation)
                .WithMany()
                .HasForeignKey(d => d.Calendar)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_TimeZone_Details ----------------------------------------------
        modelBuilder.Entity<TTimeZoneDetails>(entity =>
        {
            entity.HasOne(d => d.CalendarNavigation)
                .WithMany()
                .HasForeignKey(d => d.Calendar)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Alarms ---------------------------------------------------------
        modelBuilder.Entity<TAlarms>(entity =>
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
        modelBuilder.Entity<TFloorPlans>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.FloorPlans)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Triggers_Header -----------------------------------------------
        modelBuilder.Entity<TTriggersHeader>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.TriggersHeaders)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Triggers_Controllers ------------------------------------------
        modelBuilder.Entity<TTriggersControllers>(entity =>
        {
            entity.HasOne(d => d.TriggersHeader)
                .WithMany(p => p.TriggersControllers)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Triggers_Events -----------------------------------------------
        modelBuilder.Entity<TTriggersEvents>(entity =>
        {
            entity.HasOne(d => d.TriggersHeader)
                .WithMany(p => p.TriggersEvents)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.EventTypeNavigation)
                .WithMany()
                .HasForeignKey(d => d.EventType)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Commands -------------------------------------------------------
        modelBuilder.Entity<TCommands>(entity =>
        {
            entity.HasOne(d => d.ConnectorNavigation)
                .WithMany(p => p.Commands)
                .HasForeignKey(d => d.Connector)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardManager_Default -------------------------------------------
        modelBuilder.Entity<TCardManagerDefault>(entity =>
        {
            entity.HasOne(d => d.CardManagerHeader)
                .WithMany(p => p.CardManagerDefaults)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardManager_OrderByFields -------------------------------------
        modelBuilder.Entity<TCardManagerOrderByFields>(entity =>
        {
            entity.HasOne(d => d.CardManagerHeader)
                .WithMany(p => p.OrderByFields)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardManager_SelectFields --------------------------------------
        modelBuilder.Entity<TCardManagerSelectFields>(entity =>
        {
            entity.HasOne(d => d.CardManagerHeader)
                .WithMany(p => p.SelectFields)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardManager_WhereFields ---------------------------------------
        modelBuilder.Entity<TCardManagerWhereFields>(entity =>
        {
            entity.HasOne(d => d.CardManagerHeader)
                .WithMany(p => p.WhereFields)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Custom ---------------------------------------------------------
        modelBuilder.Entity<TCustom>(entity =>
        {
            entity.HasOne(d => d.CustomFieldType)
                .WithMany(p => p.Customs)
                .HasForeignKey(d => d.CustomFieldCode)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_Display (composite FK -> T_DisplayTypes) ----------------------
        modelBuilder.Entity<TDisplay>(entity =>
        {
            entity.HasOne(d => d.DisplayType)
                .WithMany(p => p.Displays)
                .HasForeignKey(d => new { d.Code, d.PropertyId })
                .HasPrincipalKey(p => new { p.Code, p.PropertyId })
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardDesign_Details --------------------------------------------
        modelBuilder.Entity<TCardDesignDetails>(entity =>
        {
            entity.HasOne(d => d.CardDesignHeader)
                .WithMany(p => p.CardDesignDetails)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_CardPack_Details ----------------------------------------------
        modelBuilder.Entity<TCardPackDetails>(entity =>
        {
            entity.HasOne(d => d.SiteNavigation)
                .WithMany(p => p.CardPackDetails)
                .HasForeignKey(d => d.Site)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.CardPackNavigation)
                .WithMany(p => p.CardPackDetails)
                .HasForeignKey(d => d.CardPack)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_TimeSheet_Zones -----------------------------------------------
        modelBuilder.Entity<TTimeSheetZones>(entity =>
        {
            entity.HasOne(d => d.TimeSheetHeader)
                .WithMany(p => p.TimeSheetZones)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.ZoneNavigation)
                .WithMany(p => p.TimeSheetZones)
                .HasForeignKey(d => d.Zone)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ---- T_UserSites ------------------------------------------------------
        modelBuilder.Entity<TUserSites>(entity =>
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
        modelBuilder.Entity<TUsers>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.HasData(new TUsers
            {
                Code = 1,
                Description = "admin",
                Password = "$2a$11$USGWvPjw8RXKz9LjMWWgr.IV5uCz6Zufb2zTBjSBG9fneY.JY0UDW",
                Administrator = true
            });
        });
    }
}
