using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core.Bootstrapping
{
    public interface IBootstrapper
    {
        int Order { get; }
        void Initialize();
    }
}
