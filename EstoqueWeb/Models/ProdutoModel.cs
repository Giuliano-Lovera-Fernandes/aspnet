using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstoqueWeb.Models
{
    /* Como a classe ProdutoModel foi acrescentada, o EstoqueWebContext também precisa ser alterado */
    public class ProdutoModel
    {
        [Key]        
        public int IdProduto { get; set; }

        [Required, MaxLength(128)]
        public string Nome { get; set; }

        public int Estoque { get; set; }
        public double Preco { get; set; }

        /* Este atributo pode ser colocado na propriedade concreta (neste caso, passada a propriedade de
        navegação como parâmetro), mas também em Categoria. */
        /* Num relacionamento de um para muitos essa é a convenção que está do lado do muitos */
        /* [ForeignKey("Categoria")] */
        public int IdCategoria { get; set; }

        /* [ForeignKey("Pedido")]
        public int? IdPedido { get; set; } */

        [ForeignKey("IdCategoria")]
        public CategoriaModel Categoria { get; set; }
        //public ItemPedidoModel? ItemPedido { get; set; }
    }
}