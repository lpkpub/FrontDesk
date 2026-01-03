using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FrontDesk.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    pin = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    user_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "visit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    exit_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    company = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_visit", x => x.id);
                    table.ForeignKey(
                        name: "fk_visit_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "is_active", "name", "pin", "user_type" },
                values: new object[,]
                {
                    { new Guid("b2d8f9a4-1a3e-4e6f-8c02-000000000001"), true, "Office Worker", "111111", "Staff" },
                    { new Guid("b2d8f9a4-1a3e-4e6f-8c02-000000000002"), true, "Warehouse Worker", "222222", "Staff" },
                    { new Guid("b2d8f9a4-1a3e-4e6f-8c02-000000000003"), false, "Casual Worker", "333333", "Staff" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_pin",
                table: "user",
                column: "pin");

            migrationBuilder.CreateIndex(
                name: "ix_visit_user_id",
                table: "visit",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "visit");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
