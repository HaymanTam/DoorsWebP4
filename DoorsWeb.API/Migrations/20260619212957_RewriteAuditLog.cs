using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoorsWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class RewriteAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessLevels",
                table: "T_Audit");

            migrationBuilder.DropColumn(
                name: "CardID",
                table: "T_Audit");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "T_Audit");

            migrationBuilder.DropColumn(
                name: "Forename",
                table: "T_Audit");

            migrationBuilder.DropColumn(
                name: "SavedBy",
                table: "T_Audit");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "T_Audit");

            migrationBuilder.RenameColumn(
                name: "Workstation",
                table: "T_Audit",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "SaveDate",
                table: "T_Audit",
                newName: "Timestamp");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "T_Audit",
                type: "character varying(10)",
                unicode: false,
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientIp",
                table: "T_Audit",
                type: "character varying(45)",
                unicode: false,
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityKey",
                table: "T_Audit",
                type: "character varying(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                table: "T_Audit",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "T_Audit",
                type: "character varying(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_T_Audit_Timestamp",
                table: "T_Audit",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_T_Audit_Timestamp",
                table: "T_Audit");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "T_Audit");

            migrationBuilder.DropColumn(
                name: "ClientIp",
                table: "T_Audit");

            migrationBuilder.DropColumn(
                name: "EntityKey",
                table: "T_Audit");

            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "T_Audit");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "T_Audit");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "T_Audit",
                newName: "Workstation");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "T_Audit",
                newName: "SaveDate");

            migrationBuilder.AddColumn<string>(
                name: "AccessLevels",
                table: "T_Audit",
                type: "character varying(1000)",
                unicode: false,
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CardID",
                table: "T_Audit",
                type: "character varying(8)",
                unicode: false,
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "T_Audit",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Forename",
                table: "T_Audit",
                type: "character varying(60)",
                unicode: false,
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SavedBy",
                table: "T_Audit",
                type: "character varying(60)",
                unicode: false,
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "T_Audit",
                type: "character varying(60)",
                unicode: false,
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }
    }
}
