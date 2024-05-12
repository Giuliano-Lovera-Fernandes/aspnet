using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Models
{
    [Owned, Table("Endereco")]//significa pertencimento, num objeto do tipo ClienteModel terá um objeto do tipo EnderecoModel
    public class EnderecoModel
    {
        [Required]
        public int IdEndereco { get; set; }

        [Required]
        public string Logradouro { get; set; }

        [Required]
        public string Numero { get; set; }

        
        public string Complemento { get; set; }

        [Required]
        public string  Bairro { get; set; }

        [Required]
        public string Cidade { get; set; }

        [Required, Column(TypeName = "char(2)")]
        public string Estado { get; set; }

       [Required, Column(TypeName = "char(9)")]
        public string Cep { get; set; }

        public string Referencia { get; set; }

        public bool Selecionado { get; set; }

        /* Não será gravada no banco de dados */
        [NotMapped]
        public string EnderecoCompleto
        {
            get
            {
                return $"{Logradouro}, {Numero} {Complemento}, {Bairro}, {Cidade}, {Estado}, CEP {Cep}";

            }
        }

    }
}