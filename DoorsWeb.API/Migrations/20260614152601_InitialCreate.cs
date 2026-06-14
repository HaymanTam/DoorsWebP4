using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoorsWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessCalendar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessCalendar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanEditUsers = table.Column<bool>(type: "bit", nullable: false),
                    CanEditDoors = table.Column<bool>(type: "bit", nullable: false),
                    CanEditAdmins = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoorHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ControllerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IPAddressBytes = table.Column<byte[]>(type: "varbinary(16)", maxLength: 16, nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoorHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventType",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessCalendarElement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AccessCalendarId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessCalendarElement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessCalendarElement_AccessCalendar_AccessCalendarId",
                        column: x => x.AccessCalendarId,
                        principalTable: "AccessCalendar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccessTimeZone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CalendarId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTimeZone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessTimeZone_AccessCalendar_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "AccessCalendar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DoorSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AutoRelockEnable = table.Column<bool>(type: "bit", nullable: false),
                    RelayA_Delay = table.Column<byte>(type: "tinyint", nullable: false),
                    RelayA_Time = table.Column<byte>(type: "tinyint", nullable: false),
                    RelayA_TZOverideEnable = table.Column<bool>(type: "bit", nullable: false),
                    RelayB_Mode = table.Column<byte>(type: "tinyint", nullable: false),
                    RelayB_Delay = table.Column<byte>(type: "tinyint", nullable: false),
                    RelayB_Time = table.Column<byte>(type: "tinyint", nullable: false),
                    PDO_Alarm_Time = table.Column<byte>(type: "tinyint", nullable: false),
                    LockDriveMode = table.Column<byte>(type: "tinyint", nullable: false),
                    ValidFrom = table.Column<TimeOnly>(type: "time", nullable: false),
                    ValidTo = table.Column<TimeOnly>(type: "time", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keypad_VCardLength = table.Column<byte>(type: "tinyint", nullable: false),
                    Keypad_AccessCode = table.Column<int>(type: "int", nullable: true),
                    Keypad_StarModeEnable = table.Column<bool>(type: "bit", nullable: false),
                    ReaderA_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReaderA_TechId = table.Column<byte>(type: "tinyint", nullable: false),
                    ReaderA_Volume = table.Column<byte>(type: "tinyint", nullable: false),
                    ReaderA_Brightness = table.Column<byte>(type: "tinyint", nullable: false),
                    ReaderA_MFA_Sequence = table.Column<byte>(type: "tinyint", nullable: false),
                    ReaderB_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReaderB_TechId = table.Column<byte>(type: "tinyint", nullable: false),
                    ReaderB_Volume = table.Column<byte>(type: "tinyint", nullable: false),
                    ReaderB_Brightness = table.Column<byte>(type: "tinyint", nullable: false),
                    ReaderB_MFA_Sequence = table.Column<byte>(type: "tinyint", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DoorHeader = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoorSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoorSettings_DoorHeader_Id",
                        column: x => x.Id,
                        principalTable: "DoorHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoorStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelayA = table.Column<bool>(type: "bit", nullable: false),
                    RelayB = table.Column<bool>(type: "bit", nullable: false),
                    TwoStageRelease = table.Column<bool>(type: "bit", nullable: false),
                    RequestToExit = table.Column<bool>(type: "bit", nullable: false),
                    DoorPosition = table.Column<bool>(type: "bit", nullable: false),
                    Interlocked = table.Column<bool>(type: "bit", nullable: false),
                    AlarmDoorForced = table.Column<bool>(type: "bit", nullable: false),
                    AlarmDuress = table.Column<bool>(type: "bit", nullable: false),
                    AlarmHacker = table.Column<bool>(type: "bit", nullable: false),
                    AlarmFire = table.Column<bool>(type: "bit", nullable: false),
                    AlarmIntruder = table.Column<bool>(type: "bit", nullable: false),
                    AlarmObstruction = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DoorHeader = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoorStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoorStatus_DoorHeader_Id",
                        column: x => x.Id,
                        principalTable: "DoorHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CardNumber = table.Column<int>(type: "int", nullable: false),
                    DoorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReaderId = table.Column<byte>(type: "tinyint", nullable: true),
                    EventTypeId = table.Column<short>(type: "smallint", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_DoorHeader_DoorId",
                        column: x => x.DoorId,
                        principalTable: "DoorHeader",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Event_EventType_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccessLevel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessLevel_AccessTimeZone_TimeZoneId",
                        column: x => x.TimeZoneId,
                        principalTable: "AccessTimeZone",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccessTimeZoneElement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Monday = table.Column<bool>(type: "bit", nullable: false),
                    Tuesday = table.Column<bool>(type: "bit", nullable: false),
                    Wednesday = table.Column<bool>(type: "bit", nullable: false),
                    Thursday = table.Column<bool>(type: "bit", nullable: false),
                    Friday = table.Column<bool>(type: "bit", nullable: false),
                    Saturday = table.Column<bool>(type: "bit", nullable: false),
                    Sunday = table.Column<bool>(type: "bit", nullable: false),
                    CalendarId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccessTimeZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTimeZoneElement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessTimeZoneElement_AccessCalendar_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "AccessCalendar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccessTimeZoneElement_AccessTimeZone_AccessTimeZoneId",
                        column: x => x.AccessTimeZoneId,
                        principalTable: "AccessTimeZone",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccessLevelElement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessTimeZone = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLevelElement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessLevelElement_AccessLevel_AccessLevelId",
                        column: x => x.AccessLevelId,
                        principalTable: "AccessLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessLevelElement_AccessTimeZone_AccessTimeZone",
                        column: x => x.AccessTimeZone,
                        principalTable: "AccessTimeZone",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccessLevelElement_DoorHeader_DoorId",
                        column: x => x.DoorId,
                        principalTable: "DoorHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessLevelUser",
                columns: table => new
                {
                    AccessLevelsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLevelUser", x => new { x.AccessLevelsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AccessLevelUser_AccessLevel_AccessLevelsId",
                        column: x => x.AccessLevelsId,
                        principalTable: "AccessLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessLevelUser_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Admin",
                columns: new[] { "Id", "CanEditAdmins", "CanEditDoors", "CanEditUsers", "PasswordHash", "Username" },
                values: new object[] { new Guid("a22d0dac-9476-4bc9-a564-77c36963d85e"), true, true, true, "$2a$11$KaHdki2KyqrcPv9inpMeDuisvjAklAdKJeRqJuam.xpCRgjl00CUS", "super" });

            migrationBuilder.InsertData(
                table: "EventType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (short)0, "Card OK" },
                    { (short)1, "Invalid Card" },
                    { (short)2, "Card and PIN OK" },
                    { (short)3, "Invalid PIN" },
                    { (short)4, "Card Otl" },
                    { (short)5, "Card Expired" },
                    { (short)6, "Unknown Card" },
                    { (short)7, "Anti Pass Back Failure" },
                    { (short)8, "Request to Exit" },
                    { (short)9, "Door Stuck Open" },
                    { (short)10, "Fire Alarm" },
                    { (short)11, "Fire Reset" },
                    { (short)12, "Fire Fault" },
                    { (short)13, "Intruder" },
                    { (short)14, "Intruder Reset" },
                    { (short)15, "Intruder Fault" },
                    { (short)16, "Door Forced" },
                    { (short)17, "Door Release by PC" },
                    { (short)18, "Door Pc Close" },
                    { (short)19, "Door Pc Open" },
                    { (short)20, "Tz Door Open" },
                    { (short)21, "Tz Door Close" },
                    { (short)22, "Powered Down" },
                    { (short)23, "Powered Up" },
                    { (short)24, "Door Pc Release B" },
                    { (short)25, "Door Pc Close B" },
                    { (short)26, "Door Pc Open B" },
                    { (short)27, "Tz Door Open B" },
                    { (short)28, "Tz Door Close B" },
                    { (short)29, "Invalid Code" },
                    { (short)30, "Code Ok" },
                    { (short)31, "Premature Card" },
                    { (short)32, "Supply Low Volts" },
                    { (short)33, "Supply High Volts" },
                    { (short)34, "Code Duress" },
                    { (short)35, "Hacker" },
                    { (short)36, "Menu Engineer" },
                    { (short)37, "Menu User" },
                    { (short)38, "Random Search" },
                    { (short)39, "Reader Error" },
                    { (short)46, "Card 4 Pin" },
                    { (short)47, "Pin Timeout" },
                    { (short)48, "Card 4 Code" },
                    { (short)49, "Transaction Timeout" },
                    { (short)50, "Card Code Ok" },
                    { (short)54, "Interlocked" },
                    { (short)55, "Bio Unknown" },
                    { (short)56, "Bio Aborted" },
                    { (short)57, "Bio Con Rx Tmp Err" },
                    { (short)58, "Bio Invalid" },
                    { (short)59, "Bio Timeout" },
                    { (short)60, "Bio Duplicated" },
                    { (short)61, "Bio Rdr Tx Tmp Err" },
                    { (short)62, "Sequence Err" },
                    { (short)63, "Seq Ok" },
                    { (short)64, "No Bio Template" },
                    { (short)65, "Bio Template List" },
                    { (short)66, "Bio Rdr Rx Tmp Err" },
                    { (short)67, "Bio Template Updated" },
                    { (short)68, "Bio Template Deleted" },
                    { (short)69, "Bio Duress" },
                    { (short)70, "Multi Factor Over Ride Tz Open" },
                    { (short)71, "Multi Factor Over Ride Tz Close" },
                    { (short)73, "Telekey Low Battery" },
                    { (short)74, "Call Button" },
                    { (short)200, "Key Beep" },
                    { (short)201, "Key Nokey" },
                    { (short)202, "Starkey Beep" },
                    { (short)203, "Bio Start Enrol" },
                    { (short)204, "Admin Id Ok" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessCalendar_Name",
                table: "AccessCalendar",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccessCalendarElement_AccessCalendarId",
                table: "AccessCalendarElement",
                column: "AccessCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLevel_Name",
                table: "AccessLevel",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccessLevel_TimeZoneId",
                table: "AccessLevel",
                column: "TimeZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLevelElement_AccessLevelId",
                table: "AccessLevelElement",
                column: "AccessLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLevelElement_AccessTimeZone",
                table: "AccessLevelElement",
                column: "AccessTimeZone");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLevelElement_DoorId",
                table: "AccessLevelElement",
                column: "DoorId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLevelUser_UsersId",
                table: "AccessLevelUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTimeZone_CalendarId",
                table: "AccessTimeZone",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTimeZone_Name",
                table: "AccessTimeZone",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccessTimeZoneElement_AccessTimeZoneId",
                table: "AccessTimeZoneElement",
                column: "AccessTimeZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTimeZoneElement_CalendarId",
                table: "AccessTimeZoneElement",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_Admin_Username",
                table: "Admin",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoorHeader_ControllerId",
                table: "DoorHeader",
                column: "ControllerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_DoorId",
                table: "Event",
                column: "DoorId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventTypeId",
                table: "Event",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_UserId",
                table: "Event",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_CardNumber",
                table: "User",
                column: "CardNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Name",
                table: "User",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessCalendarElement");

            migrationBuilder.DropTable(
                name: "AccessLevelElement");

            migrationBuilder.DropTable(
                name: "AccessLevelUser");

            migrationBuilder.DropTable(
                name: "AccessTimeZoneElement");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "DoorSettings");

            migrationBuilder.DropTable(
                name: "DoorStatus");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "AccessLevel");

            migrationBuilder.DropTable(
                name: "DoorHeader");

            migrationBuilder.DropTable(
                name: "EventType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "AccessTimeZone");

            migrationBuilder.DropTable(
                name: "AccessCalendar");
        }
    }
}
