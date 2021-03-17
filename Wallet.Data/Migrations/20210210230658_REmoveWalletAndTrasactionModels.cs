using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wallet.Data.Migrations
{
    public partial class REmoveWalletAndTrasactionModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionHistories");

            migrationBuilder.DropTable(
                name: "WalletAccounts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WalletAccounts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Balance = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userIDID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletAccounts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WalletAccounts_SystemUsers_userIDID",
                        column: x => x.userIDID,
                        principalTable: "SystemUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Txn_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    balance_after = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    balance_before = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    walletIDID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionHistories_WalletAccounts_walletIDID",
                        column: x => x.walletIDID,
                        principalTable: "WalletAccounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_walletIDID",
                table: "TransactionHistories",
                column: "walletIDID");

            migrationBuilder.CreateIndex(
                name: "IX_WalletAccounts_userIDID",
                table: "WalletAccounts",
                column: "userIDID");
        }
    }
}
