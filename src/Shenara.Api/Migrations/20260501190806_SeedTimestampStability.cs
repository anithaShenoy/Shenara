using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shenara.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedTimestampStability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 1, 19, 8, 5, 119, DateTimeKind.Unspecified).AddTicks(2752), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 1, 19, 8, 5, 119, DateTimeKind.Unspecified).AddTicks(2756), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 1, 19, 8, 5, 119, DateTimeKind.Unspecified).AddTicks(8131), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 1, 19, 8, 5, 119, DateTimeKind.Unspecified).AddTicks(8132), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 1, 19, 8, 5, 119, DateTimeKind.Unspecified).AddTicks(8141), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 1, 19, 8, 5, 119, DateTimeKind.Unspecified).AddTicks(8142), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(7681), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(7684), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(9167), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(9168), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(9172), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(9173), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}
