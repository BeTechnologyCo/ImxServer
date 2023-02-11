using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ImxServer.Migrations
{
    /// <inheritdoc />
    public partial class InitializeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "MonsterId", "Name" },
                values: new object[,]
                {
                    { 1, "Fordin" },
                    { 2, "Kroki" },
                    { 3, "Devidin" },
                    { 4, "Aerodin" },
                    { 5, "Weastoat" }
                });

            migrationBuilder.InsertData(
                table: "Moves",
                columns: new[] { "MoveId", "Name" },
                values: new object[,]
                {
                    { 1, "Cut" },
                    { 2, "Ember" },
                    { 3, "Growl" },
                    { 4, "PoisonPowder" },
                    { 5, "QuickAttack" },
                    { 6, "SandAttack" },
                    { 7, "Scratch" },
                    { 8, "Sing" },
                    { 9, "SuperSonic" },
                    { 10, "Surf" },
                    { 11, "Tackle" },
                    { 12, "ThunderWave" },
                    { 13, "Vine" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Moves",
                keyColumn: "MoveId",
                keyValue: 13);
        }
    }
}
