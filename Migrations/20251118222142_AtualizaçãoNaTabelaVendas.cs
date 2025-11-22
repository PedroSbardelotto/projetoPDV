using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDV.Migrations
{
    public partial class AtualizaçãoNaTabelaVendas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Cliente_ClienteId",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_TipoPagamento_TipoPagamentoId",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_UsuarioEmpresa_UsuarioEmpresaId",
                table: "Vendas");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioEmpresaId",
                table: "Vendas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TipoPagamentoId",
                table: "Vendas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Vendas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Cliente_ClienteId",
                table: "Vendas",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_TipoPagamento_TipoPagamentoId",
                table: "Vendas",
                column: "TipoPagamentoId",
                principalTable: "TipoPagamento",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_UsuarioEmpresa_UsuarioEmpresaId",
                table: "Vendas",
                column: "UsuarioEmpresaId",
                principalTable: "UsuarioEmpresa",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Cliente_ClienteId",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_TipoPagamento_TipoPagamentoId",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_UsuarioEmpresa_UsuarioEmpresaId",
                table: "Vendas");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioEmpresaId",
                table: "Vendas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TipoPagamentoId",
                table: "Vendas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Vendas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Cliente_ClienteId",
                table: "Vendas",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_TipoPagamento_TipoPagamentoId",
                table: "Vendas",
                column: "TipoPagamentoId",
                principalTable: "TipoPagamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_UsuarioEmpresa_UsuarioEmpresaId",
                table: "Vendas",
                column: "UsuarioEmpresaId",
                principalTable: "UsuarioEmpresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
