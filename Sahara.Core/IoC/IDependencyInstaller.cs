using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core.IoC
{
    public interface IDependencyInstaller
    {
        void Install(ContainerBuilder container);
    }
}
