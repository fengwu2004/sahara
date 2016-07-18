using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Sahara.Core.Configuration;
using System.Reflection;
using System.IO;

namespace Sahara.Test
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void Should_ReadUserSettings()
        {
            var settings = UserSettings.GetInstance();
            settings.Reset();
            Assert.AreEqual<int>(0, settings.Paths.Count);
        }

        [TestMethod]
        public void Should_BeAbleToAddAndRemovePath()
        {
            var settings = UserSettings.GetInstance();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Dlls");
            settings.Reset();
            settings.AddPath(path);
            Assert.AreEqual<int>(1, settings.Paths.Count);
            settings.RemovePath(path);
            Assert.AreEqual<int>(0, settings.Paths.Count);
        }
    }
}
