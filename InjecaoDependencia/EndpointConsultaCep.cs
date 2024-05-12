using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using Newtonsoft.Json;

public class EndPointConsultaCep
{
    //private IFormatadorEndereco formatador;

    //Scoped - também remover a dependência do construtor
    /* public EndPointConsultaCep(/* IFormatadorEndereco formatadorEndereco )
    {
        //formatador = formatadorEndereco;
    } */

    public async Task EndPoint(HttpContext context, IFormatadorEndereco formatador /* Adição no método EndPoint */)
    {
        string cep = HttpUtility.UrlDecode(context.Request.RouteValues["cep"] as string ?? "29216010");
        
        var objetoCep = await ConsultaCep(cep);            
        if (objetoCep == null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        else
        {
            /* context.Response.ContentType = "text/html; CharSet=utf-8";
            StringBuilder html = new StringBuilder();
            html.Append($"<h3>Dados de CEP {objetoCep.Cep}</h3>");
            html.Append($"<p>Logradouro: {objetoCep.Logradouro}</p>");
            html.Append($"<p>Bairro: {objetoCep.Bairro}</p>");
            html.Append($"<p>Cidade/UF: {objetoCep.Localidade}/{objetoCep.UF}</p>");
            string localidade = HttpUtility.UrlEncode($"{objetoCep.Localidade}-{objetoCep.UF}");            
            await context.Response.WriteAsync(html.ToString());        */
            //await EnderecoTextual.Singleton.Formatar(context, objetoCep);
            //await TypeBroker.FormatadorEndereco.Formatar(context, objetoCep);
           /*  IFormatadorEndereco formatador = context.RequestServices
                .GetRequiredService<IFormatadorEndereco>(); */
            await formatador.Formatar(context, objetoCep);    
        }                      
    }

    public static async Task<JsonCep> ConsultaCep(string cep)
    {
        var url = $"https://viacep.com.br/ws/{cep}/json";
        var cliente = new HttpClient();
        cliente.DefaultRequestHeaders.Add("User-Agent", "Middleware Consulta CEP");
        var response = await cliente.GetAsync(url);

        var dadosCep = await response.Content.ReadAsStringAsync();
        dadosCep = dadosCep.Replace("?(","").Replace(");","").Trim();        
        return dadosCep.Contains("\"erro\":") ? null : JsonConvert.DeserializeObject<JsonCep>(dadosCep);
    }
}