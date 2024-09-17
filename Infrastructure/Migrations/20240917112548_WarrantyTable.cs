using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WarrantyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WarrantyId",
                table: "Valuables",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Warranty",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warranty", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Valuables_WarrantyId",
                table: "Valuables",
                column: "WarrantyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Valuables_Warranty_WarrantyId",
                table: "Valuables",
                column: "WarrantyId",
                principalTable: "Warranty",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Valuables_Warranty_WarrantyId",
                table: "Valuables");

            migrationBuilder.DropTable(
                name: "Warranty");

            migrationBuilder.DropIndex(
                name: "IX_Valuables_WarrantyId",
                table: "Valuables");

            migrationBuilder.DropColumn(
                name: "WarrantyId",
                table: "Valuables");
        }
    }
}
