using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundRequestedAt",
                table: "payments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundedAt",
                table: "payments",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "RefundRequestedAt",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "RefundedAt",
                table: "payments");
        }
    }
}
