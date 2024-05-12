using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Xml.Serialization;

namespace EstoqueWeb.Models
{
    [Table("Cliente")]
    /* [Table("Cliente", Schema = "Dbo")] ressaltar que o schema (agrupamento de tabelas, Views) pode ser mapeado*/
    public class ClienteModel : UsuarioModel
    {
        //tipo do banco usado para mapear essa coluna, pois o padrão seria varchar ou no caso do SqLite text
        [Required, Column(TypeName = "char(14)")]
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }


        //Por ser uma propriedade calculada, não deve mapeada. 
        [NotMapped]
        public int Idade
        { 
            get => (int)Math.Floor((DateTime.Now - DataNascimento).TotalDays / 365.2425);
            /* {
                int dias = DateTime.Now.Subtract(DataNascimento).Days;
                int anos = (int)Math.Floor(dias / 365.2425); 
                return anos;                       
            }  */
        }

        //public EnderecoModel Endereco { get; set; }

        public ICollection<EnderecoModel>? Enderecos { get; set; }
        public ICollection<PedidoModel>? Pedidos { get; set; }
    }
}