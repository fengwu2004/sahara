using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using Sahara.Core;
using Autofac;
using System.Reflection;
using Sahara.Core.IoC;

namespace Sahara.Test
{
    [TestClass]
    public class VariableListTest
    {
        private TestProject project;

        [TestInitialize]
        public void Init()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<FileSystemTestSuiteManager>().As<ITestSuiteReader>();
            containerBuilder.RegisterType<TestProjectManager>().SingleInstance();
            containerBuilder.RegisterType<TestScriptReader>().SingleInstance();
            typeof(Container).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] { typeof(ContainerBuilder) }, null).Invoke(new object[] { containerBuilder });

            this.project = new TestProject()
            {
                Name = "Variable List Test Project",
                Description = "Variable List Test Project",
                ProjectVariables = new VariableList()
                {
                    Variables = new List<Variable>()
                    {
                        new Variable()
                        {
                            Name = "Var 1", Description = "Var 1 Desc", Value = "AAA", Group = "Test 1"
                        },
                        new Variable()
                        {
                            Name = "Var 2", Description = "Var 2 Desc", Value = "BBB", Group = "Test 2"
                        },
                        new Variable()
                        {
                            Name = "Var 3", Description = "Var 3 Desc", Value = "CCC", Group = "Test 1"
                        },
                    }
                }
            };
        }

        [TestMethod]
        public void Should_BeAbleTo_UpdateGroupName()
        {
            Assert.AreEqual<int>(0, this.project.ProjectVariables.Variables.Count(v => v.Group == "Test 3"));
            this.project.ProjectVariables.UpdateGroupName("Test 1", "Test 3");
            Assert.AreEqual<int>(0, this.project.ProjectVariables.Variables.Count(v => v.Group == "Test 1"));
            Assert.AreEqual<int>(2, this.project.ProjectVariables.Variables.Count(v => v.Group == "Test 3"));
        }

        [TestMethod]
        public void Should_BeAbleTo_AddVariable()
        {
            Assert.AreEqual<int>(2, this.project.ProjectVariables.Variables.Count(v => v.Group == "Test 1"));
            var variable1 = new Variable("test_var", "test_var_desc", "test_var_value", "Test 1");
            this.project.ProjectVariables.AddVariable(variable1);
            Assert.AreEqual<int>(3, this.project.ProjectVariables.Variables.Count(v => v.Group == "Test 1"));
        }

        [TestMethod]
        public void Should_BeAbleTo_UpdateVariable()
        {
            var variables = this.project.ProjectVariables.Variables;
            var var1 = variables.First(v => v.Name == "Var 1");
            Assert.IsNotNull(var1);
            var1.Value = "CCC";
            this.project.ProjectVariables.UpdateVariable(var1);
            var updatedVar1 = variables.First(v => v.Name == "Var 1");
            Assert.AreEqual<string>("CCC", updatedVar1.Value);
        }

        [TestMethod]
        public void Should_BeAbleTo_RemoveVariable()
        {
            Assert.AreEqual<int>(1, this.project.ProjectVariables.Variables.Count(v => v.Name == "Var 2"));
            this.project.ProjectVariables.RemoveVariable("Var 2");
            Assert.AreEqual<int>(0, this.project.ProjectVariables.Variables.Count(v => v.Name == "Var 2"));
        }
    }
}
