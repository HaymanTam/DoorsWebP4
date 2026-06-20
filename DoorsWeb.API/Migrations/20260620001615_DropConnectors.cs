using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoorsWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class DropConnectors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Commands_T_Connectors_Connector",
                table: "T_Commands");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Doors_T_Connectors_Connector",
                table: "T_Doors");

            migrationBuilder.DropForeignKey(
                name: "FK_T_IOController_Header_T_Connectors_Connector",
                table: "T_IOController_Header");

            migrationBuilder.DropTable(
                name: "T_Connectors");

            migrationBuilder.DropIndex(
                name: "IX_T_IOController_Header_Connector",
                table: "T_IOController_Header");

            migrationBuilder.DropIndex(
                name: "IX_T_Doors_Connector",
                table: "T_Doors");

            migrationBuilder.DropIndex(
                name: "IX_T_Commands_Connector",
                table: "T_Commands");

            migrationBuilder.DropColumn(
                name: "Connector",
                table: "T_IOController_Header");

            migrationBuilder.DropColumn(
                name: "Connector",
                table: "T_Doors");

            migrationBuilder.DropColumn(
                name: "Connector",
                table: "T_Commands");

            migrationBuilder.DropColumn(
                name: "Connector",
                table: "T_Arc_Doors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Connector",
                table: "T_IOController_Header",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Connector",
                table: "T_Doors",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Connector",
                table: "T_Commands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Connector",
                table: "T_Arc_Doors",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "T_Connectors",
                columns: table => new
                {
                    Connector = table.Column<int>(type: "integer", nullable: false),
                    Site = table.Column<int>(type: "integer", nullable: true),
                    ComPort = table.Column<int>(type: "integer", nullable: true),
                    CommandFrequency = table.Column<int>(type: "integer", nullable: true),
                    ConnType = table.Column<int>(type: "integer", nullable: true),
                    DownloadLogsWhenUpdating = table.Column<bool>(type: "boolean", nullable: true),
                    Encrypted = table.Column<bool>(type: "boolean", nullable: true),
                    ForceDistrib = table.Column<bool>(type: "boolean", nullable: true),
                    ForcePing = table.Column<int>(type: "integer", nullable: true),
                    ForceTimeSync = table.Column<bool>(type: "boolean", nullable: true),
                    Inuse = table.Column<bool>(type: "boolean", nullable: true),
                    IPAddress = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ModemConnectionFrequency = table.Column<int>(type: "integer", nullable: true),
                    ModemConnectionPeriod = table.Column<int>(type: "integer", nullable: true),
                    ModemConnectionTime = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    ModemDownloadLogs = table.Column<bool>(type: "boolean", nullable: true),
                    ModemStayConnected = table.Column<bool>(type: "boolean", nullable: true),
                    ModemUploadCommands = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PABX = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PingFrequency = table.Column<int>(type: "integer", nullable: true),
                    Retries = table.Column<int>(type: "integer", nullable: true),
                    RetryInterval = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Telnumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_T_IOController_Header_Connector",
                table: "T_IOController_Header",
                column: "Connector");

            migrationBuilder.CreateIndex(
                name: "IX_T_Doors_Connector",
                table: "T_Doors",
                column: "Connector");

            migrationBuilder.CreateIndex(
                name: "IX_T_Commands_Connector",
                table: "T_Commands",
                column: "Connector");

            migrationBuilder.CreateIndex(
                name: "IX_T_Connectors_Site",
                table: "T_Connectors",
                column: "Site");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Commands_T_Connectors_Connector",
                table: "T_Commands",
                column: "Connector",
                principalTable: "T_Connectors",
                principalColumn: "Connector");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Doors_T_Connectors_Connector",
                table: "T_Doors",
                column: "Connector",
                principalTable: "T_Connectors",
                principalColumn: "Connector");

            migrationBuilder.AddForeignKey(
                name: "FK_T_IOController_Header_T_Connectors_Connector",
                table: "T_IOController_Header",
                column: "Connector",
                principalTable: "T_Connectors",
                principalColumn: "Connector");
        }
    }
}
