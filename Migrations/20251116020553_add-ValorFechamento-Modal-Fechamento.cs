using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDV.Migrations
{
    public partial class addValorFechamentoModalFechamento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "Fechamento",
                newName: "ValorFechamento");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorAbertura",
                table: "Fechamento",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorAbertura",
                table: "Fechamento");

            migrationBuilder.RenameColumn(
                name: "ValorFechamento",
                table: "Fechamento",
                newName: "Valor");
        }
    }
}
