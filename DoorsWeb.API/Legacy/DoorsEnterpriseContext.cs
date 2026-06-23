using System;
using System.Collections.Generic;
using DoorsWeb.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoorsWeb.API.Legacy;

public partial class DoorsEnterpriseContext : DbContext
{
    public DoorsEnterpriseContext(DbContextOptions<DoorsEnterpriseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessLevelDoor> AccessLevelDoor { get; set; }

    public virtual DbSet<AccessLevels> AccessLevels { get; set; }

    public virtual DbSet<Alarms> Alarms { get; set; }

    public virtual DbSet<ApbZoneDoor> ApbZoneDoor { get; set; }

    public virtual DbSet<ApbZone> ApbZone { get; set; }

    public virtual DbSet<Audit> Audit { get; set; }

    public virtual DbSet<CalendarException> CalendarException { get; set; }

    public virtual DbSet<Calendar> Calendar { get; set; }

    public virtual DbSet<CardManagerDefault> CardManagerDefault { get; set; }

    public virtual DbSet<CardManager> CardManager { get; set; }

    public virtual DbSet<CardManagerOrderByField> CardManagerOrderByField { get; set; }

    public virtual DbSet<CardManagerSelectField> CardManagerSelectField { get; set; }

    public virtual DbSet<CardManagerWhereField> CardManagerWhereField { get; set; }

    public virtual DbSet<CardPackSite> CardPackSite { get; set; }

    public virtual DbSet<CardPack> CardPack { get; set; }

    public virtual DbSet<Custom> Custom { get; set; }

    public virtual DbSet<CustomFieldTypes> CustomFieldTypes { get; set; }

    public virtual DbSet<Display> Display { get; set; }

    public virtual DbSet<DisplayTypes> DisplayTypes { get; set; }

    public virtual DbSet<DoorTechnology> DoorTechnology { get; set; }

    public virtual DbSet<Doors> Doors { get; set; }

    public virtual DbSet<EventTypes> EventTypes { get; set; }

    public virtual DbSet<Events> Events { get; set; }

    public virtual DbSet<FloorPlans> FloorPlans { get; set; }

    public virtual DbSet<IoControllerInput> IoControllerInput { get; set; }

    public virtual DbSet<IoController> IoController { get; set; }

    public virtual DbSet<CardholderAccessLevel> CardholderAccessLevel { get; set; }

    public virtual DbSet<CardholderCustomFields> CardholderCustomFields { get; set; }

    public virtual DbSet<Cardholder> Cardholder { get; set; }

    public virtual DbSet<Sites> Sites { get; set; }

    public virtual DbSet<SpaceZoneAttendance> SpaceZoneAttendance { get; set; }

    public virtual DbSet<SpaceZoneCardholder> SpaceZoneCardholder { get; set; }

    public virtual DbSet<SpaceZoneDoor> SpaceZoneDoor { get; set; }

    public virtual DbSet<SpaceZone> SpaceZone { get; set; }

    public virtual DbSet<TimeSheet> TimeSheet { get; set; }

    public virtual DbSet<TimeSheetZone> TimeSheetZone { get; set; }

    public virtual DbSet<TimeZoneInterval> TimeZoneInterval { get; set; }

    public virtual DbSet<TimeZones> TimeZones { get; set; }

    public virtual DbSet<TriggerController> TriggerController { get; set; }

    public virtual DbSet<TriggerEvent> TriggerEvent { get; set; }

    public virtual DbSet<Trigger> Trigger { get; set; }

    public virtual DbSet<UserPermissions> UserPermissions { get; set; }

    public virtual DbSet<UserSites> UserSites { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessLevelDoor>(entity =>
        {
            entity.HasKey(e => new { e.Level, e.Door })
                .HasName("PK_AccessLevel_Details");

            entity.ToTable("T_AccessLevel_Details");
        });

        modelBuilder.Entity<AccessLevels>(entity =>
        {
            entity.HasKey(e => new { e.AccessLevel, e.Site })
                .HasName("PK_AccessLevel_Header");

            entity.ToTable("T_AccessLevel_Header");

            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Alarms>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_Alarms");

            entity.ToTable("T_Alarms");

            entity.Property(e => e.Code).ValueGeneratedNever();
            entity.Property(e => e.ActionedBy).HasMaxLength(20);
            entity.Property(e => e.ActionedDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ActionedText).HasMaxLength(255);
            entity.Property(e => e.AlarmDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.AlarmDescription).HasMaxLength(50);
        });

        modelBuilder.Entity<ApbZoneDoor>(entity =>
        {
            entity.HasKey(e => new { e.Apbnumber, e.DoorNumber })
                .HasName("PK_APBZone_Details");

            entity.ToTable("T_APBZone_Details");

            entity.Property(e => e.Apbnumber).HasColumnName("APBNumber");
            entity.Property(e => e.Key).HasMaxLength(10);
        });

        modelBuilder.Entity<ApbZone>(entity =>
        {
            entity.HasKey(e => e.Apbnumber)
                .HasName("PK_APBZone_Header");

            entity.ToTable("T_APBZone_Header");

            entity.Property(e => e.Apbnumber)
                .ValueGeneratedNever()
                .HasColumnName("APBNumber");
            entity.Property(e => e.Apbmode).HasColumnName("APBMode");
            entity.Property(e => e.DiscoveryModeExpiryDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.DiscoveryModeStart).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Key).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.NextAutoLogout).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("T_Audit");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Timestamp).HasColumnType("timestamp without time zone");
            entity.Property(e => e.UserName).HasMaxLength(60).IsUnicode(false);
            entity.Property(e => e.ClientIp).HasMaxLength(45).IsUnicode(false);
            entity.Property(e => e.Action).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.EntityType).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.EntityKey).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.EntityName).HasMaxLength(200);

            // Audit log is queried newest-first and filtered by user/type; index the time column.
            entity.HasIndex(e => e.Timestamp);
        });

        modelBuilder.Entity<CalendarException>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.ExceptionDate })
                .HasName("PK_Calendar_Details");

            entity.ToTable("T_Calendar_Details");

            entity.Property(e => e.ExceptionDate).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<Calendar>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_Calendar_Header");

            entity.ToTable("T_Calendar_Header");

            entity.Property(e => e.Description).HasMaxLength(30);
        });

        modelBuilder.Entity<CardManagerDefault>(entity =>
        {
            entity.HasKey(e => e.User)
                .HasName("PK_CardManager_Default");

            entity.ToTable("T_CardManager_Default");

            entity.Property(e => e.User).ValueGeneratedNever();
        });

        modelBuilder.Entity<CardManager>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_CardManager_Header");

            entity.ToTable("T_CardManager_Header");

            entity.Property(e => e.Code).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(30);
            entity.Property(e => e.Printer).HasMaxLength(30);
        });

        modelBuilder.Entity<CardManagerOrderByField>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.SortNumber })
                .HasName("PK_CardManager_OrderByFields");

            entity.ToTable("T_CardManager_OrderByFields");

            entity.Property(e => e.FieldName).HasMaxLength(30);
            entity.Property(e => e.TableName).HasMaxLength(30);
        });

        modelBuilder.Entity<CardManagerSelectField>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Position })
                .HasName("PK_CardManager_SelectFields");

            entity.ToTable("T_CardManager_SelectFields");

            entity.Property(e => e.FieldName).HasMaxLength(30);
            entity.Property(e => e.TableName).HasMaxLength(30);
        });

        modelBuilder.Entity<CardManagerWhereField>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Sequence })
                .HasName("PK_CardManager_WhereFields");

            entity.ToTable("T_CardManager_WhereFields");

            entity.Property(e => e.FieldName).HasMaxLength(30);
            entity.Property(e => e.Operator).HasMaxLength(10);
            entity.Property(e => e.TableName).HasMaxLength(30);
            entity.Property(e => e.Value1).HasMaxLength(30);
            entity.Property(e => e.Value2).HasMaxLength(30);
        });

        modelBuilder.Entity<CardPackSite>(entity =>
        {
            entity.HasKey(e => new { e.Site, e.CardPack })
                .HasName("PK_CardPack_Details");

            entity.ToTable("T_CardPack_Details");
        });

        modelBuilder.Entity<CardPack>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_CardPack_Header");

            entity.ToTable("T_CardPack_Header");

            entity.Property(e => e.FirstCardId)
                .HasMaxLength(8)
                .HasColumnName("FirstCardID");
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Custom>(entity =>
        {
            entity.HasKey(e => new { e.CustomFieldCode, e.Code })
                .HasName("PK_Custom");

            entity.ToTable("T_Custom");

            entity.Property(e => e.Description).HasMaxLength(30);
        });

        modelBuilder.Entity<CustomFieldTypes>(entity =>
        {
            entity.HasKey(e => e.CustomField)
                .HasName("PK_CustomFieldTypes");

            entity.ToTable("T_CustomFieldTypes");

            entity.Property(e => e.CustomField).ValueGeneratedNever();
            entity.Property(e => e.Literal).HasMaxLength(30);
        });

        modelBuilder.Entity<Display>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Position })
                .HasName("PK_Display");

            entity.ToTable("T_Display");

            entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
        });

        modelBuilder.Entity<DisplayTypes>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.PropertyId })
                .HasName("PK_DisplayTypes");

            entity.ToTable("T_DisplayTypes");

            entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
            entity.Property(e => e.Description)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.FalseDescription)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TrueDescription)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DoorTechnology>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_Door_Technology");

            entity.ToTable("T_Door_Technology");

            entity.Property(e => e.Code).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(30);
        });

        modelBuilder.Entity<Doors>(entity =>
        {
            entity.HasKey(e => e.Door)
                .HasName("PK_Doors");

            entity.ToTable("T_Doors");

            entity.HasIndex(e => e.Door, "IND_Door").IsUnique();

            entity.Property(e => e.Door).ValueGeneratedNever();
            entity.Property(e => e.AccessCodeDig1).HasColumnName("AccessCode_Dig1");
            entity.Property(e => e.AccessCodeDig2).HasColumnName("AccessCode_Dig2");
            entity.Property(e => e.AccessCodeDig3).HasColumnName("AccessCode_Dig3");
            entity.Property(e => e.AccessCodeDig4).HasColumnName("AccessCode_Dig4");
            entity.Property(e => e.AccessCodeDig5).HasColumnName("AccessCode_Dig5");
            entity.Property(e => e.AccessCodeDig6).HasColumnName("AccessCode_Dig6");
            entity.Property(e => e.AccessCodeDig7).HasColumnName("AccessCode_Dig7");
            entity.Property(e => e.AccessCodeDig8).HasColumnName("AccessCode_Dig8");
            entity.Property(e => e.AccessCodeLen).HasColumnName("AccessCode_Len");
            entity.Property(e => e.BioEnrolA).HasColumnName("Bio_Enrol_A");
            entity.Property(e => e.BioEnrolB).HasColumnName("Bio_Enrol_B");
            entity.Property(e => e.CardandPintimeZone).HasColumnName("CardandPINTimeZone");
            entity.Property(e => e.ConAlmVolume).HasColumnName("CON_ALM_Volume");
            entity.Property(e => e.ConFbVolume).HasColumnName("CON_FB_Volume");
            entity.Property(e => e.ControllerId)
                .HasMaxLength(12)
                .HasColumnName("ControllerID");
            entity.Property(e => e.ControllerIp).HasColumnName("ControllerIP");
            entity.Property(e => e.DoorIpaddress)
                .HasMaxLength(20)
                .HasColumnName("DoorIPAddress");
            entity.Property(e => e.IdSequenceA).HasColumnName("ID_Sequence_A");
            entity.Property(e => e.IdSequenceB).HasColumnName("ID_Sequence_B");
            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.KeyboardName).HasMaxLength(30);
            entity.Property(e => e.KeypadStarMode).HasColumnName("Keypad_Star_Mode");
            entity.Property(e => e.LastCardId)
                .HasMaxLength(12)
                .HasColumnName("LastCardID");
            entity.Property(e => e.LastDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.LockDriveMode).HasColumnName("Lock_Drive_Mode");
            entity.Property(e => e.Modified).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.Pdo).HasColumnName("PDO");
            entity.Property(e => e.RandomSearchFreq).HasColumnName("Random_Search_Freq");
            entity.Property(e => e.RdrBrightnessA).HasColumnName("RDR_Brightness_A");
            entity.Property(e => e.RdrBrightnessB).HasColumnName("RDR_Brightness_B");
            entity.Property(e => e.RdrVolumeA).HasColumnName("RDR_Volume_A");
            entity.Property(e => e.RdrVolumeB).HasColumnName("RDR_Volume_B");
            entity.Property(e => e.ReaderAname)
                .HasMaxLength(30)
                .HasColumnName("ReaderAName");
            entity.Property(e => e.ReaderBname)
                .HasMaxLength(30)
                .HasColumnName("ReaderBName");
            entity.Property(e => e.RelayBtimeZone).HasColumnName("RelayBTimeZone");
            entity.Property(e => e.Rtcdate)
                .HasMaxLength(12)
                .HasColumnName("RTCDate");
            entity.Property(e => e.Rtctime)
                .HasMaxLength(12)
                .HasColumnName("RTCTime");
            entity.Property(e => e.Updated).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ValidFromTimeHh).HasColumnName("ValidFromTimeHH");
            entity.Property(e => e.ValidFromTimeMm).HasColumnName("ValidFromTimeMM");
            entity.Property(e => e.ValidToTimeHh).HasColumnName("ValidToTimeHH");
            entity.Property(e => e.ValidToTimeMm).HasColumnName("ValidToTimeMM");
            entity.Property(e => e.Xplace).HasColumnName("XPlace");
            entity.Property(e => e.Yplace).HasColumnName("YPlace");
        });

        modelBuilder.Entity<EventTypes>(entity =>
        {
            entity.HasKey(e => e.EventType)
                .HasName("PK_EventTypes");

            entity.ToTable("T_EventTypes");

            entity.HasIndex(e => e.EventType, "IND_EventTypes").IsUnique();

            entity.Property(e => e.EventType).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Icon)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Events>(entity =>
        {
            entity.HasKey(e => new { e.EventDate, e.CardNumber, e.DoorNumber, e.EventType, e.ReaderId, e.EventId })
                .HasName("PK_Events");

            entity.ToTable("T_Events");

            entity.HasIndex(e => e.EventDate, "IND_EventDate");

            entity.HasIndex(e => e.EventId, "IND_EventID").IsUnique();

            entity.Property(e => e.EventDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
            entity.Property(e => e.EventId)
                .ValueGeneratedOnAdd()
                .HasColumnName("EventID");
            entity.Property(e => e.ActualCardId)
                .HasMaxLength(8)
                .HasColumnName("ActualCardID");
            entity.Property(e => e.AlarmId).HasColumnName("AlarmID");
        });

        modelBuilder.Entity<FloorPlans>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_FloorPlans");

            entity.ToTable("T_FloorPlans");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<IoControllerInput>(entity =>
        {
            entity.HasKey(e => new { e.ControllerId, e.IoinputIndex })
                .HasName("PK_IOController_Details");

            entity.ToTable("T_IOController_Details");

            entity.Property(e => e.ControllerId).HasColumnName("ControllerID");
            entity.Property(e => e.IoinputIndex).HasColumnName("IOInputIndex");
            entity.Property(e => e.InputName).HasMaxLength(30);
            entity.Property(e => e.OutputName).HasMaxLength(30);
        });

        modelBuilder.Entity<IoController>(entity =>
        {
            entity.HasKey(e => e.ControllerId)
                .HasName("PK_IOController_Header");

            entity.ToTable("T_IOController_Header");

            entity.Property(e => e.ControllerId)
                .ValueGeneratedNever()
                .HasColumnName("ControllerID");
            entity.Property(e => e.ControllerDay).HasMaxLength(4);
            entity.Property(e => e.ControllerVersion).HasMaxLength(4);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(15)
                .HasColumnName("IPAddress");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Rtcdate)
                .HasMaxLength(12)
                .HasColumnName("RTCDate");
            entity.Property(e => e.Rtctime)
                .HasMaxLength(12)
                .HasColumnName("RTCTime");
        });

        modelBuilder.Entity<CardholderAccessLevel>(entity =>
        {
            entity.HasKey(e => new { e.CardNumber, e.Site, e.Level })
                .HasName("PK_Name_AccessLevels");

            entity.ToTable("T_Name_AccessLevels");

            entity.Property(e => e.Modified).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<CardholderCustomFields>(entity =>
        {
            entity.HasKey(e => e.CardNumber)
                .HasName("PK_Name_CustomFields");

            entity.ToTable("T_Name_CustomFields");

            entity.Property(e => e.CardNumber).ValueGeneratedNever();
            entity.Property(e => e.Custom1).HasMaxLength(30);
            entity.Property(e => e.Custom10).HasMaxLength(30);
            entity.Property(e => e.Custom11).HasMaxLength(30);
            entity.Property(e => e.Custom12).HasMaxLength(30);
            entity.Property(e => e.Custom13).HasMaxLength(30);
            entity.Property(e => e.Custom14).HasMaxLength(30);
            entity.Property(e => e.Custom15).HasMaxLength(30);
            entity.Property(e => e.Custom16).HasMaxLength(30);
            entity.Property(e => e.Custom17).HasMaxLength(30);
            entity.Property(e => e.Custom18).HasMaxLength(30);
            entity.Property(e => e.Custom19).HasMaxLength(30);
            entity.Property(e => e.Custom2).HasMaxLength(30);
            entity.Property(e => e.Custom20).HasMaxLength(30);
            entity.Property(e => e.Custom21).HasMaxLength(30);
            entity.Property(e => e.Custom22).HasMaxLength(30);
            entity.Property(e => e.Custom23).HasMaxLength(30);
            entity.Property(e => e.Custom24).HasMaxLength(30);
            entity.Property(e => e.Custom25).HasMaxLength(30);
            entity.Property(e => e.Custom3).HasMaxLength(30);
            entity.Property(e => e.Custom4).HasMaxLength(30);
            entity.Property(e => e.Custom5).HasMaxLength(30);
            entity.Property(e => e.Custom6).HasMaxLength(30);
            entity.Property(e => e.Custom7).HasMaxLength(30);
            entity.Property(e => e.Custom8).HasMaxLength(30);
            entity.Property(e => e.Custom9).HasMaxLength(30);
        });

        modelBuilder.Entity<Cardholder>(entity =>
        {
            entity.HasKey(e => e.CardNumber)
                .HasName("PK_Name_Header");

            entity.ToTable("T_Name_Header");

            entity.HasIndex(e => e.CardNumber, "IND_Name").IsUnique();

            entity.Property(e => e.CardNumber).ValueGeneratedNever();
            entity.Property(e => e.Apbdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("APBDate");
            entity.Property(e => e.Apbnumber).HasColumnName("APBNumber");
            entity.Property(e => e.CardId)
                .HasMaxLength(8)
                .HasColumnName("CardID");
            entity.Property(e => e.Forname).HasMaxLength(30);
            entity.Property(e => e.HotStamp).HasMaxLength(40);
            entity.Property(e => e.IdcardDesign)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IDCardDesign");
            entity.Property(e => e.LastDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Modified).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Pin)
                .HasMaxLength(8)
                .HasColumnName("PIN");
            entity.Property(e => e.Surname).HasMaxLength(30);
            entity.Property(e => e.ValidFrom).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ValidTo).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<Sites>(entity =>
        {
            entity.HasKey(e => e.Site)
                .HasName("PK_Sites");

            entity.ToTable("T_Sites");

            entity.HasIndex(e => e.Site, "IND_Site").IsUnique();

            entity.Property(e => e.Site).ValueGeneratedNever();
            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<SpaceZoneAttendance>(entity =>
        {
            entity.HasKey(e => new { e.ZoneNumber, e.CardIndex, e.DateandTime })
                .HasName("PK_SpaceZone_Attendance");

            entity.ToTable("T_SpaceZone_Attendance");

            entity.Property(e => e.DateandTime).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Keyf).HasMaxLength(60);
        });

        modelBuilder.Entity<SpaceZoneCardholder>(entity =>
        {
            entity.HasKey(e => new { e.SpaceZone, e.CardNumber })
                .HasName("PK_SpaceZone_Cardholders");

            entity.ToTable("T_SpaceZone_Cardholders");
        });

        modelBuilder.Entity<SpaceZoneDoor>(entity =>
        {
            entity.HasKey(e => new { e.Door, e.Site, e.Zone })
                .HasName("PK_SpaceZone_Details");

            entity.ToTable("T_SpaceZone_Details");

            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<SpaceZone>(entity =>
        {
            entity.HasKey(e => e.ZoneNumber)
                .HasName("PK_SpaceZone_Header");

            entity.ToTable("T_SpaceZone_Header");

            entity.Property(e => e.ZoneNumber).ValueGeneratedNever();
            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Report).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<TimeSheet>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_TimeSheet_Header");

            entity.ToTable("T_TimeSheet_Header");

            entity.Property(e => e.CardId)
                .HasMaxLength(8)
                .HasColumnName("CardID");
            entity.Property(e => e.CardIdpageBreak).HasColumnName("CardIDPageBreak");
            entity.Property(e => e.Custom1).HasMaxLength(30);
            entity.Property(e => e.Custom10).HasMaxLength(30);
            entity.Property(e => e.Custom11).HasMaxLength(30);
            entity.Property(e => e.Custom12).HasMaxLength(30);
            entity.Property(e => e.Custom13).HasMaxLength(30);
            entity.Property(e => e.Custom14).HasMaxLength(30);
            entity.Property(e => e.Custom15).HasMaxLength(30);
            entity.Property(e => e.Custom16).HasMaxLength(30);
            entity.Property(e => e.Custom17).HasMaxLength(30);
            entity.Property(e => e.Custom18).HasMaxLength(30);
            entity.Property(e => e.Custom19).HasMaxLength(30);
            entity.Property(e => e.Custom2).HasMaxLength(30);
            entity.Property(e => e.Custom20).HasMaxLength(30);
            entity.Property(e => e.Custom21).HasMaxLength(30);
            entity.Property(e => e.Custom22).HasMaxLength(30);
            entity.Property(e => e.Custom23).HasMaxLength(30);
            entity.Property(e => e.Custom24).HasMaxLength(30);
            entity.Property(e => e.Custom25).HasMaxLength(30);
            entity.Property(e => e.Custom3).HasMaxLength(30);
            entity.Property(e => e.Custom4).HasMaxLength(30);
            entity.Property(e => e.Custom5).HasMaxLength(30);
            entity.Property(e => e.Custom6).HasMaxLength(30);
            entity.Property(e => e.Custom7).HasMaxLength(30);
            entity.Property(e => e.Custom8).HasMaxLength(30);
            entity.Property(e => e.Custom9).HasMaxLength(30);
            entity.Property(e => e.DateFrom).HasColumnType("timestamp without time zone");
            entity.Property(e => e.DateTo).HasColumnType("timestamp without time zone");
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.LastName).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Rollover).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<TimeSheetZone>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Zone })
                .HasName("PK_TimeSheet_Zones");

            entity.ToTable("T_TimeSheet_Zones");
        });

        modelBuilder.Entity<TimeZoneInterval>(entity =>
        {
            entity.HasKey(e => new { e.TimeZone, e.Sequence })
                .HasName("PK_TimeZone_Details");

            entity.ToTable("T_TimeZone_Details");

            entity.Property(e => e.EndTime).HasMaxLength(10);
            entity.Property(e => e.StartTime).HasMaxLength(10);
        });

        modelBuilder.Entity<TimeZones>(entity =>
        {
            entity.HasKey(e => new { e.TimeZone, e.Site })
                .HasName("PK_TimeZone_Header");

            entity.ToTable("T_TimeZone_Header");

            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TriggerController>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.ControllerCode, e.InputIndex })
                .HasName("PK_Triggers_Controllers");

            entity.ToTable("T_Triggers_Controllers");
        });

        modelBuilder.Entity<TriggerEvent>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.EventType })
                .HasName("PK_Triggers_Events");

            entity.ToTable("T_Triggers_Events");
        });

        modelBuilder.Entity<Trigger>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_Triggers_Header");

            entity.ToTable("T_Triggers_Header");

            entity.Property(e => e.AlarmText).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.OutputSim).HasColumnName("OutputSIM");
            entity.Property(e => e.PopulationSim).HasColumnName("PopulationSIM");
            entity.Property(e => e.RelayBdoor).HasColumnName("RelayBDoor");
            entity.Property(e => e.ResetRelayBperiod).HasColumnName("ResetRelayBPeriod");
        });

        modelBuilder.Entity<UserPermissions>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Area })
                .HasName("PK_User_Permissions");

            entity.ToTable("T_User_Permissions");

            entity.Property(e => e.Area)
                .HasMaxLength(3)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserSites>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Site })
                .HasName("PK_User_Sites");

            entity.ToTable("T_User_Sites");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            // T_Users has a real identity PK (PK_T_Users on Code); keep the model keyed so the
            // entity can be tracked for insert/update/delete (the Users & Passwords page CRUD and
            // AuthService.ChangePasswordAsync both rely on this).
            entity.HasKey(e => e.Code).HasName("PK_T_Users");

            entity.ToTable("T_Users");

            entity.Property(e => e.Code).ValueGeneratedOnAdd();
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasColumnType("text"); // widened from legacy varchar(50) to hold BCrypt hashes

            // These permission columns don't exist in a legacy DoorsEnterprise schema, so a legacy
            // restore COPYs the users table WITHOUT them. Give each a DB default so those inserts
            // succeed (NOT NULL would otherwise be violated): no access (0) / not-Super (false). The
            // legacy importer then promotes every imported user to full access — see
            // LegacyBackupService.GrantFullAccessToAllUsers, which sets these to 2 / true.
            entity.Property(e => e.Administrator).HasDefaultValue(false);
            entity.Property(e => e.CardManagerAccess).HasDefaultValue(0);
            entity.Property(e => e.SiteSettingsAccess).HasDefaultValue(0);
            entity.Property(e => e.UserSettingsAccess).HasDefaultValue(0);
            entity.Property(e => e.ReportsAccess).HasDefaultValue(0);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
