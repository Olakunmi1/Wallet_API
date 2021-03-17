using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Data.Migrations
{
    public partial class StillOnThisForeginKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_WalletAccounts_walletID",
                table: "TransactionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletAccounts_SystemUsers_userID",
                table: "WalletAccounts");

            migrationBuilder.DropIndex(
                name: "IX_WalletAccounts_userID",
                table: "WalletAccounts");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistories_walletID",
                table: "TransactionHistories");

            migrationBuilder.AlterColumn<int>(
                name: "userID",
                table: "WalletAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "walletID",
                table: "TransactionHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "userID",
                table: "WalletAccounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "walletID",
                table: "TransactionHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_WalletAccounts_userID",
                table: "WalletAccounts",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_walletID",
                table: "TransactionHistories",
                column: "walletID");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_WalletAccounts_walletID",
                table: "TransactionHistories",
                column: "walletID",
                principalTable: "WalletAccounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletAccounts_SystemUsers_userID",
                table: "WalletAccounts",
                column: "userID",
                principalTable: "SystemUsers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
