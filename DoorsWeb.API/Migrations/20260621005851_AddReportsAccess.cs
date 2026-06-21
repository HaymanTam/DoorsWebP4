using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoorsWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReportsAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportsAccess",
                table: "T_Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportsAccess",
                table: "T_Users");
        }
    }
}
