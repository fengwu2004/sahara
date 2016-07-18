using Autofac;
using Sahara.Core.Bootstrapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core.IoC
{
    public class DependencyInjectionBootstrapper : Bootstrapper
    {
        private ContainerBuilder _containerBuilder;

        public override int Order { get { return 0; } }

        protected override void BeforeInitialize()
        {
            // TODO: Add logging
            this._containerBuilder = new ContainerBuilder();
        }

        protected override void AfterInitialize()
        {
            typeof(Container).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, 
                new[] { typeof(ContainerBuilder) }, null).Invoke(new object[] { this._containerBuilder });
            // TODO: Add logging
        }

        protected override void OnInitialize()
        {
            // TODO: Need to recursively load assemblies
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var installers = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => typeof(IDependencyInstaller).IsAssignableFrom(t))
                .Select(t => (IDependencyInstaller)Activator.CreateInstance(t));

            foreach (var installer in installers)
            {
                installer.Install(this._containerBuilder);
            }
        }
    }
}
