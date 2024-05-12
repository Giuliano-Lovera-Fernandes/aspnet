using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstoqueWeb.Models
{
    /* O mecanismo automático do EF sempre vai pluralizar a entidade, caso não seja especificado */
    [Table("Usuario")]
    public class UsuarioModel
    {
        [Key]        
        public int IdUsuario { get; set; }

        [Required, MaxLength(128)]
        public string Nome { get; set; }

        [Required, MaxLength(128)]
        public string  Email { get; set; }

        [Required, MaxLength(128)]
        public string Senha { get; set; }

        /* O sistema mesmo irá preencher, nesse caso, poderá ser somente leitura */
        [ReadOnly(true)]
        public DateTime? DataCadastro { get; }
    }
}