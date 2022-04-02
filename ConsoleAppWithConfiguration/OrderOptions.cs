using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppWithConfiguration
{
    public class OrderOptions
    {
        public string Customer { get; set; } = string.Empty;
        public int Number { get; set; } = -1;

        public AddressOptions Address { get; } = new AddressOptions();
    }
}
