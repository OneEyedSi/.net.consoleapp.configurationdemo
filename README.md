.NET 6 Console App with Configuration
=====================================
Simon Elms, 24 Mar 2022

Sample .NET 6 console application reading configuration from an appsettings.json file.  

As well as reading the appsettings.json values directly the project demonstrates the Options pattern - binding an options object to the configuration to give strongly-typed configuration.  The configuration is bound to the options object once then subsequently settings values can be read from the options object.  This also simplifies reading settings in different classes: once the options object has been populated it can be injected into any class via dependency injection.

References
----------
1. "How To Use Appsettings Json Config File With .NET Console Applications", https://thecodeblogger.com/2021/05/04/how-to-use-appsettings-json-config-file-with-net-console-applications/ - this gives sample code which was used as the basis for this application.  However, it only included enumerating the key-values pairs in the configuration; the other methods of reading the configuration were added to that basic demo code.

2. "Configuration in .NET", https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration

3. "Options pattern in ASP.NET Core", https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0 (works with console app as well, not just ASP.NET Core)

4. The answer to Stackoverflow question "Add appsettings configuration DI to HostBuilder console app", https://stackoverflow.com/a/59275419/216440, shows that services.Configure<OptionsClass>(...) registers an IOptions<OptionsClass> object with the DI container, not an OptionsClass object.

5. https://github.com/dotnet/runtime/issues/46296 - code shows calling Host.CreateDefaultBuilder(args).ConfigureServices() with two arguments, not one.  The first argument, of type HostBuilderContext, provides access to a Configuration object, needed to register a bound options object with the DI container, without having it wrapped in IOptions.

6. "It’s all in the Host Class – Part 2: Configuration", https://csharp.christiannagel.com/2020/06/23/configurationwithhost/ - demonstrates binding objects to configuration sections in nested fashion, where one section contains another.  Both are bound at the same time to appropriate options objects.

Notes
-----
1. Set appsettings.json file "Copy to Output Directory" property to "Copy Always".

2. Add NuGet packages:
	* Microsoft.Extensions.Hosting (this provides support for configuration as well as dependency injection)

3. According to "Configuration in .NET", Host.CreateDefaultBuilder(String[]) provides default configuration in the following order:

	a. ChainedConfigurationProvider : Adds an existing IConfiguration as a source.
	b. appsettings.json using the JSON configuration provider.
	c. appsettings.Environment.json using the JSON configuration provider. For example, appsettings.Production.json and appsettings.Development.json.
	d. App secrets when the app runs in the Development environment.
	e. Environment variables using the Environment Variables configuration provider.
	f. Command-line arguments using the Command-line configuration provider.

4. Configuration providers that are added later override previous key settings. For example, if SomeKey is set in both appsettings.json and the environment, the environment value is used. Using the default configuration providers, the Command-line configuration provider overrides all other providers.

5. To bind an options object to a configuration section then register it with the Dependency Injection container, without wrapping it in IOptions, we have to use the ConfigureServices overload that takes a context parameter:  ConfigureServices((context, services) => ...) rather than the overload that takes only a service parameter: ConfigureServices(services => ...)  This is because we need access to context.Configuration, to instantiate the options object and bind it to the configuration section.

See Also
--------
[README_Secrets.md](/README_Secrets.md): Instructions on how to remove secrets from the appsettings.json file and store them safely elsewhere, out of source control.