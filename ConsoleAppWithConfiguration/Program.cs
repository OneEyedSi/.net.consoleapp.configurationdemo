using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleAppWithConfiguration
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            
            // Invoke Worker
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var workerInstance = provider.GetRequiredService<Worker>();

            // Looks like we could shorten that:
            // var workerInstance = host.Services.GetRequiredService<Worker>();

            workerInstance.DoWork();

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, app) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    Console.WriteLine($"Environment name: {env.EnvironmentName}");
                })
                // Register with Dependency injection container:
                // Using ConfigureServices overload that takes parameter Action<HostBuilderContext, IServiceCollection> 
                //  because services.Configure<>() and GetSection() require access to the HostBuiderContext.Configuration 
                //  property.
                // Without needing to access Configuration we could use the overload of ConfigureServices that takes just
                // Action<IServiceCollection>.
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<Worker>();

                    // This will register an IOptions<ContainerOptions> object, not a simple ContainerOptions object.
                    services.AddOptions<ContainerOptions>().BindConfiguration("Settings:Container");

                    // This will also register an IOptions<OrderOptions> object.
                    // It requires access to the HostBuilderContext.Configuration property.
                    // Note that the nested AddressOptions object will also be populated at the same time.
                    services.Configure<OrderOptions>(context.Configuration.GetSection("Settings:Order"));

                    // This allows us to register an IServerOptions object, without wrapping it in IOptions.
                    // It also requires access to the HostBuilderContext.Configuration property.
                    // If we wanted we could have registered it as a concrete type, rather than as the implementation of an 
                    // interface.
                    ServerOptions serverOptions = context.Configuration.GetSection("Settings:Server").Get<ServerOptions>();
                    services.AddSingleton<IServerOptions>(serverOptions);                    
                });

            return builder;
        }
    }
}
