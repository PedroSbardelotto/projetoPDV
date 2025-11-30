using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDV.Migrations
{
    public partial class addFilaFaturamento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilaFaturamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoNFe = table.Column<int>(type: "int", nullable: false),
                    CodigoFornecedor = table.Column<int>(type: "int", nullable: false),
                    CodigoProduto = table.Column<int>(type: "int", nullable: false),
                    Custo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilaFaturamento", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilaFaturamento");
        }
    }
}
