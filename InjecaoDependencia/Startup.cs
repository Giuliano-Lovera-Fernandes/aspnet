public class Startup
{
    private IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        // Configuração dos serviços, como injeção de dependência, que serão disponibilizados na aplicação.
        /* Código indica que qualquer local da aplicação, que solicitar uma implementação da interface
        IFormatadorEndereco via injeção de dependência, vai receber uma instância da classe concreta
        EnderecoHtml, quem é quem efetivamente executa o serviço, o método AddSingleton vai adicionar esse
        objeto usando o padrão singleton, essa instância será criada apenas uma vez e será compartilhada
        por toda a aplicação.*/ 

        /* O ciclo de vida pode variar, podendo ser utilizado mais dois métodos, que podem ser usados para adicionar
        serviço numa aplicação:
        
        - AddTransient: cria um objeto para cada dependência resolvida, a cada resolução do serviço
        solicitado, um novo objeto é criado.
        
        - AddScoped: Cria um objeto para cada escopo, que pode variar de acordo com o tipo de aplicação
        numa aplicação web, equivale a duração de uma requisição */


        //Cria um único objeto que resolve todas as dependências de um determinado tipo na aplicação.     
        //services.AddSingleton<IFormatadorEndereco, EnderecoHtml>();
        //services.AddScoped<IFormatadorEndereco, EnderecoHtml>();

        /* O erro se refere porque estamos tentando obter um serviço de escopo, a partir de um local
        que está fora do escopo, ou seja, estamos solicitando o serviço IFormatadorEndereco, nos construtores
        das classes EndpointConsultaCep e MiddlewareConsultaCep, porém esses construtores são chamados
        fora do escopo, porque essas classes são criadas no momento de configuração do pipeline e não
        durante o processamento de uma requisição, que é onde o escopo é válido. Para as classes Middleware isso
        isso pode ser facilmente resolvido, retirando as referências do construtor e as colocando no método
        invoke, que processa a requisição.  */

        //services.AddScoped<IFormatadorEndereco, EnderecoHtml>();
        /* services.AddScoped<IFormatadorEndereco>(ServiceProvider =>
        {
            string? nomeTipo = configuration["servicos:IFormatadorEndereco"];
            return (IFormatadorEndereco)ActivatorUtilities.CreateInstance(ServiceProvider,
                nomeTipo == null ? typeof(EnderecoTextual): Type.GetType(nomeTipo, true));
        }); */
        services.AddSingleton(typeof(ICollection<>), typeof(List<>));
    }

    /*Antes do método Configure ser chamado, seus parâmetros são inspecionados pelo módulo de injeção
    de dependência, sua dependência é detectada e seus serviços disponíveis no container são analisados para
    ver se algum deles resolve a dependência, o mecanismo de injeção vai resolver a dependência, com a instância
    do objeto enderecoHtml, o objeto é retornado para resolver a dependência. Pelo fato desse objeto ter vindo de 
    fora do método Configure e da classe Startup, dizemos que a dependência foi injetada no método
    
    Definir e consumir um método via injeção de dependência.
    */
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env/* , IFormatadorEndereco formatador */)
    {
        app.UseDeveloperExceptionPage();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/string", async context =>
            {
                ICollection<string>? lista = context.RequestServices.
                    GetService<ICollection<string>>();
                lista.Add($"Requisição: {DateTime.Now.ToLongTimeString()}");
                context.Response.ContentType = "text/plain; charset=utf-8;";
                foreach(string str in lista)
                {
                    await context.Response.WriteAsync($"String: {str}\n");
                }   
            });

            endpoints.MapGet("/int", async context =>
            {
                ICollection<int>? lista = context.RequestServices.
                    GetService<ICollection<int>>();
                lista.Add(lista.Count + 1);                
                context.Response.ContentType = "text/plain; charset=utf-8;";
                foreach(int val in lista)
                {
                    await context.Response.WriteAsync($"int: {val}\n");
                }   
            });
        });

        //app.UseMiddleware<MiddlewareConsultaCep>();- último

        //Pode ser entendido como um objeto formatador de serviço.
        //IFormatadorEndereco formatador = new EnderecoTextual();

        /* app.Use(async (context, next) =>
        {
            if(context.Request.Path == "/mw/lambda")
            {
                IFormatadorEndereco formatador =
                    context.RequestServices.GetRequiredService<IFormatadorEndereco>(); */
                /* await EnderecoTextual.Singleton.Formatar(context,
                    await EndPointConsultaCep.ConsultaCep("29216010")); */
                /* context.Response.ContentType = "text/html; charset=utf-8;";    
                await formatador.Formatar(context,
                    await EndPointConsultaCep.ConsultaCep("29216010"));
                await formatador.Formatar(context,
                    await EndPointConsultaCep.ConsultaCep("01001000"));    
            }
            else
            {
                await next();
            }
        }); */

        //app.UseEndpoints(endpoints =>
        //{
            //endpoints.MapEndpoint<EndPointConsultaCep>("/ep/classe/{cep:regex(^\\d{{8}}$)?}");
            //endpoints.MapEndpoint<EndPointConsultaCep>("/ep/classe/{cep:regex(^\\d{{8}}$)?}");
            //endpoints.MapGet("/ep/lambda/{cep:regex(^\\d{{8}}$)?}", async context =>
            //{
                //IFormatadorEndereco formatador =
                    //context.RequestServices.GetRequiredService<IFormatadorEndereco>();
                //context.Response.ContentType = "text/html; charset=utf-8;";    
                //string cep = context.Request.RouteValues["cep"] as string ?? "29216010";
                /* await EnderecoTextual.Singleton.Formatar(context, 
                await EndPointConsultaCep.ConsultaCep(cep)); */
                //await formatador.Formatar(context, 
                //await EndPointConsultaCep.ConsultaCep(cep));
            //});

        //});

        app.Run(async context =>
        {
            await context.Response.WriteAsync("Middleware terminal");
        });
    }
}
