using System.ComponentModel.DataAnnotations;

namespace PDV.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }

    }
}
