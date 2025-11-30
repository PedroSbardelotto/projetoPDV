using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDV.Migrations
{
    public partial class AjusteMapeamentoProdutoFornecedor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodigoProduto",
                table: "ProdutoFornecedor",
                newName: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoFornecedor_CodigoFornecedor",
                table: "ProdutoFornecedor",
                column: "CodigoFornecedor");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoFornecedor_ProdutoId",
                table: "ProdutoFornecedor",
                column: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoFornecedor_Fornecedor_CodigoFornecedor",
                table: "ProdutoFornecedor",
                column: "CodigoFornecedor",
                principalTable: "Fornecedor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoFornecedor_Produto_ProdutoId",
                table: "ProdutoFornecedor",
                column: "ProdutoId",
                principalTable: "Produto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoFornecedor_Fornecedor_CodigoFornecedor",
                table: "ProdutoFornecedor");

            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoFornecedor_Produto_ProdutoId",
                table: "ProdutoFornecedor");

            migrationBuilder.DropIndex(
                name: "IX_ProdutoFornecedor_CodigoFornecedor",
                table: "ProdutoFornecedor");

            migrationBuilder.DropIndex(
                name: "IX_ProdutoFornecedor_ProdutoId",
                table: "ProdutoFornecedor");

            migrationBuilder.RenameColumn(
                name: "ProdutoId",
                table: "ProdutoFornecedor",
                newName: "CodigoProduto");
        }
    }
}
