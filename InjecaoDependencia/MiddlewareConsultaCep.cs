using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Web;
using Newtonsoft.Json;

public class MiddlewareConsultaCep
{
    //Propriedade que guarda a referência para o próximo middleware
    private readonly RequestDelegate Next;
    //private IFormatadorEndereco formatador;

    public MiddlewareConsultaCep()
    {
       
    }

    //Para utilizar o Scoped, vamos remover a dependência e colocar em Invoke e remover o atributo formatador

    public MiddlewareConsultaCep(RequestDelegate next /*, IFormatadorEndereco formatadorEndereco */)
    {
        Next = next;
        /* formatador = formatadorEndereco; */
    }

    //Método Invoke que vai procesar a requisição
    public async Task Invoke(HttpContext context, IFormatadorEndereco formatador1, IFormatadorEndereco formatador2 /* Adição da dependência, com nome alterado, corretamente */)
    {
        //Checa se a Url é compatível com o middleware
        if(context.Request.Path.StartsWithSegments("/mw/classe"))
        {
            //captura o valor do cep da Url
            string[] segmentos = context.Request.Path.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries);
            string cep = segmentos.Length > 2 ? segmentos[2] : "29216010";

            var objetoCep = await ConsultaCep(cep);
            /* var textoCep = await ConsultaCep(cep); */
            /* context.Response.ContentType = "text/html; CharSet=utf-8";
            StringBuilder html = new StringBuilder();
            html.Append($"<h3>Dados de CEP {objetoCep.Cep}</h3>");
            html.Append($"<p>Logradouro: {objetoCep.Logradouro}</p>");
            html.Append($"<p>Bairro: {objetoCep.Bairro}</p>");
            html.Append($"<p>Cidade/UF: {objetoCep.Localidade}/{objetoCep.UF}</p>");
            string localidade = HttpUtility.UrlEncode($"{objetoCep.Localidade}-{objetoCep.UF}");            
            await context.Response.WriteAsync(html.ToString()); */

            //A fim de centralizar a formatação de endereço á um único objeto que é compartilhado por toda a aplicação.
            //await EnderecoTextual.Singleton.Formatar(context, objetoCep);
           await formatador1.Formatar(context, objetoCep); 
           await formatador2.Formatar(context, objetoCep);                    
        }
        if(Next != null)
        {
            await Next(context);
        } 
    }

    private async Task<JsonCep> ConsultaCep(string cep)
    {
        var url = $"https://viacep.com.br/ws/{cep}/json";
        var cliente = new HttpClient();
        cliente.DefaultRequestHeaders.Add("User-Agent", "Middleware Consulta CEP");
        var response = await cliente.GetAsync(url);

        var dadosCep = await response.Content.ReadAsStringAsync();
        dadosCep = dadosCep.Replace("?(","").Replace(");","").Trim();
        return JsonConvert.DeserializeObject<JsonCep>(dadosCep);
    }
}
/* O middleware de ConsultaCep tem uma dependencia forte em relacao a consulta populacao */