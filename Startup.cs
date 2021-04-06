using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDoCodigo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Permite que você adicione serviçoes a aplicação (Ex: Banco de Dados, Log etc)
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Configutando sessão para navegação (Session e Cache são os dois serviços necessários)    
            services.AddDistributedMemoryCache();
            services.AddSession();

            string connectionString = Configuration.GetConnectionString("Default");
            //<NomeDaClasseDeContexto>
            services.AddDbContext<ApplicationContext>(options => 
                options.UseSqlServer(connectionString)
            );

            // Registro injeção de dependência (Criamos uma interface para a classe de queremos injetar.)
            // Adiciona uma instância somente quando os métodos que necessitarem dela estiverem ativos. (Instância temporária)
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<IProdutoRepository, ProdutoRepository>();
            services.AddTransient<IPedidoRepository, PedidoRepository>();
            services.AddTransient<ICadastroRepository, CadastroRepository>();
            services.AddTransient<IItemPedidoRepository, ItemPedidoRepository>();
        }

        // Este método é executado sempre que a aplicação sobre (Por exemplo quando você aperta F5 ou Sobe para o servidor)
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. Consome os serviços adicionados no ConfigureServices.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Define uso de arquivos estaticos
            app.UseStaticFiles();

            // Definimos uso de sessao
            app.UseSession();

            // Define uso do padrão MVC
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Pedido}/{action=Carrossel}/{codigo?}");
            });

            // Configura chamada de método para garantir de BD foi criado, caso não tenha o .Migrate() cria.
            serviceProvider.GetService<IDataService>().inicializaDB();

        }
    }
}
