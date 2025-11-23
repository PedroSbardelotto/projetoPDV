using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDV.Models
{
    public class Vendas
    {
        public int Id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Valor")]
        public decimal ValorTotal { get; set; }
        public short Status { get; set; }
        [Display(Name = "Data")]
        public DateTime DataEntrada { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public int? TipoPagamentoId { get; set; }
        [Display(Name = "Forma de Pagamento")]
        public TipoPagamento TipoPagamento { get; set; }
        public int? ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int? UsuarioEmpresaId { get; set; }
        public UsuarioEmpresa UsuarioEmpresa { get; set; }
        public int FechamentoId { get; set; }
        public Fechamento Fechamento { get; set; }

        public List<ProdutosVenda> ProdutosVenda { get; set; }
    }
}
