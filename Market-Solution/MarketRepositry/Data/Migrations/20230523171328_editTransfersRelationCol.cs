using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketRepositry.Data.Migrations
{
    public partial class editTransfersRelationCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Branchs_ToBranchId",
                table: "Transfers");

            migrationBuilder.AlterColumn<int>(
                name: "FromBranchId",
                table: "Transfers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
                name: "FK_Transfers_Branchs_ToBranchId",
                table: "Transfers");

            migrationBuilder.AlterColumn<int>(
                name: "FromBranchId",
                table: "Transfers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Branchs_ToBranchId",
                table: "Transfers",
                column: "ToBranchId",
                principalTable: "Branchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
