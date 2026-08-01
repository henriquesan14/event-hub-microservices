using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseSaga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseStates",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ReservationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OrderCreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PaymentCompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReservationConfirmedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TicketsIssuedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseStates", x => x.CorrelationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseStates_CurrentState_ExpiresAt",
                table: "PurchaseStates",
                columns: new[] { "CurrentState", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseStates_OrderId",
                table: "PurchaseStates",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseStates_PaymentId",
                table: "PurchaseStates",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseStates_ReservationId",
                table: "PurchaseStates",
                column: "ReservationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseStates");
        }
    }
}
