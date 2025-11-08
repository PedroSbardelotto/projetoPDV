using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDV.Migrations
{
    public partial class FixClienteRemoveInscricao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CNPJ",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "InscricaoEstadual",
                table: "Cliente");

            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "Cliente",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPF",
                table: "Cliente");

            migrationBuilder.AddColumn<string>(
                name: "CNPJ",
                table: "Cliente",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InscricaoEstadual",
                table: "Cliente",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
