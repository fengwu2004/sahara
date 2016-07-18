using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sahara.Core.Utils;
using Sahara.Core;
using Autofac;
using Sahara.Core.IoC;
using System.Reflection;

namespace Sahara.Test
{
    [TestClass]
    public class TemplateEngineTest
    {
        private TemplateEngine engine;

        [TestInitialize]
        public void Init()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<TemplateEngine>().SingleInstance();
            typeof(Container).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] { typeof(ContainerBuilder) }, null).Invoke(new object[] { containerBuilder });
            this.engine = Container.Current.Resolve<TemplateEngine>();
            this.engine.Register(typeof(LogTestResult), "This is {{Message}}");
        }

        [TestMethod]
        public void Should_BeAbleTo_RenderTemplateByType()
        {
            var result = this.engine.Render(new LogTestResult("test result"));
            Assert.AreEqual<string>("This is test result", result);

            //var builder = new TestResultBuilder();
            //builder.Results.Add(new LogTestResult("test test"));
            //builder.Results.Add(new LogTestResult("test test"));
            //builder.Results.Add(new LogTestResult("test test"));
            //builder.Save("result.txt");
        }
    }
}
