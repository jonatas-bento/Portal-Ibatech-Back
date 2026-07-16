using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibatech.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirIndiceFinanceiroVenda : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_TransacoesFinanceiras_Vendas_VendaId",
				table: "TransacoesFinanceiras");

			migrationBuilder.DropIndex(
				name: "IX_TransacoesFinanceiras_VendaId",
				table: "TransacoesFinanceiras");

			migrationBuilder.CreateIndex(
				name: "IX_TransacoesFinanceiras_VendaId",
				table: "TransacoesFinanceiras",
				column: "VendaId");

			migrationBuilder.AddForeignKey(
				name: "FK_TransacoesFinanceiras_Vendas_VendaId",
				table: "TransacoesFinanceiras",
				column: "VendaId",
				principalTable: "Vendas",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_TransacoesFinanceiras_Vendas_VendaId",
				table: "TransacoesFinanceiras");

			migrationBuilder.DropIndex(
				name: "IX_TransacoesFinanceiras_VendaId",
				table: "TransacoesFinanceiras");

			migrationBuilder.CreateIndex(
				name: "IX_TransacoesFinanceiras_VendaId",
				table: "TransacoesFinanceiras",
				column: "VendaId",
				unique: true);

			migrationBuilder.AddForeignKey(
				name: "FK_TransacoesFinanceiras_Vendas_VendaId",
				table: "TransacoesFinanceiras",
				column: "VendaId",
				principalTable: "Vendas",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}
	}
}
