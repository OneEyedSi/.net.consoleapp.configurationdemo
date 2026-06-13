using ConsoleAppWithConfiguration.ConfigurationOptions;
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

                    // Binding an array of configuration objects to an options object.  Bind to the parent object,
                    // not the list object.  In this case the parent is the root of the configuration file, so we don't need
                    // to use GetSection() to acccess it.
                    // The options object must have a property named "ObjectList" to match the name of the array in the
                    // configuration file.
                    services.Configure<ObjectListOptions>(context.Configuration);

                    // Binding a nested array of configuration objects to a list.
                    services.Configure<List<ObjectListItem>>(context.Configuration.GetSection("Settings:NestedObjectList"));

                    // Binding a nested array of configuration objects to an options object.  Bind to the parent object,
                    // not the list object.  The options object must have a property named "NestedObjectList" to match the
                    // name of the array in the configuration file.
                    services.Configure<NestedObjectListOptions>(context.Configuration.GetSection("Settings"));
                });

            return builder;
        }
    }
}
