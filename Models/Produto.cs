using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDV.Models
{
    public class Produto
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }
        public int Status { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}
