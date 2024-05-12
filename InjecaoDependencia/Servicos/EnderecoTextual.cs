using System.Text;

public class EnderecoTextual : IFormatadorEndereco
{
    private int contadorDeUso = 0;
    private static EnderecoTextual instanciaCompartilhada;

    public async Task Formatar(HttpContext context, IEndereco endereco)
    {
       StringBuilder conteudo = new StringBuilder();
        conteudo.Append($"CEP: {endereco.Cep}\r\n");
        conteudo.Append($"Logradouro: {endereco.Logradouro}\r\n");
        conteudo.Append($"Complemento: {endereco.Complemento}\r\n");
        conteudo.Append($"Bairro: {endereco.Bairro}\r\n");
        conteudo.Append($"Cidade/UF: {endereco.Localidade}/{endereco.UF}\r\n");
        conteudo.Append($"\r\nFormatador usado: {++contadorDeUso} vez(es).\r\n");
        //context.Response.ContentType = "text/html; CharSet=utf-8";
        await context.Response.WriteAsync(conteudo.ToString());
    }

    public static EnderecoTextual Singleton
    {
        get
        {
            if (instanciaCompartilhada == null)
            {
                instanciaCompartilhada = new EnderecoTextual();
            }
            return instanciaCompartilhada;
        }
    }
}