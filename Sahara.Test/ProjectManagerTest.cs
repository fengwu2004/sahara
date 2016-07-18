using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sahara.Core;
using Sahara.Core.IoC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Sahara.Test
{
    [TestClass]
    public class ProjectManagerTest
    {
        private TestProjectManager _manager;

        [TestInitialize]
        public void Init()
        {
            //this._manager = TestProjectManager.GetInstance();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<FileSystemTestSuiteManager>().As<ITestSuiteReader>();
            containerBuilder.RegisterType<TestProjectManager>().SingleInstance();
            containerBuilder.RegisterType<TestScriptReader>().SingleInstance();
            typeof(Container).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] { typeof(ContainerBuilder) }, null).Invoke(new object[] { containerBuilder });
            this._manager = Container.Current.Resolve<TestProjectManager>();
        }

        [TestMethod]
        public void Should_BeAbleTo_SerializeTestProjectObject()
        {
            var testProject = new TestProject()
            {
                Name = "Sample",
                Description = "This is sample",
                TestSuitePath = "C:\\Scripts",
                ProjectVariables = new VariableList()
                {
                    Variables = new List<Variable>()
                    {
                        new Variable() { Name = "TEST_BOOLEAN", Description = "This is test boolean",
                            Value = "true", Group = "Default" },
                        new Variable() { Name = "TEST_STRING", Description = "This is test string",
                            Value = "ABCDE", Group = "Test" }
                    }
                },
                ResultTemplate = "####{{ABC}}###",
                ProjectConfigPath = "sample.xml"
            };

            this._manager.Save(testProject);

            Assert.IsTrue(File.Exists("sample.xml"));
        }

        [TestMethod]
        public void Should_BeAbleTo_DeserializeTestProjectXML()
        {
            var testProject = this._manager.LoadFromXml("../../../assets/sample-project/sample-project.xml");
            Assert.IsNotNull(testProject);
            Assert.AreEqual<string>("Test Project", testProject.Name);
            Assert.AreEqual<string>("This is sample", testProject.Description);
        }

        [TestMethod]
        public void Should_BeAbleTo_SerializeVariableList()
        {
            var project = new TestProject();
            project.ProjectVariables = new VariableList()
            {
                Variables = new List<Variable>()
                {
                    new Variable("Test0", "Test variable description", "Test variable value 0", "test0"),
                    new Variable("Test1", "Test variable description", "Test variable value 1", "test1"),
                    new Variable("Test2", "Test variable description", "Test variable value 2", "test2")
                }
            };
            project.ExportVariables("test.xml");
            Assert.IsTrue(File.Exists("test.xml"));
        }

        [TestMethod]
        public void Should_BeAbleTo_DeserializeVaraibleList()
        {
            var project = new TestProject();
            Assert.AreEqual<int>(0, project.ProjectVariables.Variables.Count);
            project.ImportVariables("test.xml");
            Assert.AreEqual<int>(3, project.ProjectVariables.Variables.Count);
        }
    }
}
