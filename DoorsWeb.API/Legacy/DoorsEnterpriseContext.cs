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

    public virtual DbSet<TAccessLevelDetails> TAccessLevelDetails { get; set; }

    public virtual DbSet<TAccessLevelHeader> TAccessLevelHeader { get; set; }

    public virtual DbSet<TAlarms> TAlarms { get; set; }

    public virtual DbSet<TApbzoneDetails> TApbzoneDetails { get; set; }

    public virtual DbSet<TApbzoneHeader> TApbzoneHeader { get; set; }

    public virtual DbSet<TArcCustom> TArcCustom { get; set; }

    public virtual DbSet<TArcCustomFieldTypes> TArcCustomFieldTypes { get; set; }

    public virtual DbSet<TArcDoors> TArcDoors { get; set; }

    public virtual DbSet<TArcEvents> TArcEvents { get; set; }

    public virtual DbSet<TArcNameCustomFields> TArcNameCustomFields { get; set; }

    public virtual DbSet<TArcNameHeader> TArcNameHeader { get; set; }

    public virtual DbSet<TArcSites> TArcSites { get; set; }

    public virtual DbSet<TAudit> TAudit { get; set; }

    public virtual DbSet<TBackup> TBackup { get; set; }

    public virtual DbSet<TBioData> TBioData { get; set; }

    public virtual DbSet<TCalendarDetails> TCalendarDetails { get; set; }

    public virtual DbSet<TCalendarHeader> TCalendarHeader { get; set; }

    public virtual DbSet<TCardDesignDetails> TCardDesignDetails { get; set; }

    public virtual DbSet<TCardDesignHeader> TCardDesignHeader { get; set; }

    public virtual DbSet<TCardManagerDefault> TCardManagerDefault { get; set; }

    public virtual DbSet<TCardManagerHeader> TCardManagerHeader { get; set; }

    public virtual DbSet<TCardManagerOrderByFields> TCardManagerOrderByFields { get; set; }

    public virtual DbSet<TCardManagerSelectFields> TCardManagerSelectFields { get; set; }

    public virtual DbSet<TCardManagerWhereFields> TCardManagerWhereFields { get; set; }

    public virtual DbSet<TCardPackDetails> TCardPackDetails { get; set; }

    public virtual DbSet<TCardPackHeader> TCardPackHeader { get; set; }

    public virtual DbSet<TCommands> TCommands { get; set; }

    public virtual DbSet<TConnectors> TConnectors { get; set; }

    public virtual DbSet<TCustom> TCustom { get; set; }

    public virtual DbSet<TCustomFieldTypes> TCustomFieldTypes { get; set; }

    public virtual DbSet<TCustomer> TCustomer { get; set; }

    public virtual DbSet<TDisplay> TDisplay { get; set; }

    public virtual DbSet<TDisplayTypes> TDisplayTypes { get; set; }

    public virtual DbSet<TDoorTechnology> TDoorTechnology { get; set; }

    public virtual DbSet<TDoors> TDoors { get; set; }

    public virtual DbSet<TEnterpriseData> TEnterpriseData { get; set; }

    public virtual DbSet<TEventTypes> TEventTypes { get; set; }

    public virtual DbSet<TEvents> TEvents { get; set; }

    public virtual DbSet<TFloorPlans> TFloorPlans { get; set; }

    public virtual DbSet<TIocontrollerDetails> TIocontrollerDetails { get; set; }

    public virtual DbSet<TIocontrollerHeader> TIocontrollerHeader { get; set; }

    public virtual DbSet<TModems> TModems { get; set; }

    public virtual DbSet<TNameAccessLevels> TNameAccessLevels { get; set; }

    public virtual DbSet<TNameCustomFields> TNameCustomFields { get; set; }

    public virtual DbSet<TNameHeader> TNameHeader { get; set; }

    public virtual DbSet<TSites> TSites { get; set; }

    public virtual DbSet<TSpaceZoneAttendance> TSpaceZoneAttendance { get; set; }

    public virtual DbSet<TSpaceZoneCardholders> TSpaceZoneCardholders { get; set; }

    public virtual DbSet<TSpaceZoneDetails> TSpaceZoneDetails { get; set; }

    public virtual DbSet<TSpaceZoneHeader> TSpaceZoneHeader { get; set; }

    public virtual DbSet<TStatusView> TStatusView { get; set; }

    public virtual DbSet<TSystem> TSystem { get; set; }

    public virtual DbSet<TTimeSheetHeader> TTimeSheetHeader { get; set; }

    public virtual DbSet<TTimeSheetZones> TTimeSheetZones { get; set; }

    public virtual DbSet<TTimeZoneDetails> TTimeZoneDetails { get; set; }

    public virtual DbSet<TTimeZoneHeader> TTimeZoneHeader { get; set; }

    public virtual DbSet<TTriggersControllers> TTriggersControllers { get; set; }

    public virtual DbSet<TTriggersEvents> TTriggersEvents { get; set; }

    public virtual DbSet<TTriggersHeader> TTriggersHeader { get; set; }

    public virtual DbSet<TUserPermissions> TUserPermissions { get; set; }

    public virtual DbSet<TUserSites> TUserSites { get; set; }

    public virtual DbSet<TUserlog> TUserlog { get; set; }

    public virtual DbSet<TUsers> TUsers { get; set; }

    public virtual DbSet<VNameAccessLevels> VNameAccessLevels { get; set; }

    public virtual DbSet<VNameBlankAccessLevels> VNameBlankAccessLevels { get; set; }

    public virtual DbSet<VNameHeader> VNameHeader { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TAccessLevelDetails>(entity =>
        {
            entity.HasKey(e => new { e.Level, e.Door })
                .HasName("PK_AccessLevel_Details")
                .IsClustered(false);

            entity.ToTable("T_AccessLevel_Details");
        });

        modelBuilder.Entity<TAccessLevelHeader>(entity =>
        {
            entity.HasKey(e => new { e.AccessLevel, e.Site })
                .HasName("PK_AccessLevel_Header")
                .IsClustered(false);

            entity.ToTable("T_AccessLevel_Header");

            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TAlarms>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_Alarms")
                .IsClustered(false);

            entity.ToTable("T_Alarms");

            entity.Property(e => e.Code).ValueGeneratedNever();
            entity.Property(e => e.ActionedBy).HasMaxLength(20);
            entity.Property(e => e.ActionedDate).HasColumnType("datetime");
            entity.Property(e => e.ActionedText).HasMaxLength(255);
            entity.Property(e => e.AlarmDate).HasColumnType("datetime");
            entity.Property(e => e.AlarmDescription).HasMaxLength(50);
        });

        modelBuilder.Entity<TApbzoneDetails>(entity =>
        {
            entity.HasKey(e => new { e.Apbnumber, e.DoorNumber })
                .HasName("PK_APBZone_Details")
                .IsClustered(false);

            entity.ToTable("T_APBZone_Details");

            entity.Property(e => e.Apbnumber).HasColumnName("APBNumber");
            entity.Property(e => e.Key).HasMaxLength(10);
        });

        modelBuilder.Entity<TApbzoneHeader>(entity =>
        {
            entity.HasKey(e => e.Apbnumber)
                .HasName("PK_APBZone_Header")
                .IsClustered(false);

            entity.ToTable("T_APBZone_Header");

            entity.Property(e => e.Apbnumber)
                .ValueGeneratedNever()
                .HasColumnName("APBNumber");
            entity.Property(e => e.Apbmode).HasColumnName("APBMode");
            entity.Property(e => e.DiscoveryModeExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.DiscoveryModeStart).HasColumnType("datetime");
            entity.Property(e => e.Key).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.NextAutoLogout).HasColumnType("datetime");
        });

        modelBuilder.Entity<TArcCustom>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_Arc_Custom");

            entity.Property(e => e.Description).HasMaxLength(30);
        });

        modelBuilder.Entity<TArcCustomFieldTypes>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_Arc_CustomFieldTypes");

            entity.Property(e => e.Literal).HasMaxLength(30);
        });

        modelBuilder.Entity<TArcDoors>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_Arc_Doors");

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
            entity.Property(e => e.LastDate).HasColumnType("datetime");
            entity.Property(e => e.LockDriveMode).HasColumnName("Lock_Drive_Mode");
            entity.Property(e => e.Modified).HasColumnType("datetime");
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
            entity.Property(e => e.Updated).HasColumnType("datetime");
            entity.Property(e => e.ValidFromTimeHh).HasColumnName("ValidFromTimeHH");
            entity.Property(e => e.ValidFromTimeMm).HasColumnName("ValidFromTimeMM");
            entity.Property(e => e.ValidToTimeHh).HasColumnName("ValidToTimeHH");
            entity.Property(e => e.ValidToTimeMm).HasColumnName("ValidToTimeMM");
            entity.Property(e => e.Xplace).HasColumnName("XPlace");
            entity.Property(e => e.Yplace).HasColumnName("YPlace");
        });

        modelBuilder.Entity<TArcEvents>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_Arc_Events");

            entity.Property(e => e.ActualCardId)
                .HasMaxLength(8)
                .HasColumnName("ActualCardID");
            entity.Property(e => e.AlarmId).HasColumnName("AlarmID");
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.EventId)
                .ValueGeneratedOnAdd()
                .HasColumnName("EventID");
            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
        });

        modelBuilder.Entity<TArcNameCustomFields>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_Arc_Name_CustomFields");

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

        modelBuilder.Entity<TArcNameHeader>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_Arc_Name_Header");

            entity.Property(e => e.Apbdate)
                .HasColumnType("datetime")
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
            entity.Property(e => e.LastDate).HasColumnType("datetime");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.OldCardId)
                .HasMaxLength(8)
                .HasColumnName("OldCardID");
            entity.Property(e => e.Pin)
                .HasMaxLength(8)
                .HasColumnName("PIN");
            entity.Property(e => e.Surname).HasMaxLength(30);
            entity.Property(e => e.ValidFrom).HasColumnType("datetime");
            entity.Property(e => e.ValidTo).HasColumnType("datetime");
        });

        modelBuilder.Entity<TArcSites>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_Arc_Sites");

            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<TAudit>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("T_Audit");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccessLevels)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.CardId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("CardID");
            entity.Property(e => e.Forename)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.SaveDate).HasColumnType("datetime");
            entity.Property(e => e.SavedBy)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.Workstation)
                .HasMaxLength(60)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TBackup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_Backup");

            entity.Property(e => e.LastBackup).HasColumnType("datetime");
            entity.Property(e => e.ScheduleTime).HasMaxLength(8);
        });

        modelBuilder.Entity<TBioData>(entity =>
        {
            entity.HasKey(e => e.Slot)
                .HasName("PK_Bio_Data")
                .IsClustered(false);

            entity.ToTable("T_Bio_Data");

            entity.HasIndex(e => e.Slot, "IND_Slot").IsUnique();

            entity.Property(e => e.Slot).ValueGeneratedNever();
            entity.Property(e => e.BioChecksumLeft).HasColumnName("BioChecksum_Left");
            entity.Property(e => e.BioChecksumRight).HasColumnName("BioChecksum_Right");
            entity.Property(e => e.BioTemplateLeft)
                .HasMaxLength(4000)
                .HasColumnName("Bio_Template_Left");
            entity.Property(e => e.BioTemplateRight)
                .HasMaxLength(4000)
                .HasColumnName("Bio_Template_Right");
            entity.Property(e => e.FingerLeft)
                .HasMaxLength(1)
                .HasColumnName("Finger_Left");
            entity.Property(e => e.FingerRight)
                .HasMaxLength(1)
                .HasColumnName("Finger_Right");
            entity.Property(e => e.Id)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Status).HasMaxLength(1);
        });

        modelBuilder.Entity<TCalendarDetails>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.ExceptionDate })
                .HasName("PK_Calendar_Details")
                .IsClustered(false);

            entity.ToTable("T_Calendar_Details");

            entity.Property(e => e.ExceptionDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TCalendarHeader>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_Calendar_Header")
                .IsClustered(false);

            entity.ToTable("T_Calendar_Header");

            entity.Property(e => e.Description).HasMaxLength(30);
        });

        modelBuilder.Entity<TCardDesignDetails>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Sequence })
                .HasName("PK_CardDesign_Details")
                .IsClustered(false);

            entity.ToTable("T_CardDesign_Details");

            entity.Property(e => e.FontName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.FontSize)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Text)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TCardDesignHeader>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_CardDesign_Header")
                .IsClustered(false);

            entity.ToTable("T_CardDesign_Header", tb => tb.HasTrigger("trgCardDesign"));

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TCardManagerDefault>(entity =>
        {
            entity.HasKey(e => e.User)
                .HasName("PK_CardManager_Default")
                .IsClustered(false);

            entity.ToTable("T_CardManager_Default");

            entity.Property(e => e.User).ValueGeneratedNever();
        });

        modelBuilder.Entity<TCardManagerHeader>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_CardManager_Header")
                .IsClustered(false);

            entity.ToTable("T_CardManager_Header");

            entity.Property(e => e.Code).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(30);
            entity.Property(e => e.Printer).HasMaxLength(30);
        });

        modelBuilder.Entity<TCardManagerOrderByFields>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.SortNumber })
                .HasName("PK_CardManager_OrderByFields")
                .IsClustered(false);

            entity.ToTable("T_CardManager_OrderByFields");

            entity.Property(e => e.FieldName).HasMaxLength(30);
            entity.Property(e => e.TableName).HasMaxLength(30);
        });

        modelBuilder.Entity<TCardManagerSelectFields>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Position })
                .HasName("PK_CardManager_SelectFields")
                .IsClustered(false);

            entity.ToTable("T_CardManager_SelectFields");

            entity.Property(e => e.FieldName).HasMaxLength(30);
            entity.Property(e => e.TableName).HasMaxLength(30);
        });

        modelBuilder.Entity<TCardManagerWhereFields>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Sequence })
                .HasName("PK_CardManager_WhereFields")
                .IsClustered(false);

            entity.ToTable("T_CardManager_WhereFields");

            entity.Property(e => e.FieldName).HasMaxLength(30);
            entity.Property(e => e.Operator).HasMaxLength(10);
            entity.Property(e => e.TableName).HasMaxLength(30);
            entity.Property(e => e.Value1).HasMaxLength(30);
            entity.Property(e => e.Value2).HasMaxLength(30);
        });

        modelBuilder.Entity<TCardPackDetails>(entity =>
        {
            entity.HasKey(e => new { e.Site, e.CardPack })
                .HasName("PK_CardPack_Details")
                .IsClustered(false);

            entity.ToTable("T_CardPack_Details");
        });

        modelBuilder.Entity<TCardPackHeader>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_CardPack_Header")
                .IsClustered(false);

            entity.ToTable("T_CardPack_Header");

            entity.Property(e => e.FirstCardId)
                .HasMaxLength(8)
                .HasColumnName("FirstCardID");
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TCommands>(entity =>
        {
            entity.HasKey(e => e.CommandId)
                .HasName("PK_Commands")
                .IsClustered(false);

            entity.ToTable("T_Commands");

            entity.HasIndex(e => e.ControllerId, "IND_ControllerID");

            entity.Property(e => e.CommandId).HasColumnName("CommandID");
            entity.Property(e => e.ControllerId)
                .HasMaxLength(12)
                .HasColumnName("ControllerID");
            entity.Property(e => e.Data).HasMaxLength(100);
            entity.Property(e => e.NewId)
                .HasMaxLength(12)
                .HasColumnName("NewID");
            entity.Property(e => e.OldId)
                .HasMaxLength(12)
                .HasColumnName("OldID");
            entity.Property(e => e.ValidFrom).HasMaxLength(12);
            entity.Property(e => e.ValidTo).HasMaxLength(12);
        });

        modelBuilder.Entity<TConnectors>(entity =>
        {
            entity.HasKey(e => e.Connector)
                .HasName("PK_Connectors")
                .IsClustered(false);

            entity.ToTable("T_Connectors");

            entity.Property(e => e.Connector).ValueGeneratedNever();
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(30)
                .HasColumnName("IPAddress");
            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.ModemConnectionTime).HasMaxLength(5);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Pabx)
                .HasMaxLength(30)
                .HasColumnName("PABX");
            entity.Property(e => e.Path).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(10);
            entity.Property(e => e.Telnumber).HasMaxLength(30);
        });

        modelBuilder.Entity<TCustom>(entity =>
        {
            entity.HasKey(e => new { e.CustomFieldCode, e.Code })
                .HasName("PK_Custom")
                .IsClustered(false);

            entity.ToTable("T_Custom");

            entity.Property(e => e.Description).HasMaxLength(30);
        });

        modelBuilder.Entity<TCustomFieldTypes>(entity =>
        {
            entity.HasKey(e => e.CustomField)
                .HasName("PK_CustomFieldTypes")
                .IsClustered(false);

            entity.ToTable("T_CustomFieldTypes");

            entity.Property(e => e.CustomField).ValueGeneratedNever();
            entity.Property(e => e.Literal).HasMaxLength(30);
        });

        modelBuilder.Entity<TCustomer>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_Customer");

            entity.Property(e => e.Comments)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CommissionDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerAddress1).HasMaxLength(30);
            entity.Property(e => e.CustomerAddress2).HasMaxLength(30);
            entity.Property(e => e.CustomerAddress3).HasMaxLength(30);
            entity.Property(e => e.CustomerAddress4).HasMaxLength(30);
            entity.Property(e => e.CustomerCompany).HasMaxLength(30);
            entity.Property(e => e.CustomerContact).HasMaxLength(30);
            entity.Property(e => e.CustomerCountry).HasMaxLength(30);
            entity.Property(e => e.CustomerFax).HasMaxLength(30);
            entity.Property(e => e.CustomerPostCode).HasMaxLength(10);
            entity.Property(e => e.CustomerTelephone).HasMaxLength(30);
            entity.Property(e => e.Customeremail).HasMaxLength(50);
            entity.Property(e => e.InstallationDate).HasColumnType("datetime");
            entity.Property(e => e.InstallerAddress1).HasMaxLength(30);
            entity.Property(e => e.InstallerAddress2).HasMaxLength(30);
            entity.Property(e => e.InstallerAddress3).HasMaxLength(30);
            entity.Property(e => e.InstallerAddress4).HasMaxLength(30);
            entity.Property(e => e.InstallerCompany).HasMaxLength(30);
            entity.Property(e => e.InstallerContact).HasMaxLength(30);
            entity.Property(e => e.InstallerCountry).HasMaxLength(30);
            entity.Property(e => e.InstallerFax).HasMaxLength(30);
            entity.Property(e => e.InstallerPostCode).HasMaxLength(10);
            entity.Property(e => e.InstallerTelephone).HasMaxLength(30);
            entity.Property(e => e.Installeremail).HasMaxLength(50);
            entity.Property(e => e.ProductKey).HasMaxLength(9);
        });

        modelBuilder.Entity<TDisplay>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Position })
                .HasName("PK_Display")
                .IsClustered(false);

            entity.ToTable("T_Display");

            entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
        });

        modelBuilder.Entity<TDisplayTypes>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.PropertyId })
                .HasName("PK_DisplayTypes")
                .IsClustered(false);

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

        modelBuilder.Entity<TDoorTechnology>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_Door_Technology")
                .IsClustered(false);

            entity.ToTable("T_Door_Technology");

            entity.Property(e => e.Code).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(30);
        });

        modelBuilder.Entity<TDoors>(entity =>
        {
            entity.HasKey(e => e.Door)
                .HasName("PK_Doors")
                .IsClustered(false);

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
            entity.Property(e => e.LastDate).HasColumnType("datetime");
            entity.Property(e => e.LockDriveMode).HasColumnName("Lock_Drive_Mode");
            entity.Property(e => e.Modified).HasColumnType("datetime");
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
            entity.Property(e => e.Updated).HasColumnType("datetime");
            entity.Property(e => e.ValidFromTimeHh).HasColumnName("ValidFromTimeHH");
            entity.Property(e => e.ValidFromTimeMm).HasColumnName("ValidFromTimeMM");
            entity.Property(e => e.ValidToTimeHh).HasColumnName("ValidToTimeHH");
            entity.Property(e => e.ValidToTimeMm).HasColumnName("ValidToTimeMM");
            entity.Property(e => e.Xplace).HasColumnName("XPlace");
            entity.Property(e => e.Yplace).HasColumnName("YPlace");
        });

        modelBuilder.Entity<TEnterpriseData>(entity =>
        {
            entity.HasKey(e => new { e.Property, e.Value })
                .HasName("PK_EnterpriseData")
                .IsClustered(false);

            entity.ToTable("T_EnterpriseData");

            entity.Property(e => e.Property).HasMaxLength(30);
            entity.Property(e => e.Value).HasMaxLength(50);
        });

        modelBuilder.Entity<TEventTypes>(entity =>
        {
            entity.HasKey(e => e.EventType)
                .HasName("PK_EventTypes")
                .IsClustered(false);

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

        modelBuilder.Entity<TEvents>(entity =>
        {
            entity.HasKey(e => new { e.EventDate, e.CardNumber, e.DoorNumber, e.EventType, e.ReaderId, e.EventId })
                .HasName("PK_Events")
                .IsClustered(false);

            entity.ToTable("T_Events");

            entity.HasIndex(e => e.EventDate, "IND_EventDate");

            entity.HasIndex(e => e.EventId, "IND_EventID").IsUnique();

            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
            entity.Property(e => e.EventId)
                .ValueGeneratedOnAdd()
                .HasColumnName("EventID");
            entity.Property(e => e.ActualCardId)
                .HasMaxLength(8)
                .HasColumnName("ActualCardID");
            entity.Property(e => e.AlarmId).HasColumnName("AlarmID");
        });

        modelBuilder.Entity<TFloorPlans>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_FloorPlans")
                .IsClustered(false);

            entity.ToTable("T_FloorPlans");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TIocontrollerDetails>(entity =>
        {
            entity.HasKey(e => new { e.ControllerId, e.IoinputIndex })
                .HasName("PK_IOController_Details")
                .IsClustered(false);

            entity.ToTable("T_IOController_Details");

            entity.Property(e => e.ControllerId).HasColumnName("ControllerID");
            entity.Property(e => e.IoinputIndex).HasColumnName("IOInputIndex");
            entity.Property(e => e.InputName).HasMaxLength(30);
            entity.Property(e => e.OutputName).HasMaxLength(30);
        });

        modelBuilder.Entity<TIocontrollerHeader>(entity =>
        {
            entity.HasKey(e => e.ControllerId)
                .HasName("PK_IOController_Header")
                .IsClustered(false);

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

        modelBuilder.Entity<TModems>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_Modems")
                .IsClustered(false);

            entity.ToTable("T_Modems");

            entity.Property(e => e.Code).ValueGeneratedNever();
            entity.Property(e => e.Comport).HasColumnName("COMPort");
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<TNameAccessLevels>(entity =>
        {
            entity.HasKey(e => new { e.CardNumber, e.Site, e.Level })
                .HasName("PK_Name_AccessLevels")
                .IsClustered(false);

            entity.ToTable("T_Name_AccessLevels");

            entity.Property(e => e.Modified).HasColumnType("datetime");
        });

        modelBuilder.Entity<TNameCustomFields>(entity =>
        {
            entity.HasKey(e => e.CardNumber)
                .HasName("PK_Name_CustomFields")
                .IsClustered(false);

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

        modelBuilder.Entity<TNameHeader>(entity =>
        {
            entity.HasKey(e => e.CardNumber)
                .HasName("PK_Name_Header")
                .IsClustered(false);

            entity.ToTable("T_Name_Header");

            entity.HasIndex(e => e.CardNumber, "IND_Name").IsUnique();

            entity.Property(e => e.CardNumber).ValueGeneratedNever();
            entity.Property(e => e.Apbdate)
                .HasColumnType("datetime")
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
            entity.Property(e => e.LastDate).HasColumnType("datetime");
            entity.Property(e => e.Modified).HasColumnType("datetime");
            entity.Property(e => e.OldCardId)
                .HasMaxLength(8)
                .HasColumnName("OldCardID");
            entity.Property(e => e.Pin)
                .HasMaxLength(8)
                .HasColumnName("PIN");
            entity.Property(e => e.Surname).HasMaxLength(30);
            entity.Property(e => e.ValidFrom).HasColumnType("datetime");
            entity.Property(e => e.ValidTo).HasColumnType("datetime");
        });

        modelBuilder.Entity<TSites>(entity =>
        {
            entity.HasKey(e => e.Site)
                .HasName("PK_Sites")
                .IsClustered(false);

            entity.ToTable("T_Sites");

            entity.HasIndex(e => e.Site, "IND_Site").IsUnique();

            entity.Property(e => e.Site).ValueGeneratedNever();
            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<TSpaceZoneAttendance>(entity =>
        {
            entity.HasKey(e => new { e.ZoneNumber, e.CardIndex, e.DateandTime })
                .HasName("PK_SpaceZone_Attendance")
                .IsClustered(false);

            entity.ToTable("T_SpaceZone_Attendance");

            entity.Property(e => e.DateandTime).HasColumnType("datetime");
            entity.Property(e => e.Keyf).HasMaxLength(60);
        });

        modelBuilder.Entity<TSpaceZoneCardholders>(entity =>
        {
            entity.HasKey(e => new { e.SpaceZone, e.CardNumber })
                .HasName("PK_SpaceZone_Cardholders")
                .IsClustered(false);

            entity.ToTable("T_SpaceZone_Cardholders");
        });

        modelBuilder.Entity<TSpaceZoneDetails>(entity =>
        {
            entity.HasKey(e => new { e.Door, e.Site, e.Zone })
                .HasName("PK_SpaceZone_Details")
                .IsClustered(false);

            entity.ToTable("T_SpaceZone_Details");

            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<TSpaceZoneHeader>(entity =>
        {
            entity.HasKey(e => e.ZoneNumber)
                .HasName("PK_SpaceZone_Header")
                .IsClustered(false);

            entity.ToTable("T_SpaceZone_Header");

            entity.Property(e => e.ZoneNumber).ValueGeneratedNever();
            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Report).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<TStatusView>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_StatusView")
                .IsClustered(false);

            entity.ToTable("T_StatusView");

            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TSystem>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_System");

            entity.Property(e => e.Corporate1000Code)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.CurrentVersion).HasMaxLength(10);
        });

        modelBuilder.Entity<TTimeSheetHeader>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_TimeSheet_Header")
                .IsClustered(false);

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
            entity.Property(e => e.DateFrom).HasColumnType("datetime");
            entity.Property(e => e.DateTo).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.LastName).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Rollover).HasColumnType("datetime");
        });

        modelBuilder.Entity<TTimeSheetZones>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Zone })
                .HasName("PK_TimeSheet_Zones")
                .IsClustered(false);

            entity.ToTable("T_TimeSheet_Zones");
        });

        modelBuilder.Entity<TTimeZoneDetails>(entity =>
        {
            entity.HasKey(e => new { e.TimeZone, e.Sequence })
                .HasName("PK_TimeZone_Details")
                .IsClustered(false);

            entity.ToTable("T_TimeZone_Details");

            entity.Property(e => e.EndTime).HasMaxLength(10);
            entity.Property(e => e.StartTime).HasMaxLength(10);
        });

        modelBuilder.Entity<TTimeZoneHeader>(entity =>
        {
            entity.HasKey(e => new { e.TimeZone, e.Site })
                .HasName("PK_TimeZone_Header")
                .IsClustered(false);

            entity.ToTable("T_TimeZone_Header");

            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TTriggersControllers>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.ControllerCode, e.InputIndex })
                .HasName("PK_Triggers_Controllers")
                .IsClustered(false);

            entity.ToTable("T_Triggers_Controllers");
        });

        modelBuilder.Entity<TTriggersEvents>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.EventType })
                .HasName("PK_Triggers_Events")
                .IsClustered(false);

            entity.ToTable("T_Triggers_Events");
        });

        modelBuilder.Entity<TTriggersHeader>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK_Triggers_Header")
                .IsClustered(false);

            entity.ToTable("T_Triggers_Header");

            entity.Property(e => e.AlarmText).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.OutputSim).HasColumnName("OutputSIM");
            entity.Property(e => e.PopulationSim).HasColumnName("PopulationSIM");
            entity.Property(e => e.RelayBdoor).HasColumnName("RelayBDoor");
            entity.Property(e => e.ResetRelayBperiod).HasColumnName("ResetRelayBPeriod");
        });

        modelBuilder.Entity<TUserPermissions>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Area })
                .HasName("PK_User_Permissions")
                .IsClustered(false);

            entity.ToTable("T_User_Permissions");

            entity.Property(e => e.Area)
                .HasMaxLength(3)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TUserSites>(entity =>
        {
            entity.HasKey(e => new { e.Code, e.Site })
                .HasName("PK_User_Sites")
                .IsClustered(false);

            entity.ToTable("T_User_Sites");
        });

        modelBuilder.Entity<TUserlog>(entity =>
        {
            entity.HasKey(e => e.Key)
                .HasName("PK_Userlog")
                .IsClustered(false);

            entity.ToTable("T_Userlog");

            entity.Property(e => e.Key).HasMaxLength(28);
            entity.Property(e => e.Id)
                .HasMaxLength(28)
                .HasColumnName("ID");
            entity.Property(e => e.LogInDateTime).HasMaxLength(30);
            entity.Property(e => e.LogInEvent).HasMaxLength(200);
            entity.Property(e => e.LogInPoint).HasMaxLength(28);
            entity.Property(e => e.User).HasMaxLength(28);
        });

        modelBuilder.Entity<TUsers>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_Users");

            entity.Property(e => e.Code).ValueGeneratedOnAdd();
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasColumnType("varchar(max)"); // widened from legacy varchar(50) to hold BCrypt hashes
        });

        modelBuilder.Entity<VNameAccessLevels>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_Name_AccessLevels");

            entity.Property(e => e.CardId)
                .HasMaxLength(8)
                .HasColumnName("CardID");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Pin)
                .HasMaxLength(8)
                .HasColumnName("PIN");
            entity.Property(e => e.ValidFrom)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ValidTo)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VNameBlankAccessLevels>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_Name_BlankAccessLevels");

            entity.Property(e => e.CardId)
                .HasMaxLength(8)
                .HasColumnName("CardID");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Pin)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("PIN");
            entity.Property(e => e.ValidFrom)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ValidTo)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VNameHeader>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_Name_Header");

            entity.Property(e => e.CardId).HasColumnName("CardID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
