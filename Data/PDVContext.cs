using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDV.Models;

namespace PDV.Data
{
    public class PDVContext : DbContext
    {

        public PDVContext(DbContextOptions<PDVContext> options)
           : base(options)
        {
        }

        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<ChaveAcesso> ChaveAcesso { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Fechamento> Fechamento { get; set; }
        public DbSet<Fornecedor> Fornecedor { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<ProdutoPromocao> ProdutoPromocao { get; set; }
        public DbSet<ProdutosVenda> ProdutosVenda { get; set; }
        public DbSet<TipoPagamento> TipoPagamento { get; set; }
        public DbSet<UsuarioEmpresa> UsuarioEmpresa { get; set; }
        public DbSet<Vendas> Vendas { get; set; }

            
    }
}
