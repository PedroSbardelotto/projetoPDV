using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDV.Migrations
{
    public partial class AdicionandoItemProduto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProdutoId",
                table: "ProdutoPromocao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoPromocao_ProdutoId",
                table: "ProdutoPromocao",
                column: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoPromocao_Produto_ProdutoId",
                table: "ProdutoPromocao",
                column: "ProdutoId",
                principalTable: "Produto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoPromocao_Produto_ProdutoId",
                table: "ProdutoPromocao");

            migrationBuilder.DropIndex(
                name: "IX_ProdutoPromocao_ProdutoId",
                table: "ProdutoPromocao");

            migrationBuilder.DropColumn(
                name: "ProdutoId",
                table: "ProdutoPromocao");
        }
    }
}
