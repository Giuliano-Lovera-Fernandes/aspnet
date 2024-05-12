using System.Text;

public class EnderecoHtml : IFormatadorEndereco
{
    private int contadorDeUso = 0;

    private Guid guid = Guid.NewGuid();

    public async Task Formatar(HttpContext context, IEndereco endereco)
    {
        StringBuilder conteudo = new StringBuilder();
        conteudo.Append($"<h3>CEP: {endereco.Cep}</h3>\r\n");
        conteudo.Append($"<p>Logradouro: {endereco.Logradouro}</p>\r\n");
        conteudo.Append($"<p>Complemento: {endereco.Complemento}</p>\r\n");
        conteudo.Append($"<p>Bairro: {endereco.Bairro}</p>\r\n");
        conteudo.Append($"<p>Cidade/UF: {endereco.Localidade}/{endereco.UF}</p>\r\n");
        conteudo.Append($"<p><small>\r\nFormatador usado: {++contadorDeUso} vez(es).\r\n</small></p>");
        conteudo.Append($"<p><small>GUID: {guid}</small></p>");
        //context.Response.ContentType = "text/html; CharSet=utf-8";
        await context.Response.WriteAsync(conteudo.ToString());
    }
}