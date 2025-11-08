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
        [Display(Name = "Quantidade Mínima")]
        public int QuantidadeMinima { get; set; }
        [Display(Name = "Preço")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }
        public Boolean Status { get; set; }
        [Display(Name = "Criado em:")]
        public DateTime DataEntrada { get; set; }
        [Display(Name = "Atualizado em:")]
        public DateTime DataAtualizacao { get; set; }
        [Display(Name ="Categoria")]
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
