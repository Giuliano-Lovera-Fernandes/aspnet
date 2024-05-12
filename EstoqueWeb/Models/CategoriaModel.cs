using System.ComponentModel.DataAnnotations;

namespace EstoqueWeb.Models
{
    public class CategoriaModel
    {
        [Key]
        public int IdCategoria { get; set; }

        //Tamanho máximo em caracteres,funcionam como instruções na hora de converter essa classe (entidade) para uma
        //tabela do banco de dados.
        [Required, MaxLength(128)]
        public string? Nome { get; set; }
        public ICollection<ProdutoModel> Produtos { get; set; }
    }    
}