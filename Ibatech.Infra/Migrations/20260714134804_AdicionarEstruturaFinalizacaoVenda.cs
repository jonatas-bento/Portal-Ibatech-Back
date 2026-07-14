using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibatech.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarEstruturaFinalizacaoVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFinalizacao",
                table: "Vendas",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormaPagamento",
                table: "Vendas",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Troco",
                table: "Vendas",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorRecebido",
                table: "Vendas",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VendaId",
                table: "TransacoesFinanceiras",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "VendaId",
                table: "Movimentacoes",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_TransacoesFinanceiras_VendaId",
                table: "TransacoesFinanceiras",
                column: "VendaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacoes_VendaId",
                table: "Movimentacoes",
                column: "VendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentacoes_Vendas_VendaId",
                table: "Movimentacoes",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Movimentacoes_Vendas_VendaId",
                table: "Movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_TransacoesFinanceiras_Vendas_VendaId",
                table: "TransacoesFinanceiras");

            migrationBuilder.DropIndex(
                name: "IX_TransacoesFinanceiras_VendaId",
                table: "TransacoesFinanceiras");

            migrationBuilder.DropIndex(
                name: "IX_Movimentacoes_VendaId",
                table: "Movimentacoes");

            migrationBuilder.DropColumn(
                name: "DataFinalizacao",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "FormaPagamento",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "Troco",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "ValorRecebido",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "VendaId",
                table: "TransacoesFinanceiras");

            migrationBuilder.DropColumn(
                name: "VendaId",
                table: "Movimentacoes");
        }
    }
}
