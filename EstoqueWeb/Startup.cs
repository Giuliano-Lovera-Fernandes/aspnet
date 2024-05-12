using EstoqueWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb
{
    public class Startup
    {
        /*Por injeção um objeto capaz de ler o arquivo de configuração da aplicação e assim ler o arquivo de banco de dados, a partir do arquivo de configuração
        da aplicação */
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
           services.AddControllersWithViews().AddRazorRuntimeCompilation();
           //services.AddDbContext<EstoqueWebContext>(options => options.UseSqlite("Data Source=estoque.db")); 
           services.AddDbContext<EstoqueWebContext>(options => 
            options.UseSqlite(Configuration.GetConnectionString("EstoqueWebContext"))); 
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

        } 
    }
}