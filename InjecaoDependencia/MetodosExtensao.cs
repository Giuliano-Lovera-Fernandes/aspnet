using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class MetodosExtensao
    {
        /* public static void MapConsultaCep(this IEndpointRouteBuilder endpoints, string path)
        {
            IFormatadorEndereco formatador = endpoints.ServiceProvider.GetService<IFormatadorEndereco>();
            endpoints.MapGet(path, context => EndPointConsultaCep.EndPoint(context, formatador));
        } */
        public static void MapEndpoint<T>(this IEndpointRouteBuilder app, string caminho, string nomeMetodo = "EndPoint")
        {
            //Objeto que contém metadados.
            MethodInfo? mi = typeof(T).GetMethod(nomeMetodo);
            if (mi == null || mi.ReturnType != typeof(Task))
            {
                throw new Exception("Método não é compatível");                
            }
            //A classe ActivatorUtilities só consegue resolver dependências, durante a construção do pipeline
            //
            T instanciaEndPoint = ActivatorUtilities.CreateInstance<T>(app.ServiceProvider);
            /* app.MapGet(caminho, (RequestDelegate)mi
                .CreateDelegate(typeof(RequestDelegate), instanciaEndPoint)); */
             ParameterInfo[] parametros = mi.GetParameters();

            /* app.MapGet(caminho, context => 
               (Task)mi.Invoke(instanciaEndPoint, parametros.Select(
                p => p.ParameterType == typeof(HttpContext) ? context:
                context.RequestServices.GetServices(p.ParameterType)).ToArray()));  */


            app.MapGet(caminho, context => 
            {
                var argumentos = parametros.Select((p, i) =>
                {
                    if (p.ParameterType == typeof(HttpContext))
                    {
                        return context;
                    }
                    else
                    {
                        var service = context.RequestServices.GetService(p.ParameterType);

                        if (service == null)
                        {
                            // Lidar com a situação em que o serviço não está registrado.
                            // Isso pode incluir lançar uma exceção, usar um valor padrão ou tomar alguma ação apropriada.
                        }

                        return service;
                    }
                }).ToArray();

                return (Task)mi.Invoke(instanciaEndPoint, argumentos);
            });                   
       
        }
    }
}
