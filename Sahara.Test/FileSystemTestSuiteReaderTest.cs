using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sahara.Core;
using Sahara.Core.IoC;
using Autofac;
using System.Reflection;

namespace Sahara.Test
{
    [TestClass]
    public class FileSystemTestSuiteReaderTest
    {
        private DirectoryInfo assetRoot = new DirectoryInfo("../../../assets");

        [TestInitialize]
        public void Init()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TestScriptReader>().SingleInstance();
            typeof(Container).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] { typeof(ContainerBuilder) }, null).Invoke(new object[] { builder });
        }

        [TestMethod]
        public void Should_ReadAndParseIgnoreAndManifest()
        {
            var reader = new FileSystemTestSuiteManager();
            reader.TestSuiteRoot = Path.Combine(assetRoot.FullName, "scripts");
            Assert.AreEqual<int>(1, reader.Ignored.Count);
            Assert.AreEqual<string>("1.0", reader.Manifest["VERSION"]);
            Assert.AreEqual<string>("test", reader.Manifest["DEVICE"]);
        }

        [TestMethod]
        public void Should_IgnoreTestScriptsAccordingToSaharaIgnore()
        {
            var reader = new FileSystemTestSuiteManager();
            reader.TestSuiteRoot = Path.Combine(assetRoot.FullName, "scripts");
            var root = reader.Read() as TestScriptGroup;
            var uiGroup = root.Nodes.Find(n => n.Name == "UI") as TestScriptGroup; // UI
            var subGroup = uiGroup.Nodes.Find(n => n.Name == "Sub") as TestScriptGroup; // Sub
            Assert.IsNotNull(root);
            Assert.IsNotNull(uiGroup);
            Assert.IsNotNull(subGroup);
            Assert.AreEqual<int>(7, root.Nodes.Count);
            Assert.AreEqual<int>(1, uiGroup.Nodes.Count);
            Assert.AreEqual<int>(1, subGroup.Nodes.Count);
            Assert.AreEqual<string>("dialog", subGroup.Nodes[0].Name);
        }
    }
}
