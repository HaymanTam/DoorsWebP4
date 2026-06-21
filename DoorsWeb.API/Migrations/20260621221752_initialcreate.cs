using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DoorsWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "T_Backup");

            migrationBuilder.DropTable(
                name: "T_Bio_Data");

            migrationBuilder.DropTable(
                name: "T_Commands");

            migrationBuilder.DropTable(
                name: "T_Customer");

            migrationBuilder.DropTable(
                name: "T_EnterpriseData");

            migrationBuilder.DropTable(
                name: "T_Modems");

            migrationBuilder.DropTable(
                name: "T_StatusView");

            migrationBuilder.DropTable(
                name: "T_System");

            migrationBuilder.DropTable(
                name: "T_Userlog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Arc_Custom",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    CustomFieldCode = table.Column<int>(type: "integer", nullable: false),
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
                    DataType = table.Column<int>(type: "integer", nullable: true),
                    Literal = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Arc_Doors",
                columns: table => new
                {
                    AccessCode_Dig1 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig2 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig3 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig4 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig5 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig6 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig7 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Dig8 = table.Column<int>(type: "integer", nullable: true),
                    AccessCode_Len = table.Column<int>(type: "integer", nullable: true),
                    AlarmZoneNumber = table.Column<int>(type: "integer", nullable: true),
                    AutoDelayVal = table.Column<int>(type: "integer", nullable: true),
                    AutoRelock = table.Column<bool>(type: "boolean", nullable: true),
                    Bio_Enrol_A = table.Column<bool>(type: "boolean", nullable: true),
                    Bio_Enrol_B = table.Column<bool>(type: "boolean", nullable: true),
                    CarIn = table.Column<bool>(type: "boolean", nullable: true),
                    CarOut = table.Column<bool>(type: "boolean", nullable: true),
                    CardandPINTimeZone = table.Column<int>(type: "integer", nullable: true),
                    CON_ALM_Volume = table.Column<int>(type: "integer", nullable: true),
                    CON_FB_Volume = table.Column<int>(type: "integer", nullable: true),
                    ControllerID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    ControllerIP = table.Column<bool>(type: "boolean", nullable: true),
                    Door = table.Column<int>(type: "integer", nullable: false),
                    DoorIPAddress = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DoorType = table.Column<int>(type: "integer", nullable: true),
                    FloorPlan = table.Column<int>(type: "integer", nullable: true),
                    FloorPlanX = table.Column<int>(type: "integer", nullable: true),
                    FloorPlanY = table.Column<int>(type: "integer", nullable: true),
                    ID_Sequence_A = table.Column<int>(type: "integer", nullable: true),
                    ID_Sequence_B = table.Column<int>(type: "integer", nullable: true),
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    KeyboardName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    KeyboardTech = table.Column<int>(type: "integer", nullable: true),
                    Keypad_Star_Mode = table.Column<int>(type: "integer", nullable: true),
                    LastCard = table.Column<int>(type: "integer", nullable: true),
                    LastCardID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    LastDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastEvent = table.Column<int>(type: "integer", nullable: true),
                    LocalDoorNumber = table.Column<int>(type: "integer", nullable: true),
                    Lock_Drive_Mode = table.Column<int>(type: "integer", nullable: true),
                    LogCount = table.Column<int>(type: "integer", nullable: true),
                    LogInA = table.Column<bool>(type: "boolean", nullable: true),
                    LogInB = table.Column<bool>(type: "boolean", nullable: true),
                    LogUpdateInterval1 = table.Column<int>(type: "integer", nullable: true),
                    LogUpdateInterval2 = table.Column<int>(type: "integer", nullable: true),
                    LogoutA = table.Column<bool>(type: "boolean", nullable: true),
                    LogoutB = table.Column<bool>(type: "boolean", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PDO = table.Column<int>(type: "integer", nullable: true),
                    PlanNumber = table.Column<int>(type: "integer", nullable: true),
                    Random_Search_Freq = table.Column<int>(type: "integer", nullable: true),
                    RDR_Brightness_A = table.Column<int>(type: "integer", nullable: true),
                    RDR_Brightness_B = table.Column<int>(type: "integer", nullable: true),
                    RDR_Volume_A = table.Column<int>(type: "integer", nullable: true),
                    RDR_Volume_B = table.Column<int>(type: "integer", nullable: true),
                    ReaderAName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ReaderBName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RelayBmode = table.Column<int>(type: "integer", nullable: true),
                    RelayBTimeZone = table.Column<int>(type: "integer", nullable: true),
                    ReleaseDelay = table.Column<int>(type: "integer", nullable: true),
                    ReleaseTime = table.Column<int>(type: "integer", nullable: true),
                    ReleaseTimeB = table.Column<int>(type: "integer", nullable: true),
                    RTCDate = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    RTCTime = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    Site = table.Column<int>(type: "integer", nullable: true),
                    Status1 = table.Column<int>(type: "integer", nullable: true),
                    Status2 = table.Column<int>(type: "integer", nullable: true),
                    StatusUpdateInterval1 = table.Column<int>(type: "integer", nullable: true),
                    StatusUpdateInterval2 = table.Column<int>(type: "integer", nullable: true),
                    TechnologyA = table.Column<int>(type: "integer", nullable: true),
                    TechnologyB = table.Column<int>(type: "integer", nullable: true),
                    TimeLock = table.Column<int>(type: "integer", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValidFromTimeHH = table.Column<int>(type: "integer", nullable: true),
                    ValidFromTimeMM = table.Column<int>(type: "integer", nullable: true),
                    ValidToTimeHH = table.Column<int>(type: "integer", nullable: true),
                    ValidToTimeMM = table.Column<int>(type: "integer", nullable: true),
                    VdiskDirectories = table.Column<int>(type: "integer", nullable: true),
                    XPlace = table.Column<float>(type: "real", nullable: true),
                    YPlace = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Arc_Events",
                columns: table => new
                {
                    ActualCardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    AlarmID = table.Column<int>(type: "integer", nullable: true),
                    CardNumber = table.Column<int>(type: "integer", nullable: false),
                    DoorNumber = table.Column<int>(type: "integer", nullable: false),
                    EventDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EventID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    ReaderID = table.Column<int>(type: "integer", nullable: false)
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
                    Custom2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom20 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom21 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom22 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom23 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom24 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom25 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom3 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom4 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom5 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom6 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom7 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom8 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Custom9 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Arc_Name_Header",
                columns: table => new
                {
                    APBDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    APBNumber = table.Column<int>(type: "integer", nullable: true),
                    BioAdmin = table.Column<bool>(type: "boolean", nullable: true),
                    BioOptOut = table.Column<bool>(type: "boolean", nullable: true),
                    CardDesign = table.Column<int>(type: "integer", nullable: true),
                    CardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    CardNumber = table.Column<int>(type: "integer", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: true),
                    Flexi = table.Column<bool>(type: "boolean", nullable: true),
                    Forname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    HotStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    IDCardDesign = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    InUse = table.Column<bool>(type: "boolean", nullable: true),
                    LastDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastDoor = table.Column<int>(type: "integer", nullable: true),
                    LastEvent = table.Column<int>(type: "integer", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OldCardID = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    PIN = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    PinRequired = table.Column<int>(type: "integer", nullable: true),
                    Rollcall = table.Column<int>(type: "integer", nullable: true),
                    Surname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UpdatePending = table.Column<bool>(type: "boolean", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValidFromOverride = table.Column<int>(type: "integer", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValidToOverride = table.Column<int>(type: "integer", nullable: true),
                    Void = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Arc_Sites",
                columns: table => new
                {
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Site = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Backup",
                columns: table => new
                {
                    DeleteToRecycleBin = table.Column<bool>(type: "boolean", nullable: false),
                    IncludePhotos = table.Column<bool>(type: "boolean", nullable: false),
                    IncludePlans = table.Column<bool>(type: "boolean", nullable: false),
                    KeepNumber = table.Column<int>(type: "integer", nullable: false),
                    KeepOn = table.Column<bool>(type: "boolean", nullable: false),
                    LastBackup = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MaximumNumber = table.Column<int>(type: "integer", nullable: false),
                    MaximumOn = table.Column<bool>(type: "boolean", nullable: false),
                    SaveDays = table.Column<int>(type: "integer", nullable: false),
                    ScheduleFrequency = table.Column<int>(type: "integer", nullable: false),
                    ScheduleNumber = table.Column<int>(type: "integer", nullable: false),
                    ScheduleOn = table.Column<bool>(type: "boolean", nullable: false),
                    ScheduleTime = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false)
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
                    Bio_Template_Left = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Bio_Template_Right = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Finger_Left = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Finger_Right = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    ID = table.Column<string>(type: "character varying(12)", unicode: false, maxLength: 12, nullable: true),
                    Status = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bio_Data", x => x.Slot);
                });

            migrationBuilder.CreateTable(
                name: "T_Commands",
                columns: table => new
                {
                    CommandID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    B1 = table.Column<short>(type: "smallint", nullable: true),
                    B10 = table.Column<short>(type: "smallint", nullable: true),
                    B11 = table.Column<short>(type: "smallint", nullable: true),
                    B12 = table.Column<short>(type: "smallint", nullable: true),
                    B13 = table.Column<short>(type: "smallint", nullable: true),
                    B14 = table.Column<short>(type: "smallint", nullable: true),
                    B15 = table.Column<short>(type: "smallint", nullable: true),
                    B16 = table.Column<short>(type: "smallint", nullable: true),
                    B2 = table.Column<short>(type: "smallint", nullable: true),
                    B3 = table.Column<short>(type: "smallint", nullable: true),
                    B4 = table.Column<short>(type: "smallint", nullable: true),
                    B5 = table.Column<short>(type: "smallint", nullable: true),
                    B6 = table.Column<short>(type: "smallint", nullable: true),
                    B7 = table.Column<short>(type: "smallint", nullable: true),
                    B8 = table.Column<short>(type: "smallint", nullable: true),
                    B9 = table.Column<short>(type: "smallint", nullable: true),
                    Bol1 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol2 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol3 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol4 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol5 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol6 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol7 = table.Column<bool>(type: "boolean", nullable: true),
                    Bol8 = table.Column<bool>(type: "boolean", nullable: true),
                    Command = table.Column<int>(type: "integer", nullable: true),
                    ControllerID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    Data = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EngFunctionNumber = table.Column<int>(type: "integer", nullable: true),
                    LevelA = table.Column<int>(type: "integer", nullable: true),
                    LevelB = table.Column<int>(type: "integer", nullable: true),
                    LevelC = table.Column<int>(type: "integer", nullable: true),
                    LevelD = table.Column<int>(type: "integer", nullable: true),
                    NewID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    NewQty = table.Column<int>(type: "integer", nullable: true),
                    OldID = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    OldQty = table.Column<int>(type: "integer", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    ValidFrom = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    ValidTo = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => x.CommandID);
                });

            migrationBuilder.CreateTable(
                name: "T_Customer",
                columns: table => new
                {
                    Comments = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    CommissionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CustomerAddress1 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerAddress2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerAddress3 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerAddress4 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerCompany = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerContact = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerCountry = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerFax = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerPostCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CustomerTelephone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Customeremail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InstallType = table.Column<int>(type: "integer", nullable: false),
                    InstallationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InstallerAddress1 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerAddress2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerAddress3 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerAddress4 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerCompany = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerContact = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerCountry = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerFax = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InstallerPostCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    InstallerTelephone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Installeremail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductKey = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false)
                },
                constraints: table =>
                {
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
                name: "T_StatusView",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Display = table.Column<int>(type: "integer", nullable: true),
                    Move = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Pause = table.Column<int>(type: "integer", nullable: true),
                    ShowAllFloors = table.Column<bool>(type: "boolean", nullable: true),
                    StatusView = table.Column<int>(type: "integer", nullable: true),
                    StatusViewX = table.Column<int>(type: "integer", nullable: true),
                    StatusViewY = table.Column<int>(type: "integer", nullable: true),
                    StatusViewZ = table.Column<int>(type: "integer", nullable: true),
                    UpdateChangesWhenPanning = table.Column<bool>(type: "boolean", nullable: true),
                    Wait = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusView", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "T_System",
                columns: table => new
                {
                    AutoConfigUpdate = table.Column<bool>(type: "boolean", nullable: true),
                    Corporate1000Code = table.Column<string>(type: "character varying(4)", unicode: false, maxLength: 4, nullable: true),
                    CurrentVersion = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    EnableFlexi = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_Userlog",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(28)", maxLength: 28, nullable: false),
                    ID = table.Column<string>(type: "character varying(28)", maxLength: 28, nullable: true),
                    LogInDateTime = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    LogInEvent = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LogInPoint = table.Column<string>(type: "character varying(28)", maxLength: 28, nullable: true),
                    User = table.Column<string>(type: "character varying(28)", maxLength: 28, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Userlog", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IND_Slot",
                table: "T_Bio_Data",
                column: "Slot",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IND_ControllerID",
                table: "T_Commands",
                column: "ControllerID");
        }
    }
}
