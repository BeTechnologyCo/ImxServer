using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImxServer.Migrations
{
    /// <inheritdoc />
    public partial class MonsterExp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Exp",
                table: "Tokens",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exp",
                table: "Tokens");
        }
    }
}
