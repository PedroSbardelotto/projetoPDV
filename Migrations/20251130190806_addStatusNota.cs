using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDV.Migrations
{
    public partial class addStatusNota : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "NotaFiscal",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "NotaFiscal");
        }
    }
}
