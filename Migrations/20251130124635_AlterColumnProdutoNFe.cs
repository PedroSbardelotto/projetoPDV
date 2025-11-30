using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDV.Migrations
{
    public partial class AlterColumnProdutoNFe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CodigoProdNFe",
                table: "ProdutosNFe",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CodigoProdNFe",
                table: "ProdutosNFe",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
