using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreGoDelivery.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnforceBusinessUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tb_rental_ID_FK_MOTORCYCLE",
                schema: "dbgodelivery",
                table: "tb_rental");

            migrationBuilder.CreateIndex(
                name: "UX_tb_rental_active_motorcycle",
                schema: "dbgodelivery",
                table: "tb_rental",
                column: "ID_FK_MOTORCYCLE",
                unique: true,
                filter: "\"DATE_RETURNED_TO_BASE\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "UX_tb_motorcycle_plate_normalized",
                schema: "dbgodelivery",
                table: "tb_motorcycle",
                column: "PLATE_NORMALIZED",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_tb_deliverier_cnpj",
                schema: "dbgodelivery",
                table: "tb_deliverier",
                column: "CNPJ",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_tb_rental_active_motorcycle",
                schema: "dbgodelivery",
                table: "tb_rental");

            migrationBuilder.DropIndex(
                name: "UX_tb_motorcycle_plate_normalized",
                schema: "dbgodelivery",
                table: "tb_motorcycle");

            migrationBuilder.DropIndex(
                name: "UX_tb_deliverier_cnpj",
                schema: "dbgodelivery",
                table: "tb_deliverier");

            migrationBuilder.CreateIndex(
                name: "IX_tb_rental_ID_FK_MOTORCYCLE",
                schema: "dbgodelivery",
                table: "tb_rental",
                column: "ID_FK_MOTORCYCLE");
        }
    }
}
