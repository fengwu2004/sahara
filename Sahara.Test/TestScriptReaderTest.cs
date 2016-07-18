using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sahara.Core;

namespace Sahara.Test
{
    [TestClass]
    public class TestScriptReaderTest
    {
        private DirectoryInfo assetRoot = new DirectoryInfo("../../../assets");

        [TestMethod]
        public void Should_ReadTestScriptFromFile()
        {
            var reader = new TestScriptReader();
            var script = reader.Read(Path.Combine(assetRoot.FullName, "scripts/UI/Sub/dialog.py"));
            Assert.IsInstanceOfType(script, typeof(PythonTestScript));
            Assert.AreEqual<string>("dialog", script.Name);
        }
    }
}
