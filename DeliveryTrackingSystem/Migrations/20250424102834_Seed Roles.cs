using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeliveryTrackingSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0c20a355-12dd-449d-a8d5-6e33960c45ee", "0c20a355-12dd-449d-a8d5-6e33960c45ee", "Employee", "EMPLOYEE" },
                    { "393f1091-b175-4cca-a1df-19971e86e2a3", "393f1091-b175-4cca-a1df-19971e86e2a3", "Driver", "DRIVER" },
                    { "7d090697-295a-43bf-bb0b-3a19843fb528", "7d090697-295a-43bf-bb0b-3a19843fb528", "Admin", "ADMIN" },
                    { "8b2fbfe2-0a51-4f8e-b57f-4504d20a3739", "8b2fbfe2-0a51-4f8e-b57f-4504d20a3739", "SuperAdmin", "SUPERADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0c20a355-12dd-449d-a8d5-6e33960c45ee");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "393f1091-b175-4cca-a1df-19971e86e2a3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7d090697-295a-43bf-bb0b-3a19843fb528");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8b2fbfe2-0a51-4f8e-b57f-4504d20a3739");
        }
    }
}
