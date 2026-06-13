using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppWithConfiguration.ConfigurationOptions;
public class ObjectListOptions
{
    // Property name "ObjectList" matches the name of the array in the configuration file.
    public List<ObjectListItem> ObjectList { get; set; } = new();
}
