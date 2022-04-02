Handling Secrets in the Configuration of a Console App
======================================================
Simon Elms, 2 Apr 2022

Parent README: [README.md](/README.md)

References 
----------
* "Safe storage of app secrets in development in ASP.NET Core", https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets - good overview, although applies to ASP.NET Core, not a console app

* "How to add User Secrets in a .NET Core console app", https://makolyte.com/how-to-add-user-secrets-in-a-dotnetcore-console-app/ - the main steps needed, although they don't build a host using Host.CreateDefaultBuilder but use a ConfigurationBuilder instead.  As a result they have to manually call AddUserSecrets<T>() while building the configuration.

* Answers to Stack Overflow question "How to set hosting environment name for .NET Core console app using Generic Host (HostBuilder)", https://stackoverflow.com/questions/58105146/how-to-set-hosting-environment-name-for-net-core-console-app-using-generic-host - how to set the EnvironmentName to "Development", which is required to allow the project to read the secrets.json file.

Overview
--------
We don't want to save secrets such as connection strings to source control.  So we need to keep them out of the appsettings.json file.  

When running in the "Development" environment (ie an environment with that name) settings will be read from a secrets.json file as well as the appsettings.json file.  In production secrets could instead be saved to environment variables.

Setting the EnvironmentName to "Development"
--------------------------------------------
By default the host configuration can be read from environment variables prefixed by "DOTNET_".  Environment variable DOTNET_ENVIRONMENT sets the environment name.  This needs to be set to "Development" for the application to be able to read the secrets.json file.

Either set an environment variable on your development machine manually or set it in your project settings in Visual Studio.

### To set DOTNET_ENVIRONMENT environment variable in Visual Studio:

1. In the Solution Explorer right-click on the project then select Properties to open to the project properties dialog;

2. In the project properties dialog select Debug > General then click the link "Open debug launch profiles UI" to open the Launch Profiles dialog;

3. In the Launch Profiles dialog under Environment variables enter `DOTNET_ENVIRONMENT=Development` then close the dialog.  This will add a launchSettings.json file to the project, within a Properties folder.  The launchSettings.json file will set the environment variable.

To view the EnvironmentName in the Application
----------------------------------------------
When building the host, use the .ConfigureAppConfiguration overload that takes a delegate with two parameters - a HostBuilderContext and an IConfigurationBuilder.  You can then view the environment name from `HostBuilderContext.HostingEnvironment.EnvironmentName`.  For example:

```
using IHost host = Host.CreateDefaultBuilder()
.ConfigureAppConfiguration((hostingContext, app) =>
{
    app.AddJsonFile("appsettings.json");

    var env = hostingContext.HostingEnvironment;
    Console.WriteLine(env.EnvironmentName);
})
...
```

To Add a secrets.json File
--------------------------
### Prerequisities - Required NuGet Packages:
* Microsoft.Extensions.Configuration.UserSecrets

### Steps
1. In Visual Studio Solution Explorer right click project then select "Manage User Secrets" (NOTE: This will be present but won't do anything if the NuGet package isn't installed).  This will create the secrets.json file and open it.

2. Move the secret settings from appsettings.json to the secrets.json.  The settings must have the same structure in the secrets.json as they did in appsettings.json.

### Notes
1. Selecting "Manage User Secrets" in the Visual Studio Solution Explorer will add a `UsersSecretsId` element to the project file.  This will contain a GUID which is the user secrets ID (any text will do for the `UsersSecretsId` but Visual Studio will use a GUID).  NOTE: This can also be done from the .NET CLI via `dotnet user-secrets init`.

2. The secrets.json file will be saved to the user's profile at `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`. 

3. No change is needed to read the secrets.json file.  The built-in IConfiguration object will read settings from that file as if they were in the appsettings.json file.

4. The settings in the secrets.json file will be flattened if secrets are added or removed via the .NET CLI commands `dotnet user-secrets set` or `dotnet user-secrets remove`.  This is okay; the paths to each setting will be maintained.  For excample, if the secrets.json file originally looks like: 

```
{
  "Movies": {
    "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=Movie-1;Trusted_Connection=True;MultipleActiveResultSets=true",
    "ServiceApiKey": "12345"
  }
}
```

and the following .NET CLI command is run: `dotnet user-secrets remove "Movies:ConnectionString"`

then the remaining setting in the secrets.json file will be flattened to:

```
{
  "Movies:ServiceApiKey": "12345"
}
```