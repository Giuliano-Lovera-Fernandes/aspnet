using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EstoqueWeb.Models
{
    [Table("ItemPedido")]
    public class ItemPedidoModel
    {       
        /* IdPedido e IdProduto formam uma chave primária composta e formam o relacionamento de muitos para muitos */
        [DatabaseGenerated(DatabaseGeneratedOption.None)]        
        public int IdPedido { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public double  ValorUnitario { get; set; }

        [ForeignKey("IdPedido")]
        public PedidoModel Pedido { get; set; }

        [ForeignKey("IdProduto")]
        public ProdutoModel Produto { get; set; }

        [NotMapped]
        public double ValorItem { get => Quantidade * ValorUnitario; }
    }
}