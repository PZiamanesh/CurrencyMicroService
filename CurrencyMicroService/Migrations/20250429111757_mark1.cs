using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyMicroService.Migrations
{
    /// <inheritdoc />
    public partial class mark1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SettingKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ETSCurrencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CashBuy = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashSell = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransferBuy = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransferSell = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EssentialGoodsBuy = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EssentialGoodsSell = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeightedAverage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FetchDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETSCurrencies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSettings_SettingKey",
                table: "ApplicationSettings",
                column: "SettingKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropTable(
                name: "ETSCurrencies");
        }
    }
}
