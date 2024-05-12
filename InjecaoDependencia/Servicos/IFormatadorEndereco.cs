public interface IEndereco
{
    //Representa o endereço e pode ser implementada por qualquer classe que deva armazenar dados de endereço
    string Cep { get; set; }
    string Logradouro { get; set; }
    string Complemento { get; set; }
    string Bairro { get; set; }
    string Localidade { get; set; } 
    string UF { get; set; }
}

public interface IFormatadorEndereco
{
    Task Formatar(HttpContext context, IEndereco endereco);
}