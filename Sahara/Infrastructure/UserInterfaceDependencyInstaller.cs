using Autofac;
using Sahara.Core;
using Sahara.Core.IoC;
using Sahara.Core.Utils;
using Sahara.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Infrastructure
{
    public class UserInterfaceDependencyInstaller : IDependencyInstaller
    {
        public void Install(ContainerBuilder container)
        {
            //container.RegisterType<PythonTestContext>().As<BaseTestContext>();
            container.RegisterType<FileSystemTestSuiteManager>().As<ITestSuiteReader>();
            container.RegisterType<TestScriptReader>().SingleInstance();
            container.RegisterType<TestProjectManager>().SingleInstance();
            container.RegisterType<TemplateEngine>().SingleInstance();

            container.RegisterType<MainViewModel>().SingleInstance();
            container.RegisterType<PreferencePaneViewModel>().SingleInstance();
            container.RegisterType<TestProjectSettingViewModel>().SingleInstance();

            container.RegisterType<ViewModelLocator>().SingleInstance();
        }
    }
}
