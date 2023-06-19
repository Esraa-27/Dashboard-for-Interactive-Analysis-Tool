using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketRepositry.Data.Migrations
{
    public partial class transfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Branchs_TransferFromId",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Branchs_TransferToId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_TransferFromId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_TransferToId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "TransferFromId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "TransferToId",
                table: "Transfers");

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
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.AddColumn<int>(
                name: "TransferFromId",
                table: "Transfers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransferToId",
                table: "Transfers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferFromId",
                table: "Transfers",
                column: "TransferFromId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferToId",
                table: "Transfers",
                column: "TransferToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Branchs_TransferFromId",
                table: "Transfers",
                column: "TransferFromId",
                principalTable: "Branchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Branchs_TransferToId",
                table: "Transfers",
                column: "TransferToId",
                principalTable: "Branchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
