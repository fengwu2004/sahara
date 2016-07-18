using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    [Serializable]
    public sealed class Address
    {
        public string Name { get; set; }

        public string Street { get; set; }

        public string StreetNum { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}
