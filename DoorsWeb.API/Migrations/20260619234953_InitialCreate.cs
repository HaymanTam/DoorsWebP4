using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DoorsWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Arc_Custom",
                columns: table => new
                {
                    CustomFieldCode = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Arc_CustomFieldTypes",
                columns: table => new
                {
                    CustomField = table.Column<int>(type: "integer", nullable: false),
                    Literal = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DataType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Arc_Doors",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Door = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ControllerID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    Site = table.Column<int>(type: "integer", nullable: true),
                    Connector = table.Column<int>(type: "integer", nullable: true),
                    LocalDoorNumber = table.Column<int>(type: "integer", nullable: true),
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    RTCTime = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    RTCDate = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    LogCount = table.Column<int>(type: "integer", nullable: true),
                    VdiskDirectories = table.Column<int>(type: "integer", nullable: true),
                    LogUpdateInterval1 = table.Column<int>(type: "integer", nullable: true),
                    LogUpdateInterval2 = table.Column<int>(type: "integer", nullable: true),
                    StatusUpdateInterval1 = table.Column<int>(type: "integer", nullable: true),
                    StatusUpdateInterval2 = table.Column<int>(type: "integer", nullable: true),
                    DoorType = table.Column<int>(type: "integer", nullable: true),
                    ReleaseTime = table.Column<int>(type: "integer", nullable: true),
                    ReleaseDelay = table.Column<int>(type: "integer", nullable: true),
                    PDO = table.Column<int>(type: "integer", nullable: true),
                    TechnologyA = table.Column<int>(type: "integer", nullable: true),
                    RelayBmode = table.Column<int>(type: "integer", nullable: true),
                    Lock_Drive_Mode = table.Column<int>(type: "integer", nullable: true),
                    LogInA = table.Column<bool>(type: "boolean", nullable: true),
                    LogoutA = table.Column<bool>(type: "boolean", nullable: true),
                    TechnologyB = table.Column<int>(type: "integer", nullable: true),
                    LogInB = table.Column<bool>(type: "boolean", nullable: true),
                    LogoutB = table.Column<bool>(type: "boolean", nullable: true),
                    CarIn = table.Column<bool>(type: "boolean", nullable: true),
                    CarOut = table.Column<bool>(type: "boolean", nullable: true),
                    KeyboardTech = table.Column<int>(type: "integer", nullable: true),
                    AutoRelock = table.Column<bool>(type: "boolean", nullable: true),
                    RelayBTimeZone = table.Column<int>(type: "integer", nullable: true),
                    CardandPINTimeZone = table.Column<int>(type: "integer", nullable: true),
                    ControllerIP = table.Column<bool>(type: "boolean", nullable: true),
                    DoorIPAddress = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AutoDelayVal = table.Column<int>(type: "integer", nullable: true),
                    ReleaseTimeB = table.Column<int>(type: "integer", nullable: true),
                    TimeLock = table.Column<int>(type: "integer", nullable: true),
                    LastCard = table.Column<int>(type: "integer", nullable: true),
                    LastEvent = table.Column<int>(type: "integer", nullable: true),
                    LastCardID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    LastDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status1 = table.Column<int>(type: "integer", nullable: true),
                    Status2 = table.Column<int>(type: "integer", nullable: true),
                    XPlace = table.Column<float>(type: "real", nullable: true),
                    YPlace = table.Column<float>(type: "real", nullable: true),
                    PlanNumber = table.Column<int>(type: "integer", nullable: true),
                    AlarmZoneNumber = table.Column<int>(type: "integer", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReaderAName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ReaderBName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    KeyboardName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FloorPlan = table.Column<int>(type: "integer", nullable: true),
                    FloorPlanX = table.Column<int>(type: "integer", nullable: true),
                    FloorPlanY = table.Column<int>(type: "integer", nullable: true),
                    Keypad_Star_Mode = table.Column<int>(type: "integer", nullable: true),
                    Random_Search_Freq = table.Column<int>(type: "integer", nullable: true),
                    CON_FB_Volume = table.Column<int>(type: "integer", nullable: true),
                    CON_ALM_Volume = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Len = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig1 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig2 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig3 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig4 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig5 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig6 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig7 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig8 = table.Column<int>(type: "integer", nullable: true),
                    Bio_Enrol_A = table.Column<bool>(type: "boolean", nullable: true),
                    Bio_Enrol_B = table.Column<bool>(type: "boolean", nullable: true),
                    RDR_Brightness_A = table.Column<int>(type: "integer", nullable: true),
                    RDR_Brightness_B = table.Column<int>(type: "integer", nullable: true),
                    RDR_Volume_A = table.Column<int>(type: "integer", nullable: true),
                    RDR_Volume_B = table.Column<int>(type: "integer", nullable: true),
                    ID_Sequence_A = table.Column<int>(type: "integer", nullable: true),
                    ID_Sequence_B = table.Column<int>(type: "integer", nullable: true),
                    ValidFromTimeHH = table.Column<int>(type: "integer", nullable: true),
                    ValidFromTimeMM = table.Column<int>(type: "integer", nullable: true),
                    ValidToTimeHH = table.Column<int>(type: "integer", nullable: true),
                    ValidToTimeMM = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Arc_Events",
                columns: table => new
                {
                    EventDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CardNumber = table.Column<int>(type: "integer", nullable: false),
                    DoorNumber = table.Column<int>(type: "integer", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    ReaderID = table.Column<int>(type: "integer", nullable: false),
                    EventID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActualCardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    AlarmID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Arc_Name_CustomFields",
                columns: table => new
                {
                    CardNumber = table.Column<int>(type: "integer", nullable: false),
                    Custom1 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom3 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom4 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom5 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom6 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom7 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom8 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom9 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom10 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom11 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom12 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom13 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom14 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom15 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom16 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom17 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom18 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom19 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom20 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom21 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom22 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom23 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom24 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom25 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Arc_Name_Header",
                columns: table => new
                {
                    Surname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Forname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CardNumber = table.Column<int>(type: "integer", nullable: false),
                    CardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    OldCardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    Flexi = table.Column<bool>(type: "boolean", nullable: true),
                    InUse = table.Column<bool>(type: "boolean", nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: true),
                    Void = table.Column<bool>(type: "boolean", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastDoor = table.Column<int>(type: "integer", nullable: true),
                    LastEvent = table.Column<int>(type: "integer", nullable: true),
                    LastDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Rollcall = table.Column<int>(type: "integer", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatePending = table.Column<bool>(type: "boolean", nullable: true),
                    PIN = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    PinRequired = table.Column<int>(type: "integer", nullable: true),
                    ValidToOverride = table.Column<int>(type: "integer", nullable: true),
                    ValidFromOverride = table.Column<int>(type: "integer", nullable: true),
                    HotStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    BioAdmin = table.Column<bool>(type: "boolean", nullable: true),
                    BioOptOut = table.Column<bool>(type: "boolean", nullable: true),
                    CardDesign = table.Column<int>(type: "integer", nullable: true),
                    IDCardDesign = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    APBNumber = table.Column<int>(type: "integer", nullable: true),
                    APBDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Arc_Sites",
                columns: table => new
                {
                    Site = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Audit",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(60)", unicode: false, maxLength: 60, nullable: false),
                    ClientIp = table.Column<string>(type: "character varying(45)", unicode: false, maxLength: 45, nullable: true),
                    Action = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    EntityKey = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    EntityName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Audit", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "T_Backup",
                columns: table => new
                {
                    ScheduleOn = table.Column<bool>(type: "boolean", nullable: false),
                    ScheduleNumber = table.Column<int>(type: "integer", nullable: false),
                    ScheduleFrequency = table.Column<int>(type: "integer", nullable: false),
                    ScheduleTime = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    MaximumOn = table.Column<bool>(type: "boolean", nullable: false),
                    MaximumNumber = table.Column<int>(type: "integer", nullable: false),
                    DeleteToRecycleBin = table.Column<bool>(type: "boolean", nullable: false),
                    KeepOn = table.Column<bool>(type: "boolean", nullable: false),
                    KeepNumber = table.Column<int>(type: "integer", nullable: false),
                    SaveDays = table.Column<int>(type: "integer", nullable: false),
                    IncludePhotos = table.Column<bool>(type: "boolean", nullable: false),
                    IncludePlans = table.Column<bool>(type: "boolean", nullable: false),
                    LastBackup = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Bio_Data",
                columns: table => new
                {
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    BioChecksum_Left = table.Column<int>(type: "integer", nullable: true),
                    BioChecksum_Right = table.Column<int>(type: "integer", nullable: true),
                    ID = table.Column<string>(type: "character varying(12)", unicode: false, maxLength: 12, nullable: true),
                    Status = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Finger_Left = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Finger_Right = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Bio_Template_Left = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Bio_Template_Right = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bio_Data", x => x.Slot);
                });

            migrationBuilder.CreateTable(
                name: "T_CardManager_Header",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ViewType = table.Column<int>(type: "integer", nullable: true),
                    Owner = table.Column<int>(type: "integer", nullable: true),
                    ListViewType = table.Column<int>(type: "integer", nullable: true),
                    Orientation = table.Column<int>(type: "integer", nullable: true),
                    LeftMargin = table.Column<int>(type: "integer", nullable: true),
                    RightMargin = table.Column<int>(type: "integer", nullable: true),
                    TopMargin = table.Column<int>(type: "integer", nullable: true),
                    BottomMargin = table.Column<int>(type: "integer", nullable: true),
                    Printer = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardManager_Header", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "T_CardPack_Header",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FirstCardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardPack_Header", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "T_Customer",
                columns: table => new
                {
                    CustomerCompany = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerContact = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerTelephone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerFax = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Customeremail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CustomerAddress1 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerAddress2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerAddress3 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerAddress4 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerCountry = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerPostCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    InstallerCompany = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerContact = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerTelephone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerFax = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Installeremail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InstallerAddress1 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerAddress2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerAddress3 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerAddress4 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerCountry = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerPostCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    InstallationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CommissionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InstallType = table.Column<int>(type: "integer", nullable: false),
                    ProductKey = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    Comments = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_CustomFieldTypes",
                columns: table => new
                {
                    CustomField = table.Column<int>(type: "integer", nullable: false),
                    Literal = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DataType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldTypes", x => x.CustomField);
                });

            migrationBuilder.CreateTable(
                name: "T_DisplayTypes",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    PropertyID = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false),
                    IsTrueFalse = table.Column<bool>(type: "boolean", nullable: false),
                    TrueDescription = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false),
                    FalseDescription = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayTypes", x => new { x.Code, x.PropertyID });
                });

            migrationBuilder.CreateTable(
                name: "T_Door_Technology",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TechnologyCode = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Door_Technology", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "T_EnterpriseData",
                columns: table => new
                {
                    Property = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnterpriseData", x => new { x.Property, x.Value });
                });

            migrationBuilder.CreateTable(
                name: "T_EventTypes",
                columns: table => new
                {
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: true),
                    Icon = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.EventType);
                });

            migrationBuilder.CreateTable(
                name: "T_Modems",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    COMPort = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modems", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "T_Sites",
                columns: table => new
                {
                    Site = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Site);
                });

            migrationBuilder.CreateTable(
                name: "T_StatusView",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ShowAllFloors = table.Column<bool>(type: "boolean", nullable: true),
                    Display = table.Column<int>(type: "integer", nullable: true),
                    Wait = table.Column<int>(type: "integer", nullable: true),
                    Move = table.Column<int>(type: "integer", nullable: true),
                    Pause = table.Column<int>(type: "integer", nullable: true),
                    UpdateChangesWhenPanning = table.Column<bool>(type: "boolean", nullable: true),
                    StatusView = table.Column<int>(type: "integer", nullable: true),
                    StatusViewX = table.Column<int>(type: "integer", nullable: true),
                    StatusViewY = table.Column<int>(type: "integer", nullable: true),
                    StatusViewZ = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusView", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "T_System",
                columns: table => new
                {
                    CurrentVersion = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    AutoConfigUpdate = table.Column<bool>(type: "boolean", nullable: true),
                    EnableFlexi = table.Column<bool>(type: "boolean", nullable: true),
                    Corporate1000Code = table.Column<string>(type: "character varying(4)", unicode: false, maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_TimeSheet_Header",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LastName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom1 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom3 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom4 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom5 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom6 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom7 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom8 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom9 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom10 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom11 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom12 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom13 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom14 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom15 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom16 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom17 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom18 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom19 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom20 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom21 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom22 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom23 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom24 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Custom25 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DateSearch = table.Column<int>(type: "integer", nullable: false),
                    InLastNumber = table.Column<int>(type: "integer", nullable: false),
                    InLastPeriod = table.Column<int>(type: "integer", nullable: false),
                    DateFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Rollover = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DatePageBreak = table.Column<bool>(type: "boolean", nullable: false),
                    CardIDPageBreak = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSheet_Header", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "T_User_Permissions",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Area = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Permissions", x => new { x.Code, x.Area });
                });

            migrationBuilder.CreateTable(
                name: "T_Userlog",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(28)", maxLength: 28, nullable: false),
                    ID = table.Column<string>(type: "character varying(28)", maxLength: 28, nullable: true),
                    User = table.Column<string>(type: "character varying(28)", maxLength: 28, nullable: true),
                    LogInPoint = table.Column<string>(type: "character varying(28)", maxLength: 28, nullable: true),
                    LogInEvent = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LogInDateTime = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Userlog", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "T_Users",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Administrator = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CardManagerAccess = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SiteSettingsAccess = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UserSettingsAccess = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Users", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "T_CardManager_Default",
                columns: table => new
                {
                    User = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardManager_Default", x => x.User);
                    table.ForeignKey(
                        name: "FK_T_CardManager_Default_T_CardManager_Header_Code",
                        column: x => x.Code,
                        principalTable: "T_CardManager_Header",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "T_CardManager_OrderByFields",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    SortNumber = table.Column<int>(type: "integer", nullable: false),
                    TableName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Descending = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardManager_OrderByFields", x => new { x.Code, x.SortNumber });
                    table.ForeignKey(
                        name: "FK_T_CardManager_OrderByFields_T_CardManager_Header_Code",
                        column: x => x.Code,
                        principalTable: "T_CardManager_Header",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "T_CardManager_SelectFields",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    FieldType = table.Column<int>(type: "integer", nullable: true),
                    TableName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ColumnWidth = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardManager_SelectFields", x => new { x.Code, x.Position });
                    table.ForeignKey(
                        name: "FK_T_CardManager_SelectFields_T_CardManager_Header_Code",
                        column: x => x.Code,
                        principalTable: "T_CardManager_Header",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "T_CardManager_WhereFields",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    TableName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FieldType = table.Column<int>(type: "integer", nullable: true),
                    Operator = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Value1 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Value2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardManager_WhereFields", x => new { x.Code, x.Sequence });
                    table.ForeignKey(
                        name: "FK_T_CardManager_WhereFields_T_CardManager_Header_Code",
                        column: x => x.Code,
                        principalTable: "T_CardManager_Header",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "T_Custom",
                columns: table => new
                {
                    CustomFieldCode = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Custom", x => new { x.CustomFieldCode, x.Code });
                    table.ForeignKey(
                        name: "FK_T_Custom_T_CustomFieldTypes_CustomFieldCode",
                        column: x => x.CustomFieldCode,
                        principalTable: "T_CustomFieldTypes",
                        principalColumn: "CustomField");
                });

            migrationBuilder.CreateTable(
                name: "T_Display",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    PropertyID = table.Column<int>(type: "integer", nullable: false),
                    ColumnWidth = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Display", x => new { x.Code, x.Position });
                    table.ForeignKey(
                        name: "FK_T_Display_T_DisplayTypes_Code_PropertyID",
                        columns: x => new { x.Code, x.PropertyID },
                        principalTable: "T_DisplayTypes",
                        principalColumns: new[] { "Code", "PropertyID" });
                });

            migrationBuilder.CreateTable(
                name: "T_AccessLevel_Header",
                columns: table => new
                {
                    AccessLevel = table.Column<int>(type: "integer", nullable: false),
                    Site = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TimeZone = table.Column<int>(type: "integer", nullable: true),
                    LocalLevel = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLevel_Header", x => new { x.AccessLevel, x.Site });
                    table.ForeignKey(
                        name: "FK_T_AccessLevel_Header_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_Alarms",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Site = table.Column<int>(type: "integer", nullable: true),
                    AlarmType = table.Column<int>(type: "integer", nullable: true),
                    AlarmDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AlarmDescription = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ActionedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ActionedBy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ActionedText = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: true),
                    ControllerNumber = table.Column<int>(type: "integer", nullable: true),
                    InputIndex = table.Column<int>(type: "integer", nullable: true),
                    EventType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alarms", x => x.Code);
                    table.ForeignKey(
                        name: "FK_T_Alarms_T_EventTypes_EventType",
                        column: x => x.EventType,
                        principalTable: "T_EventTypes",
                        principalColumn: "EventType");
                    table.ForeignKey(
                        name: "FK_T_Alarms_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_APBZone_Header",
                columns: table => new
                {
                    APBNumber = table.Column<int>(type: "integer", nullable: false),
                    Site = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    APBMode = table.Column<int>(type: "integer", nullable: true),
                    DiscoveryMode = table.Column<int>(type: "integer", nullable: true),
                    DiscoveryModeDuration = table.Column<int>(type: "integer", nullable: true),
                    DiscoveryModeExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DiscoveryModeStart = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DiscoveryModeOnFireAlarm = table.Column<bool>(type: "boolean", nullable: true),
                    FireInterfaceDoor = table.Column<int>(type: "integer", nullable: true),
                    DiscoveryModeOnFireAlarmDuration = table.Column<int>(type: "integer", nullable: true),
                    AutoLogOut = table.Column<bool>(type: "boolean", nullable: true),
                    NextAutoLogout = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APBZone_Header", x => x.APBNumber);
                    table.ForeignKey(
                        name: "FK_T_APBZone_Header_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_Calendar_Header",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Site = table.Column<int>(type: "integer", nullable: false),
                    LocalNumber = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar_Header", x => x.Code);
                    table.ForeignKey(
                        name: "FK_T_Calendar_Header_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_CardPack_Details",
                columns: table => new
                {
                    Site = table.Column<int>(type: "integer", nullable: false),
                    CardPack = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardPack_Details", x => new { x.Site, x.CardPack });
                    table.ForeignKey(
                        name: "FK_T_CardPack_Details_T_CardPack_Header_CardPack",
                        column: x => x.CardPack,
                        principalTable: "T_CardPack_Header",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_T_CardPack_Details_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_Connectors",
                columns: table => new
                {
                    Connector = table.Column<int>(type: "integer", nullable: false),
                    Site = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    Path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ComPort = table.Column<int>(type: "integer", nullable: true),
                    ForceDistrib = table.Column<bool>(type: "boolean", nullable: true),
                    ForceTimeSync = table.Column<bool>(type: "boolean", nullable: true),
                    Telnumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RetryInterval = table.Column<int>(type: "integer", nullable: true),
                    Retries = table.Column<int>(type: "integer", nullable: true),
                    PABX = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ConnType = table.Column<int>(type: "integer", nullable: true),
                    PingFrequency = table.Column<int>(type: "integer", nullable: true),
                    CommandFrequency = table.Column<int>(type: "integer", nullable: true),
                    ForcePing = table.Column<int>(type: "integer", nullable: true),
                    DownloadLogsWhenUpdating = table.Column<bool>(type: "boolean", nullable: true),
                    Encrypted = table.Column<bool>(type: "boolean", nullable: true),
                    IPAddress = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ModemConnectionFrequency = table.Column<int>(type: "integer", nullable: true),
                    ModemConnectionPeriod = table.Column<int>(type: "integer", nullable: true),
                    ModemConnectionTime = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    ModemUploadCommands = table.Column<bool>(type: "boolean", nullable: true),
                    ModemDownloadLogs = table.Column<bool>(type: "boolean", nullable: true),
                    ModemStayConnected = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connectors", x => x.Connector);
                    table.ForeignKey(
                        name: "FK_T_Connectors_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_FloorPlans",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Site = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FloorPlans", x => x.Code);
                    table.ForeignKey(
                        name: "FK_T_FloorPlans_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_SpaceZone_Header",
                columns: table => new
                {
                    ZoneNumber = table.Column<int>(type: "integer", nullable: false),
                    Site = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    FireZone = table.Column<bool>(type: "boolean", nullable: true),
                    FireInterfaceDoor = table.Column<int>(type: "integer", nullable: true),
                    OpenDoorsOnFireAlarm = table.Column<bool>(type: "boolean", nullable: true),
                    CloseDoorsOnFireReset = table.Column<bool>(type: "boolean", nullable: true),
                    MaxStayOn = table.Column<bool>(type: "boolean", nullable: true),
                    MaxStay = table.Column<int>(type: "integer", nullable: true),
                    Report = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    LocalReport = table.Column<bool>(type: "boolean", nullable: true),
                    Rented = table.Column<bool>(type: "boolean", nullable: true),
                    InDispute = table.Column<bool>(type: "boolean", nullable: true),
                    Reserved = table.Column<bool>(type: "boolean", nullable: true),
                    RestrictCardholders = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceZone_Header", x => x.ZoneNumber);
                    table.ForeignKey(
                        name: "FK_T_SpaceZone_Header_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_Triggers_Header",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Site = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TriggerType = table.Column<int>(type: "integer", nullable: false),
                    PopulationDirection = table.Column<int>(type: "integer", nullable: false),
                    PopulationValue = table.Column<int>(type: "integer", nullable: false),
                    PopulationSIM = table.Column<int>(type: "integer", nullable: false),
                    PopulationInput = table.Column<int>(type: "integer", nullable: false),
                    PopulationInputOpens = table.Column<bool>(type: "boolean", nullable: false),
                    TriggerOutput = table.Column<bool>(type: "boolean", nullable: false),
                    SuppressDuplicates = table.Column<bool>(type: "boolean", nullable: false),
                    OutputSIM = table.Column<int>(type: "integer", nullable: false),
                    OutputIndex = table.Column<int>(type: "integer", nullable: false),
                    OpenOutput = table.Column<bool>(type: "boolean", nullable: false),
                    ResetOutput = table.Column<bool>(type: "boolean", nullable: false),
                    ResetOutputPeriod = table.Column<int>(type: "integer", nullable: false),
                    TriggerRelayB = table.Column<bool>(type: "boolean", nullable: false),
                    RelayBDoor = table.Column<int>(type: "integer", nullable: false),
                    OpenRelayB = table.Column<bool>(type: "boolean", nullable: false),
                    ResetRelayB = table.Column<bool>(type: "boolean", nullable: false),
                    ResetRelayBPeriod = table.Column<int>(type: "integer", nullable: false),
                    ShowAlarm = table.Column<bool>(type: "boolean", nullable: false),
                    AlarmText = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triggers_Header", x => x.Code);
                    table.ForeignKey(
                        name: "FK_T_Triggers_Header_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_User_Sites",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Site = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Sites", x => new { x.Code, x.Site });
                    table.ForeignKey(
                        name: "FK_T_User_Sites_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_Calendar_Details",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    ExceptionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar_Details", x => new { x.Code, x.ExceptionDate });
                    table.ForeignKey(
                        name: "FK_T_Calendar_Details_T_Calendar_Header_Code",
                        column: x => x.Code,
                        principalTable: "T_Calendar_Header",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "T_TimeZone_Details",
                columns: table => new
                {
                    TimeZone = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    EndTime = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Sun = table.Column<bool>(type: "boolean", nullable: true),
                    Mon = table.Column<bool>(type: "boolean", nullable: true),
                    Tue = table.Column<bool>(type: "boolean", nullable: true),
                    Wed = table.Column<bool>(type: "boolean", nullable: true),
                    Thu = table.Column<bool>(type: "boolean", nullable: true),
                    Fri = table.Column<bool>(type: "boolean", nullable: true),
                    Sat = table.Column<bool>(type: "boolean", nullable: true),
                    Calendar = table.Column<int>(type: "integer", nullable: true),
                    DefaultCalendar = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZone_Details", x => new { x.TimeZone, x.Sequence });
                    table.ForeignKey(
                        name: "FK_T_TimeZone_Details_T_Calendar_Header_Calendar",
                        column: x => x.Calendar,
                        principalTable: "T_Calendar_Header",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "T_TimeZone_Header",
                columns: table => new
                {
                    TimeZone = table.Column<int>(type: "integer", nullable: false),
                    Site = table.Column<int>(type: "integer", nullable: false),
                    LocalTimeZone = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Calendar = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZone_Header", x => new { x.TimeZone, x.Site });
                    table.ForeignKey(
                        name: "FK_T_TimeZone_Header_T_Calendar_Header_Calendar",
                        column: x => x.Calendar,
                        principalTable: "T_Calendar_Header",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_T_TimeZone_Header_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_Commands",
                columns: table => new
                {
                    CommandID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    ControllerID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    Connector = table.Column<int>(type: "integer", nullable: true),
                    Command = table.Column<int>(type: "integer", nullable: true),
                    EngFunctionNumber = table.Column<int>(type: "integer", nullable: true),
                    Data = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NewID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    NewQty = table.Column<int>(type: "integer", nullable: true),
                    OldID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    OldQty = table.Column<int>(type: "integer", nullable: true),
                    ValidFrom = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    ValidTo = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    LevelA = table.Column<int>(type: "integer", nullable: true),
                    LevelB = table.Column<int>(type: "integer", nullable: true),
                    LevelC = table.Column<int>(type: "integer", nullable: true),
                    LevelD = table.Column<int>(type: "integer", nullable: true),
                    B1 = table.Column<short>(type: "smallint", nullable: true),
                    B2 = table.Column<short>(type: "smallint", nullable: true),
                    B3 = table.Column<short>(type: "smallint", nullable: true),
                    B4 = table.Column<short>(type: "smallint", nullable: true),
                    B5 = table.Column<short>(type: "smallint", nullable: true),
                    B6 = table.Column<short>(type: "smallint", nullable: true),
                    B7 = table.Column<short>(type: "smallint", nullable: true),
                    B8 = table.Column<short>(type: "smallint", nullable: true),
                    B9 = table.Column<short>(type: "smallint", nullable: true),
                    B10 = table.Column<short>(type: "smallint", nullable: true),
                    B11 = table.Column<short>(type: "smallint", nullable: true),
                    B12 = table.Column<short>(type: "smallint", nullable: true),
                    B13 = table.Column<short>(type: "smallint", nullable: true),
                    B14 = table.Column<short>(type: "smallint", nullable: true),
                    B15 = table.Column<short>(type: "smallint", nullable: true),
                    B16 = table.Column<short>(type: "smallint", nullable: true),
                    Bol1 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol2 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol3 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol4 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol5 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol6 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol7 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol8 = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => x.CommandID);
                    table.ForeignKey(
                        name: "FK_T_Commands_T_Connectors_Connector",
                        column: x => x.Connector,
                        principalTable: "T_Connectors",
                        principalColumn: "Connector");
                });

            migrationBuilder.CreateTable(
                name: "T_IOController_Header",
                columns: table => new
                {
                    ControllerID = table.Column<int>(type: "integer", nullable: false),
                    ControllerIndex = table.Column<int>(type: "integer", nullable: true),
                    Connector = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IPAddress = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    ControllerDay = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    ControllerVersion = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    RTCTime = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    RTCDate = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOController_Header", x => x.ControllerID);
                    table.ForeignKey(
                        name: "FK_T_IOController_Header_T_Connectors_Connector",
                        column: x => x.Connector,
                        principalTable: "T_Connectors",
                        principalColumn: "Connector");
                });

            migrationBuilder.CreateTable(
                name: "T_Doors",
                columns: table => new
                {
                    Door = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ControllerID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    Site = table.Column<int>(type: "integer", nullable: true),
                    Connector = table.Column<int>(type: "integer", nullable: true),
                    LocalDoorNumber = table.Column<int>(type: "integer", nullable: true),
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    RTCTime = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    RTCDate = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    LogCount = table.Column<int>(type: "integer", nullable: true),
                    VdiskDirectories = table.Column<int>(type: "integer", nullable: true),
                    LogUpdateInterval1 = table.Column<int>(type: "integer", nullable: true),
                    LogUpdateInterval2 = table.Column<int>(type: "integer", nullable: true),
                    StatusUpdateInterval1 = table.Column<int>(type: "integer", nullable: true),
                    StatusUpdateInterval2 = table.Column<int>(type: "integer", nullable: true),
                    DoorType = table.Column<int>(type: "integer", nullable: true),
                    ReleaseTime = table.Column<int>(type: "integer", nullable: true),
                    ReleaseDelay = table.Column<int>(type: "integer", nullable: true),
                    PDO = table.Column<int>(type: "integer", nullable: true),
                    TechnologyA = table.Column<int>(type: "integer", nullable: true),
                    RelayBmode = table.Column<int>(type: "integer", nullable: true),
                    Lock_Drive_Mode = table.Column<int>(type: "integer", nullable: true),
                    LogInA = table.Column<bool>(type: "boolean", nullable: true),
                    LogoutA = table.Column<bool>(type: "boolean", nullable: true),
                    TechnologyB = table.Column<int>(type: "integer", nullable: true),
                    LogInB = table.Column<bool>(type: "boolean", nullable: true),
                    LogoutB = table.Column<bool>(type: "boolean", nullable: true),
                    CarIn = table.Column<bool>(type: "boolean", nullable: true),
                    CarOut = table.Column<bool>(type: "boolean", nullable: true),
                    KeyboardTech = table.Column<int>(type: "integer", nullable: true),
                    AutoRelock = table.Column<bool>(type: "boolean", nullable: true),
                    RelayBTimeZone = table.Column<int>(type: "integer", nullable: true),
                    CardandPINTimeZone = table.Column<int>(type: "integer", nullable: true),
                    ControllerIP = table.Column<bool>(type: "boolean", nullable: true),
                    DoorIPAddress = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AutoDelayVal = table.Column<int>(type: "integer", nullable: true),
                    ReleaseTimeB = table.Column<int>(type: "integer", nullable: true),
                    TimeLock = table.Column<int>(type: "integer", nullable: true),
                    LastCard = table.Column<int>(type: "integer", nullable: true),
                    LastEvent = table.Column<int>(type: "integer", nullable: true),
                    LastCardID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    LastDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status1 = table.Column<int>(type: "integer", nullable: true),
                    Status2 = table.Column<int>(type: "integer", nullable: true),
                    XPlace = table.Column<float>(type: "real", nullable: true),
                    YPlace = table.Column<float>(type: "real", nullable: true),
                    PlanNumber = table.Column<int>(type: "integer", nullable: true),
                    AlarmZoneNumber = table.Column<int>(type: "integer", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReaderAName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ReaderBName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    KeyboardName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FloorPlan = table.Column<int>(type: "integer", nullable: true),
                    FloorPlanX = table.Column<int>(type: "integer", nullable: true),
                    FloorPlanY = table.Column<int>(type: "integer", nullable: true),
                    Keypad_Star_Mode = table.Column<int>(type: "integer", nullable: true),
                    Random_Search_Freq = table.Column<int>(type: "integer", nullable: true),
                    CON_FB_Volume = table.Column<int>(type: "integer", nullable: true),
                    CON_ALM_Volume = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Len = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig1 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig2 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig3 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig4 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig5 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig6 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig7 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig8 = table.Column<int>(type: "integer", nullable: true),
                    Bio_Enrol_A = table.Column<bool>(type: "boolean", nullable: true),
                    Bio_Enrol_B = table.Column<bool>(type: "boolean", nullable: true),
                    RDR_Brightness_A = table.Column<int>(type: "integer", nullable: true),
                    RDR_Brightness_B = table.Column<int>(type: "integer", nullable: true),
                    RDR_Volume_A = table.Column<int>(type: "integer", nullable: true),
                    RDR_Volume_B = table.Column<int>(type: "integer", nullable: true),
                    ID_Sequence_A = table.Column<int>(type: "integer", nullable: true),
                    ID_Sequence_B = table.Column<int>(type: "integer", nullable: true),
                    ValidFromTimeHH = table.Column<int>(type: "integer", nullable: true),
                    ValidFromTimeMM = table.Column<int>(type: "integer", nullable: true),
                    ValidToTimeHH = table.Column<int>(type: "integer", nullable: true),
                    ValidToTimeMM = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doors", x => x.Door);
                    table.ForeignKey(
                        name: "FK_T_Doors_T_Connectors_Connector",
                        column: x => x.Connector,
                        principalTable: "T_Connectors",
                        principalColumn: "Connector");
                    table.ForeignKey(
                        name: "FK_T_Doors_T_Door_Technology_KeyboardTech",
                        column: x => x.KeyboardTech,
                        principalTable: "T_Door_Technology",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_T_Doors_T_Door_Technology_TechnologyA",
                        column: x => x.TechnologyA,
                        principalTable: "T_Door_Technology",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_T_Doors_T_Door_Technology_TechnologyB",
                        column: x => x.TechnologyB,
                        principalTable: "T_Door_Technology",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_T_Doors_T_FloorPlans_FloorPlan",
                        column: x => x.FloorPlan,
                        principalTable: "T_FloorPlans",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_T_Doors_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_TimeSheet_Zones",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Zone = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSheet_Zones", x => new { x.Code, x.Zone });
                    table.ForeignKey(
                        name: "FK_T_TimeSheet_Zones_T_SpaceZone_Header_Zone",
                        column: x => x.Zone,
                        principalTable: "T_SpaceZone_Header",
                        principalColumn: "ZoneNumber");
                    table.ForeignKey(
                        name: "FK_T_TimeSheet_Zones_T_TimeSheet_Header_Code",
                        column: x => x.Code,
                        principalTable: "T_TimeSheet_Header",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "T_Triggers_Controllers",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    ControllerCode = table.Column<int>(type: "integer", nullable: false),
                    InputIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triggers_Controllers", x => new { x.Code, x.ControllerCode, x.InputIndex });
                    table.ForeignKey(
                        name: "FK_T_Triggers_Controllers_T_Triggers_Header_Code",
                        column: x => x.Code,
                        principalTable: "T_Triggers_Header",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "T_Triggers_Events",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triggers_Events", x => new { x.Code, x.EventType });
                    table.ForeignKey(
                        name: "FK_T_Triggers_Events_T_EventTypes_EventType",
                        column: x => x.EventType,
                        principalTable: "T_EventTypes",
                        principalColumn: "EventType");
                    table.ForeignKey(
                        name: "FK_T_Triggers_Events_T_Triggers_Header_Code",
                        column: x => x.Code,
                        principalTable: "T_Triggers_Header",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "T_IOController_Details",
                columns: table => new
                {
                    ControllerID = table.Column<int>(type: "integer", nullable: false),
                    IOInputIndex = table.Column<short>(type: "smallint", nullable: false),
                    InputName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OutputName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Inverted = table.Column<bool>(type: "boolean", nullable: true),
                    FloorPlan = table.Column<int>(type: "integer", nullable: true),
                    FloorPlanX = table.Column<int>(type: "integer", nullable: true),
                    FloorPlanY = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOController_Details", x => new { x.ControllerID, x.IOInputIndex });
                    table.ForeignKey(
                        name: "FK_T_IOController_Details_T_FloorPlans_FloorPlan",
                        column: x => x.FloorPlan,
                        principalTable: "T_FloorPlans",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_T_IOController_Details_T_IOController_Header_ControllerID",
                        column: x => x.ControllerID,
                        principalTable: "T_IOController_Header",
                        principalColumn: "ControllerID");
                });

            migrationBuilder.CreateTable(
                name: "T_AccessLevel_Details",
                columns: table => new
                {
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Door = table.Column<int>(type: "integer", nullable: false),
                    LevelDefault = table.Column<bool>(type: "boolean", nullable: true),
                    DoorTimeZone = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLevel_Details", x => new { x.Level, x.Door });
                    table.ForeignKey(
                        name: "FK_T_AccessLevel_Details_T_Doors_Door",
                        column: x => x.Door,
                        principalTable: "T_Doors",
                        principalColumn: "Door");
                });

            migrationBuilder.CreateTable(
                name: "T_APBZone_Details",
                columns: table => new
                {
                    APBNumber = table.Column<int>(type: "integer", nullable: false),
                    DoorNumber = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    MemberType = table.Column<int>(type: "integer", nullable: true),
                    ReaderA = table.Column<int>(type: "integer", nullable: true),
                    ReaderB = table.Column<int>(type: "integer", nullable: true),
                    EnforceOnEntry = table.Column<bool>(type: "boolean", nullable: true),
                    EnforceOnExit = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APBZone_Details", x => new { x.APBNumber, x.DoorNumber });
                    table.ForeignKey(
                        name: "FK_T_APBZone_Details_T_APBZone_Header_APBNumber",
                        column: x => x.APBNumber,
                        principalTable: "T_APBZone_Header",
                        principalColumn: "APBNumber");
                    table.ForeignKey(
                        name: "FK_T_APBZone_Details_T_Doors_DoorNumber",
                        column: x => x.DoorNumber,
                        principalTable: "T_Doors",
                        principalColumn: "Door");
                });

            migrationBuilder.CreateTable(
                name: "T_Name_Header",
                columns: table => new
                {
                    CardNumber = table.Column<int>(type: "integer", nullable: false),
                    Surname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Forname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    OldCardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    Flexi = table.Column<bool>(type: "boolean", nullable: true),
                    InUse = table.Column<bool>(type: "boolean", nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: true),
                    Void = table.Column<bool>(type: "boolean", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastDoor = table.Column<int>(type: "integer", nullable: true),
                    LastEvent = table.Column<int>(type: "integer", nullable: true),
                    LastDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Rollcall = table.Column<int>(type: "integer", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatePending = table.Column<bool>(type: "boolean", nullable: true),
                    PIN = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    PinRequired = table.Column<int>(type: "integer", nullable: true),
                    ValidToOverride = table.Column<int>(type: "integer", nullable: true),
                    ValidFromOverride = table.Column<int>(type: "integer", nullable: true),
                    HotStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    BioAdmin = table.Column<bool>(type: "boolean", nullable: true),
                    BioOptOut = table.Column<bool>(type: "boolean", nullable: true),
                    CardDesign = table.Column<int>(type: "integer", nullable: true),
                    IDCardDesign = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    APBNumber = table.Column<int>(type: "integer", nullable: true),
                    APBDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Name_Header", x => x.CardNumber);
                    table.ForeignKey(
                        name: "FK_T_Name_Header_T_APBZone_Header_APBNumber",
                        column: x => x.APBNumber,
                        principalTable: "T_APBZone_Header",
                        principalColumn: "APBNumber");
                    table.ForeignKey(
                        name: "FK_T_Name_Header_T_Doors_LastDoor",
                        column: x => x.LastDoor,
                        principalTable: "T_Doors",
                        principalColumn: "Door");
                    table.ForeignKey(
                        name: "FK_T_Name_Header_T_EventTypes_LastEvent",
                        column: x => x.LastEvent,
                        principalTable: "T_EventTypes",
                        principalColumn: "EventType");
                });

            migrationBuilder.CreateTable(
                name: "T_SpaceZone_Details",
                columns: table => new
                {
                    Door = table.Column<int>(type: "integer", nullable: false),
                    Site = table.Column<int>(type: "integer", nullable: false),
                    Zone = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    InReader1 = table.Column<bool>(type: "boolean", nullable: true),
                    InReader2 = table.Column<bool>(type: "boolean", nullable: true),
                    InReader3 = table.Column<bool>(type: "boolean", nullable: true),
                    OutReader1 = table.Column<bool>(type: "boolean", nullable: true),
                    OutReader2 = table.Column<bool>(type: "boolean", nullable: true),
                    OutReader3 = table.Column<bool>(type: "boolean", nullable: true),
                    OpenOnFireAlarm = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceZone_Details", x => new { x.Door, x.Site, x.Zone });
                    table.ForeignKey(
                        name: "FK_T_SpaceZone_Details_T_Doors_Door",
                        column: x => x.Door,
                        principalTable: "T_Doors",
                        principalColumn: "Door");
                    table.ForeignKey(
                        name: "FK_T_SpaceZone_Details_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                    table.ForeignKey(
                        name: "FK_T_SpaceZone_Details_T_SpaceZone_Header_Zone",
                        column: x => x.Zone,
                        principalTable: "T_SpaceZone_Header",
                        principalColumn: "ZoneNumber");
                });

            migrationBuilder.CreateTable(
                name: "T_Events",
                columns: table => new
                {
                    EventDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CardNumber = table.Column<int>(type: "integer", nullable: false),
                    DoorNumber = table.Column<int>(type: "integer", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    ReaderID = table.Column<int>(type: "integer", nullable: false),
                    EventID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActualCardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    AlarmID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => new { x.EventDate, x.CardNumber, x.DoorNumber, x.EventType, x.ReaderID, x.EventID });
                    table.ForeignKey(
                        name: "FK_T_Events_T_Doors_DoorNumber",
                        column: x => x.DoorNumber,
                        principalTable: "T_Doors",
                        principalColumn: "Door");
                    table.ForeignKey(
                        name: "FK_T_Events_T_EventTypes_EventType",
                        column: x => x.EventType,
                        principalTable: "T_EventTypes",
                        principalColumn: "EventType");
                    table.ForeignKey(
                        name: "FK_T_Events_T_Name_Header_CardNumber",
                        column: x => x.CardNumber,
                        principalTable: "T_Name_Header",
                        principalColumn: "CardNumber");
                });

            migrationBuilder.CreateTable(
                name: "T_Name_AccessLevels",
                columns: table => new
                {
                    Site = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    CardNumber = table.Column<int>(type: "integer", nullable: false),
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Name_AccessLevels", x => new { x.CardNumber, x.Site, x.Level });
                    table.ForeignKey(
                        name: "FK_T_Name_AccessLevels_T_AccessLevel_Header_Level_Site",
                        columns: x => new { x.Level, x.Site },
                        principalTable: "T_AccessLevel_Header",
                        principalColumns: new[] { "AccessLevel", "Site" });
                    table.ForeignKey(
                        name: "FK_T_Name_AccessLevels_T_Name_Header_CardNumber",
                        column: x => x.CardNumber,
                        principalTable: "T_Name_Header",
                        principalColumn: "CardNumber");
                    table.ForeignKey(
                        name: "FK_T_Name_AccessLevels_T_Sites_Site",
                        column: x => x.Site,
                        principalTable: "T_Sites",
                        principalColumn: "Site");
                });

            migrationBuilder.CreateTable(
                name: "T_Name_CustomFields",
                columns: table => new
                {
                    CardNumber = table.Column<int>(type: "integer", nullable: false),
                    Custom1 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom3 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom4 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom5 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom6 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom7 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom8 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom9 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom10 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom11 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom12 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom13 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom14 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom15 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom16 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom17 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom18 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom19 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom20 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom21 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom22 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom23 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom24 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom25 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Name_CustomFields", x => x.CardNumber);
                    table.ForeignKey(
                        name: "FK_T_Name_CustomFields_T_Name_Header_CardNumber",
                        column: x => x.CardNumber,
                        principalTable: "T_Name_Header",
                        principalColumn: "CardNumber");
                });

            migrationBuilder.CreateTable(
                name: "T_SpaceZone_Attendance",
                columns: table => new
                {
                    ZoneNumber = table.Column<int>(type: "integer", nullable: false),
                    CardIndex = table.Column<int>(type: "integer", nullable: false),
                    DateandTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Keyf = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    AlarmFlag = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceZone_Attendance", x => new { x.ZoneNumber, x.CardIndex, x.DateandTime });
                    table.ForeignKey(
                        name: "FK_T_SpaceZone_Attendance_T_Name_Header_CardIndex",
                        column: x => x.CardIndex,
                        principalTable: "T_Name_Header",
                        principalColumn: "CardNumber");
                    table.ForeignKey(
                        name: "FK_T_SpaceZone_Attendance_T_SpaceZone_Header_ZoneNumber",
                        column: x => x.ZoneNumber,
                        principalTable: "T_SpaceZone_Header",
                        principalColumn: "ZoneNumber");
                });

            migrationBuilder.CreateTable(
                name: "T_SpaceZone_Cardholders",
                columns: table => new
                {
                    SpaceZone = table.Column<int>(type: "integer", nullable: false),
                    CardNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceZone_Cardholders", x => new { x.SpaceZone, x.CardNumber });
                    table.ForeignKey(
                        name: "FK_T_SpaceZone_Cardholders_T_Name_Header_CardNumber",
                        column: x => x.CardNumber,
                        principalTable: "T_Name_Header",
                        principalColumn: "CardNumber");
                    table.ForeignKey(
                        name: "FK_T_SpaceZone_Cardholders_T_SpaceZone_Header_SpaceZone",
                        column: x => x.SpaceZone,
                        principalTable: "T_SpaceZone_Header",
                        principalColumn: "ZoneNumber");
                });

            migrationBuilder.InsertData(
                table: "T_Sites",
                columns: new[] { "Site", "Inuse", "Key", "Name", "Status" },
                values: new object[] { 1, true, null, "Default Site", null });

            migrationBuilder.InsertData(
                table: "T_Users",
                columns: new[] { "Code", "Administrator", "Description", "Password" },
                values: new object[] { 1, true, "admin", "$2a$11$USGWvPjw8RXKz9LjMWWgr.IV5uCz6Zufb2zTBjSBG9fneY.JY0UDW" });

            migrationBuilder.CreateIndex(
                name: "IX_T_AccessLevel_Details_Door",
                table: "T_AccessLevel_Details",
                column: "Door");

            migrationBuilder.CreateIndex(
                name: "IX_T_AccessLevel_Header_Site",
                table: "T_AccessLevel_Header",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_Alarms_EventType",
                table: "T_Alarms",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_T_Alarms_Site",
                table: "T_Alarms",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_APBZone_Details_DoorNumber",
                table: "T_APBZone_Details",
                column: "DoorNumber");

            migrationBuilder.CreateIndex(
                name: "IX_T_APBZone_Header_Site",
                table: "T_APBZone_Header",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_Audit_Timestamp",
                table: "T_Audit",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IND_Slot",
                table: "T_Bio_Data",
                column: "Slot",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_Calendar_Header_Site",
                table: "T_Calendar_Header",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_CardManager_Default_Code",
                table: "T_CardManager_Default",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_T_CardPack_Details_CardPack",
                table: "T_CardPack_Details",
                column: "CardPack");

            migrationBuilder.CreateIndex(
                name: "IND_ControllerID",
                table: "T_Commands",
                column: "ControllerID");

            migrationBuilder.CreateIndex(
                name: "IX_T_Commands_Connector",
                table: "T_Commands",
                column: "Connector");

            migrationBuilder.CreateIndex(
                name: "IX_T_Connectors_Site",
                table: "T_Connectors",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_Display_Code_PropertyID",
                table: "T_Display",
                columns: new[] { "Code", "PropertyID" });

            migrationBuilder.CreateIndex(
                name: "IND_Door",
                table: "T_Doors",
                column: "Door",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_Doors_Connector",
                table: "T_Doors",
                column: "Connector");

            migrationBuilder.CreateIndex(
                name: "IX_T_Doors_FloorPlan",
                table: "T_Doors",
                column: "FloorPlan");

            migrationBuilder.CreateIndex(
                name: "IX_T_Doors_KeyboardTech",
                table: "T_Doors",
                column: "KeyboardTech");

            migrationBuilder.CreateIndex(
                name: "IX_T_Doors_Site",
                table: "T_Doors",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_Doors_TechnologyA",
                table: "T_Doors",
                column: "TechnologyA");

            migrationBuilder.CreateIndex(
                name: "IX_T_Doors_TechnologyB",
                table: "T_Doors",
                column: "TechnologyB");

            migrationBuilder.CreateIndex(
                name: "IND_EventDate",
                table: "T_Events",
                column: "EventDate");

            migrationBuilder.CreateIndex(
                name: "IND_EventID",
                table: "T_Events",
                column: "EventID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_Events_CardNumber",
                table: "T_Events",
                column: "CardNumber");

            migrationBuilder.CreateIndex(
                name: "IX_T_Events_DoorNumber",
                table: "T_Events",
                column: "DoorNumber");

            migrationBuilder.CreateIndex(
                name: "IX_T_Events_EventType",
                table: "T_Events",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IND_EventTypes",
                table: "T_EventTypes",
                column: "EventType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_FloorPlans_Site",
                table: "T_FloorPlans",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_IOController_Details_FloorPlan",
                table: "T_IOController_Details",
                column: "FloorPlan");

            migrationBuilder.CreateIndex(
                name: "IX_T_IOController_Header_Connector",
                table: "T_IOController_Header",
                column: "Connector");

            migrationBuilder.CreateIndex(
                name: "IX_T_Name_AccessLevels_Level_Site",
                table: "T_Name_AccessLevels",
                columns: new[] { "Level", "Site" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Name_AccessLevels_Site",
                table: "T_Name_AccessLevels",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IND_Name",
                table: "T_Name_Header",
                column: "CardNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_Name_Header_APBNumber",
                table: "T_Name_Header",
                column: "APBNumber");

            migrationBuilder.CreateIndex(
                name: "IX_T_Name_Header_LastDoor",
                table: "T_Name_Header",
                column: "LastDoor");

            migrationBuilder.CreateIndex(
                name: "IX_T_Name_Header_LastEvent",
                table: "T_Name_Header",
                column: "LastEvent");

            migrationBuilder.CreateIndex(
                name: "IND_Site",
                table: "T_Sites",
                column: "Site",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_SpaceZone_Attendance_CardIndex",
                table: "T_SpaceZone_Attendance",
                column: "CardIndex");

            migrationBuilder.CreateIndex(
                name: "IX_T_SpaceZone_Cardholders_CardNumber",
                table: "T_SpaceZone_Cardholders",
                column: "CardNumber");

            migrationBuilder.CreateIndex(
                name: "IX_T_SpaceZone_Details_Site",
                table: "T_SpaceZone_Details",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_SpaceZone_Details_Zone",
                table: "T_SpaceZone_Details",
                column: "Zone");

            migrationBuilder.CreateIndex(
                name: "IX_T_SpaceZone_Header_Site",
                table: "T_SpaceZone_Header",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_TimeSheet_Zones_Zone",
                table: "T_TimeSheet_Zones",
                column: "Zone");

            migrationBuilder.CreateIndex(
                name: "IX_T_TimeZone_Details_Calendar",
                table: "T_TimeZone_Details",
                column: "Calendar");

            migrationBuilder.CreateIndex(
                name: "IX_T_TimeZone_Header_Calendar",
                table: "T_TimeZone_Header",
                column: "Calendar");

            migrationBuilder.CreateIndex(
                name: "IX_T_TimeZone_Header_Site",
                table: "T_TimeZone_Header",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_Triggers_Events_EventType",
                table: "T_Triggers_Events",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_T_Triggers_Header_Site",
                table: "T_Triggers_Header",
                column: "Site");

            migrationBuilder.CreateIndex(
                name: "IX_T_User_Sites_Site",
                table: "T_User_Sites",
                column: "Site");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_AccessLevel_Details");

            migrationBuilder.DropTable(
                name: "T_Alarms");

            migrationBuilder.DropTable(
                name: "T_APBZone_Details");

            migrationBuilder.DropTable(
                name: "T_Arc_Custom");

            migrationBuilder.DropTable(
                name: "T_Arc_CustomFieldTypes");

            migrationBuilder.DropTable(
                name: "T_Arc_Doors");

            migrationBuilder.DropTable(
                name: "T_Arc_Events");

            migrationBuilder.DropTable(
                name: "T_Arc_Name_CustomFields");

            migrationBuilder.DropTable(
                name: "T_Arc_Name_Header");

            migrationBuilder.DropTable(
                name: "T_Arc_Sites");

            migrationBuilder.DropTable(
                name: "T_Audit");

            migrationBuilder.DropTable(
                name: "T_Backup");

            migrationBuilder.DropTable(
                name: "T_Bio_Data");

            migrationBuilder.DropTable(
                name: "T_Calendar_Details");

            migrationBuilder.DropTable(
                name: "T_CardManager_Default");

            migrationBuilder.DropTable(
                name: "T_CardManager_OrderByFields");

            migrationBuilder.DropTable(
                name: "T_CardManager_SelectFields");

            migrationBuilder.DropTable(
                name: "T_CardManager_WhereFields");

            migrationBuilder.DropTable(
                name: "T_CardPack_Details");

            migrationBuilder.DropTable(
                name: "T_Commands");

            migrationBuilder.DropTable(
                name: "T_Custom");

            migrationBuilder.DropTable(
                name: "T_Customer");

            migrationBuilder.DropTable(
                name: "T_Display");

            migrationBuilder.DropTable(
                name: "T_EnterpriseData");

            migrationBuilder.DropTable(
                name: "T_Events");

            migrationBuilder.DropTable(
                name: "T_IOController_Details");

            migrationBuilder.DropTable(
                name: "T_Modems");

            migrationBuilder.DropTable(
                name: "T_Name_AccessLevels");

            migrationBuilder.DropTable(
                name: "T_Name_CustomFields");

            migrationBuilder.DropTable(
                name: "T_SpaceZone_Attendance");

            migrationBuilder.DropTable(
                name: "T_SpaceZone_Cardholders");

            migrationBuilder.DropTable(
                name: "T_SpaceZone_Details");

            migrationBuilder.DropTable(
                name: "T_StatusView");

            migrationBuilder.DropTable(
                name: "T_System");

            migrationBuilder.DropTable(
                name: "T_TimeSheet_Zones");

            migrationBuilder.DropTable(
                name: "T_TimeZone_Details");

            migrationBuilder.DropTable(
                name: "T_TimeZone_Header");

            migrationBuilder.DropTable(
                name: "T_Triggers_Controllers");

            migrationBuilder.DropTable(
                name: "T_Triggers_Events");

            migrationBuilder.DropTable(
                name: "T_User_Permissions");

            migrationBuilder.DropTable(
                name: "T_User_Sites");

            migrationBuilder.DropTable(
                name: "T_Userlog");

            migrationBuilder.DropTable(
                name: "T_Users");

            migrationBuilder.DropTable(
                name: "T_CardManager_Header");

            migrationBuilder.DropTable(
                name: "T_CardPack_Header");

            migrationBuilder.DropTable(
                name: "T_CustomFieldTypes");

            migrationBuilder.DropTable(
                name: "T_DisplayTypes");

            migrationBuilder.DropTable(
                name: "T_IOController_Header");

            migrationBuilder.DropTable(
                name: "T_AccessLevel_Header");

            migrationBuilder.DropTable(
                name: "T_Name_Header");

            migrationBuilder.DropTable(
                name: "T_SpaceZone_Header");

            migrationBuilder.DropTable(
                name: "T_TimeSheet_Header");

            migrationBuilder.DropTable(
                name: "T_Calendar_Header");

            migrationBuilder.DropTable(
                name: "T_Triggers_Header");

            migrationBuilder.DropTable(
                name: "T_APBZone_Header");

            migrationBuilder.DropTable(
                name: "T_Doors");

            migrationBuilder.DropTable(
                name: "T_EventTypes");

            migrationBuilder.DropTable(
                name: "T_Connectors");

            migrationBuilder.DropTable(
                name: "T_Door_Technology");

            migrationBuilder.DropTable(
                name: "T_FloorPlans");

            migrationBuilder.DropTable(
                name: "T_Sites");
        }
    }
}
