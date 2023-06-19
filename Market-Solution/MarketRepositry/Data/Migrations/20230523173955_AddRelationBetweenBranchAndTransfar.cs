using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketRepositry.Data.Migrations
{
    public partial class AddRelationBetweenBranchAndTransfar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transfers_FromBranchId",
                table: "Transfers",
                column: "FromBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ToBranchId",
                table: "Transfers",
                column: "ToBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Branchs_FromBranchId",
                table: "Transfers",
                column: "FromBranchId",
                principalTable: "Branchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Branchs_ToBranchId",
                table: "Transfers",
                column: "ToBranchId",
                principalTable: "Branchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Branchs_FromBranchId",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Branchs_ToBranchId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_FromBranchId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_ToBranchId",
                table: "Transfers");
        }
    }
}
