using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoorsWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class DropCardholderOldCardId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldCardID",
                table: "T_Name_Header");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OldCardID",
                table: "T_Name_Header",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);
        }
    }
}
