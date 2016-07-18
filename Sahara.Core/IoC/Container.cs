using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core.IoC
{
    public class Container
    {
        public static IContainer Current { get; private set; }
        protected Container(ContainerBuilder builder)
        {
            Container.Current = builder.Build();
        }
    }
}
