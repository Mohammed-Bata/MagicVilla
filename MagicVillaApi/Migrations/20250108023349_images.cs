using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVillaApi.Migrations
{
    /// <inheritdoc />
    public partial class images : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageLocalPath",
                table: "Villas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ImageLocalPath", "ImageUrl" },
                values: new object[] { new DateTime(2025, 1, 8, 4, 33, 48, 245, DateTimeKind.Local).AddTicks(2372), null, "villa3.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageLocalPath", "ImageUrl" },
                values: new object[] { new DateTime(2025, 1, 8, 4, 33, 48, 245, DateTimeKind.Local).AddTicks(2508), null, "villa1.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "ImageLocalPath", "ImageUrl" },
                values: new object[] { new DateTime(2025, 1, 8, 4, 33, 48, 245, DateTimeKind.Local).AddTicks(2515), null, "villa4.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "ImageLocalPath", "ImageUrl" },
                values: new object[] { new DateTime(2025, 1, 8, 4, 33, 48, 245, DateTimeKind.Local).AddTicks(2521), null, "villa5.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "ImageLocalPath", "ImageUrl" },
                values: new object[] { new DateTime(2025, 1, 8, 4, 33, 48, 245, DateTimeKind.Local).AddTicks(2527), null, "villa2.jpg" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageLocalPath",
                table: "Villas");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTime(2024, 8, 25, 0, 49, 52, 4, DateTimeKind.Local).AddTicks(6641), "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTime(2024, 8, 25, 0, 49, 52, 4, DateTimeKind.Local).AddTicks(6735), "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTime(2024, 8, 25, 0, 49, 52, 4, DateTimeKind.Local).AddTicks(6742), "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTime(2024, 8, 25, 0, 49, 52, 4, DateTimeKind.Local).AddTicks(6748), "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTime(2024, 8, 25, 0, 49, 52, 4, DateTimeKind.Local).AddTicks(6754), "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg" });
        }
    }
}
