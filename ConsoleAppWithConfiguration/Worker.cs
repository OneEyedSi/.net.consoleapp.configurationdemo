using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppWithConfiguration
{
    internal class Worker
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<ContainerOptions> _optionsRegisteredByAddOptions;
        private readonly IOptions<OrderOptions> _optionsRegisteredByConfigure;
        private readonly IServerOptions _optionsRegisteredManually;

        public Worker(IConfiguration configuration, IOptions<ContainerOptions> optionsRegisteredByAddOptions,
            IOptions<OrderOptions> optionsRegisteredByConfigure,
            IServerOptions optionsRegisteredManually)
        {
            this._configuration = configuration;
            this._optionsRegisteredByAddOptions = optionsRegisteredByAddOptions;
            this._optionsRegisteredByConfigure = optionsRegisteredByConfigure;
            this._optionsRegisteredManually = optionsRegisteredManually;    
        }

        public void DoWork()
        {
            var keyValuePairs = _configuration.AsEnumerable().ToList();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("==============================================");
            Console.WriteLine("Enumerate configurations...");
            Console.WriteLine("==============================================");
            // Lists the settings in the appsettings.json file as:
            /*
                SettingsList -
                SettingsList:2 - third
                SettingsList:1 - second
                SettingsList:0 - first
                Settings -
                Settings:Server -
                Settings:Server:OS - Windows Server 2019
                Settings:Server:Name - ABC123
                Settings:Order -
                Settings:Order:Number - 524
                Settings:Order:Customer - Joe Bloggs
                Settings:Order:Address -
                Settings:Order:Address:Street - 10 High Street
                Settings:Order:Address:City - Christchurch
                Settings:KeyTwo - True
                Settings:KeyThree - text value
                Settings:KeyOne - 1
                Settings:Container -
                Settings:Container:Name - Second container
                Settings:Container:Index - 2
             */
            foreach (var pair in keyValuePairs)
            {
                Console.WriteLine($"{pair.Key} - {pair.Value}");
            }
            Console.WriteLine("==============================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("==============================================");
            Console.WriteLine("Configuration 'Settings' Section values...");
            Console.WriteLine("==============================================");
            // Individual settings in the appsettings.json file 'Settings' Section read as:
            /*
                KeyOne (int) value: 1
                KeyTwo (bool) value: True
                KeyThree (string) value: 'text value'
             */
            IConfigurationSection settingsSection = _configuration.GetRequiredSection("Settings");
            int intSetting = settingsSection.GetValue<int>("KeyOne");
            Console.WriteLine($"KeyOne (int) value: {intSetting}");
            bool boolSetting = settingsSection.GetValue<bool>("KeyTwo");
            Console.WriteLine($"KeyTwo (bool) value: {boolSetting}");
            string stringSetting = settingsSection.GetValue<string>("KeyThree");
            Console.WriteLine($"KeyThree (string) value: '{stringSetting}'");

            Console.WriteLine("==============================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==============================================");
            Console.WriteLine("Reading nested values...");
            Console.WriteLine("==============================================");
            // Individual nested values in the appsettings.json file read from the root as:
            /*
                Reading key 'Settings:Container:Name' via configuration.GetValue<string>(): 'Second container'
                Reading key 'Settings:Container:Index' via configuration.GetValue<int>(): '2'
             */
            string key = "Settings:Container:Name";
            string nestedStringSetting = _configuration.GetValue<string>(key);
            Console.WriteLine($"Reading key '{key}' via configuration.GetValue<string>(): '{nestedStringSetting}'");

            key = "Settings:Container:Index";
            int nestedIntSetting = _configuration.GetValue<int>(key);
            Console.WriteLine($"Reading key '{key}' via configuration.GetValue<int>(): '{nestedIntSetting}'");

            Console.WriteLine("==============================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("======================================================");
            Console.WriteLine("Binding section to options object (Options pattern)...");
            Console.WriteLine("======================================================");
            // Binding object to configuration section 'Settings:Container':
            /*
                ContainerOptions object properties via Section.Bind(): Name: 'Second container', Index: 2.
                ContainerOptions object properties via Section.Get<T>(): Name: 'Second container', Index: 2.
             */
            ContainerOptions options = new();
            _configuration.GetSection("Settings:Container").Bind(options);
            Console.WriteLine($"ContainerOptions object properties via Section.Bind(): Name: '{options.Name}', Index: {options.Index}.");

            ContainerOptions options2 = _configuration.GetSection("Settings:Container").Get<ContainerOptions>();
            Console.WriteLine($"ContainerOptions object properties via Section.Get<T>(): Name: '{options2.Name}', Index: {options2.Index}.");

            Console.WriteLine("======================================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("======================================================");
            Console.WriteLine("Options registered with DI container...");
            Console.WriteLine("======================================================");
            // Binding object to configuration section then adding resultant IOptions to DI container:
            /*
                ContainerOptions object properties registered with DI via AddOptions: Name: 'Second container', Index: 2.
                OrderOptions object properties registered with DI via Configure: Customer: 'Joe Bloggs', Number: 524.
                    Address.Street: 10 High Street, Address.City: Christchurch.
                ServerOptions object properties registered with DI manually: Name: 'ABC123', OS: 'Windows Server 2019'.
             */
            ContainerOptions options3 = _optionsRegisteredByAddOptions.Value;
            Console.WriteLine($"ContainerOptions object properties registered with DI via AddOptions: Name: '{options3.Name}', Index: {options3.Index}.");

            // OrderOptions demonstrates that sections can be bound recursively: OrderOptions.Address, or type AddressOptions, 
            //  is set along with the simple properties of OrderOptions.
            OrderOptions options4 = _optionsRegisteredByConfigure.Value;
            AddressOptions address = options4.Address; 
            Console.WriteLine($"OrderOptions object properties registered with DI via Configure: Customer: '{options4.Customer}', Number: {options4.Number}.");
            Console.WriteLine($"    Address.Street: {address.Street}, Address.City: {address.City}.");

            // Registered with DI container manually so it's not wrapped in IOptions.
            Console.WriteLine($"ServerOptions object properties registered with DI manually: Name: '{_optionsRegisteredManually.Name}', OS: '{_optionsRegisteredManually.OS}'.");
            Console.WriteLine($"ServerOptions properties with no value in appsettings: HasInitializer: {_optionsRegisteredManually.HasInitializer}, NoInitializer: {_optionsRegisteredManually.NoInitializer}");

            Console.WriteLine("======================================================");
            Console.ResetColor();
        }
    }
}
