using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibatech.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RenomearTabelasParaPascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_estoques_produtos_ProdutoId",
                table: "estoques");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimentacoes_produtos_ProdutoId",
                table: "Movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimentacoes_usuarios_UsuarioId",
                table: "Movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_projetos_requisitos_usuarios_UsuarioId",
                table: "projetos_requisitos");

            migrationBuilder.DropForeignKey(
                name: "FK_transacoes_financeiras_usuarios_UsuarioId",
                table: "transacoes_financeiras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_produtos",
                table: "produtos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_estoques",
                table: "estoques");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transacoes_financeiras",
                table: "transacoes_financeiras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_projetos_requisitos",
                table: "projetos_requisitos");

            migrationBuilder.RenameTable(
                name: "usuarios",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "produtos",
                newName: "Produtos");

            migrationBuilder.RenameTable(
                name: "estoques",
                newName: "Estoques");

            migrationBuilder.RenameTable(
                name: "transacoes_financeiras",
                newName: "TransacoesFinanceiras");

            migrationBuilder.RenameTable(
                name: "projetos_requisitos",
                newName: "ProjetosRequisitos");

            migrationBuilder.RenameIndex(
                name: "IX_usuarios_Email",
                table: "Usuarios",
                newName: "IX_Usuarios_Email");

            migrationBuilder.RenameIndex(
                name: "IX_produtos_CodigoSku",
                table: "Produtos",
                newName: "IX_Produtos_CodigoSku");

            migrationBuilder.RenameIndex(
                name: "IX_estoques_ProdutoId",
                table: "Estoques",
                newName: "IX_Estoques_ProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_transacoes_financeiras_UsuarioId",
                table: "TransacoesFinanceiras",
                newName: "IX_TransacoesFinanceiras_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_projetos_requisitos_UsuarioId",
                table: "ProjetosRequisitos",
                newName: "IX_ProjetosRequisitos_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Estoques",
                table: "Estoques",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransacoesFinanceiras",
                table: "TransacoesFinanceiras",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjetosRequisitos",
                table: "ProjetosRequisitos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Estoques_Produtos_ProdutoId",
                table: "Estoques",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentacoes_Produtos_ProdutoId",
                table: "Movimentacoes",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentacoes_Usuarios_UsuarioId",
                table: "Movimentacoes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjetosRequisitos_Usuarios_UsuarioId",
                table: "ProjetosRequisitos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransacoesFinanceiras_Usuarios_UsuarioId",
                table: "TransacoesFinanceiras",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estoques_Produtos_ProdutoId",
                table: "Estoques");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimentacoes_Produtos_ProdutoId",
                table: "Movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimentacoes_Usuarios_UsuarioId",
                table: "Movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjetosRequisitos_Usuarios_UsuarioId",
                table: "ProjetosRequisitos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransacoesFinanceiras_Usuarios_UsuarioId",
                table: "TransacoesFinanceiras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Estoques",
                table: "Estoques");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransacoesFinanceiras",
                table: "TransacoesFinanceiras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjetosRequisitos",
                table: "ProjetosRequisitos");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "usuarios");

            migrationBuilder.RenameTable(
                name: "Produtos",
                newName: "produtos");

            migrationBuilder.RenameTable(
                name: "Estoques",
                newName: "estoques");

            migrationBuilder.RenameTable(
                name: "TransacoesFinanceiras",
                newName: "transacoes_financeiras");

            migrationBuilder.RenameTable(
                name: "ProjetosRequisitos",
                newName: "projetos_requisitos");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_Email",
                table: "usuarios",
                newName: "IX_usuarios_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_CodigoSku",
                table: "produtos",
                newName: "IX_produtos_CodigoSku");

            migrationBuilder.RenameIndex(
                name: "IX_Estoques_ProdutoId",
                table: "estoques",
                newName: "IX_estoques_ProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_TransacoesFinanceiras_UsuarioId",
                table: "transacoes_financeiras",
                newName: "IX_transacoes_financeiras_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjetosRequisitos_UsuarioId",
                table: "projetos_requisitos",
                newName: "IX_projetos_requisitos_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_produtos",
                table: "produtos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_estoques",
                table: "estoques",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transacoes_financeiras",
                table: "transacoes_financeiras",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_projetos_requisitos",
                table: "projetos_requisitos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_estoques_produtos_ProdutoId",
                table: "estoques",
                column: "ProdutoId",
                principalTable: "produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentacoes_produtos_ProdutoId",
                table: "Movimentacoes",
                column: "ProdutoId",
                principalTable: "produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentacoes_usuarios_UsuarioId",
                table: "Movimentacoes",
                column: "UsuarioId",
                principalTable: "usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_projetos_requisitos_usuarios_UsuarioId",
                table: "projetos_requisitos",
                column: "UsuarioId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transacoes_financeiras_usuarios_UsuarioId",
                table: "transacoes_financeiras",
                column: "UsuarioId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
