using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Data.Migrations
{
    public partial class ForeingKeyChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_WalletAccounts_wallet_IdID",
                table: "TransactionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletAccounts_SystemUsers_user_IdID",
                table: "WalletAccounts");

            migrationBuilder.RenameColumn(
                name: "user_IdID",
                table: "WalletAccounts",
                newName: "userID");

            migrationBuilder.RenameIndex(
                name: "IX_WalletAccounts_user_IdID",
                table: "WalletAccounts",
                newName: "IX_WalletAccounts_userID");

            migrationBuilder.RenameColumn(
                name: "wallet_IdID",
                table: "TransactionHistories",
                newName: "walletID");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionHistories_wallet_IdID",
                table: "TransactionHistories",
                newName: "IX_TransactionHistories_walletID");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_WalletAccounts_walletIDID",
                table: "TransactionHistories",
                column: "walletID",
                principalTable: "WalletAccounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletAccounts_SystemUsers_userIDID",
                table: "WalletAccounts",
                column: "userID",
                principalTable: "SystemUsers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_WalletAccounts_walletIDID",
                table: "TransactionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletAccounts_SystemUsers_userIDID",
                table: "WalletAccounts");

            migrationBuilder.RenameColumn(
                name: "userIDID",
                table: "WalletAccounts",
                newName: "user_IdID");

            migrationBuilder.RenameIndex(
                name: "IX_WalletAccounts_userIDID",
                table: "WalletAccounts",
                newName: "IX_WalletAccounts_user_IdID");

            migrationBuilder.RenameColumn(
                name: "walletIDID",
                table: "TransactionHistories",
                newName: "wallet_IdID");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionHistories_walletIDID",
                table: "TransactionHistories",
                newName: "IX_TransactionHistories_wallet_IdID");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_WalletAccounts_wallet_IdID",
                table: "TransactionHistories",
                column: "wallet_IdID",
                principalTable: "WalletAccounts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletAccounts_SystemUsers_user_IdID",
                table: "WalletAccounts",
                column: "user_IdID",
                principalTable: "SystemUsers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
