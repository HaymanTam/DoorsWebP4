using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DoorsWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class DropCardDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Name_Header_T_CardDesign_Header_CardDesign",
                table: "T_Name_Header");

            migrationBuilder.DropTable(
                name: "T_CardDesign_Details");

            migrationBuilder.DropTable(
                name: "T_CardDesign_Header");

            migrationBuilder.DropIndex(
                name: "IX_T_Name_Header_CardDesign",
                table: "T_Name_Header");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_CardDesign_Header",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    Orientation = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardDesign_Header", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "T_CardDesign_Details",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    Alignment = table.Column<int>(type: "integer", nullable: false),
                    Bold = table.Column<bool>(type: "boolean", nullable: false),
                    Colour = table.Column<int>(type: "integer", nullable: false),
                    FontName = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false),
                    FontSize = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Italic = table.Column<bool>(type: "boolean", nullable: false),
                    Left = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    Top = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Underline = table.Column<bool>(type: "boolean", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardDesign_Details", x => new { x.Code, x.Sequence });
                    table.ForeignKey(
                        name: "FK_T_CardDesign_Details_T_CardDesign_Header_Code",
                        column: x => x.Code,
                        principalTable: "T_CardDesign_Header",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Name_Header_CardDesign",
                table: "T_Name_Header",
                column: "CardDesign");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Name_Header_T_CardDesign_Header_CardDesign",
                table: "T_Name_Header",
                column: "CardDesign",
                principalTable: "T_CardDesign_Header",
                principalColumn: "Code");
        }
    }
}
