using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppWithConfiguration
{
    public interface IServerOptions
    {
        string Name { get; set; }
        string OS { get; set; }
        bool HasInitializer { get; set; }
        bool NoInitializer { get; set; }
    }

    public class ServerOptions : IServerOptions
    {
        public string Name { get; set; } = string.Empty;
        public string OS { get; set; } = string.Empty;

        // This property does not have a corresponding value in appsettings.json. 
        // Not a problem though, as the property is initialized to true.
        public bool HasInitializer { get; set; } = true;

        // This property does not have a corresponding value in appsettings.json.
        // Not a problem though, as it will be set to the default value for the 
        // type (ie false).
        public bool NoInitializer { get; set; }
    }
}
