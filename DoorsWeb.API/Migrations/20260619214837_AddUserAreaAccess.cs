using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoorsWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAreaAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardManagerAccess",
                table: "T_Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SiteSettingsAccess",
                table: "T_Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserSettingsAccess",
                table: "T_Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "T_Users",
                keyColumn: "Code",
                keyValue: 1,
                columns: new[] { "CardManagerAccess", "SiteSettingsAccess", "UserSettingsAccess" },
                values: new object[] { 0, 0, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardManagerAccess",
                table: "T_Users");

            migrationBuilder.DropColumn(
                name: "SiteSettingsAccess",
                table: "T_Users");

            migrationBuilder.DropColumn(
                name: "UserSettingsAccess",
                table: "T_Users");
        }
    }
}
