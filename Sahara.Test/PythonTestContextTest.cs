using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sahara.Core;
using Sahara.Core.Configuration;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Sahara.Test
{
    [TestClass]
    public class PythonTestContextTest
    {
        private PythonTestContext context;

        private const string SAMPLE_LOG_MESSAGE = "import sys; sys.stdout.write('Logging'); FinalResult='PASS'";
        private const string SAMPLE_ERROR_MESSAGE = "import sys; sys.stderr.write('Error'); FinalResult='PASS'";
        private const string SAMPLE_RAISED_EXCEPTION = "raise Exception('Exception')";
        private const string SAMPLE_AGGREGATE_ERRORS = "import sys; sys.stdout.write('Logging');" +
            "sys.stderr.write('Error');" +
            "raise Exception('Exception');";
        private const string SAMPLE_MULTI_MESSAGES = "import sys; sys.stdout.write('Log 1');" + 
            "sys.stdout.write('Log 2');" +
            "sys.stderr.write('Error 1');" +
            "sys.stderr.write('Error 2');" +
            "FinalResult='PASS'";
        private const string SAMPLE_SYNTAX_ERROR = "print 'test";
        private const string SAMPLE_PRINT_VARIABLE = "import sys; sys.stdout.write(foo)";
        private const string SAMPLE_IMPORT_MODULE = "from context import *; FinalResult=\"PASS\"";
        private const string SAMPLE_IMPORT_CLR_MODULE = "import clr; clr.AddReferenceToFile(\"iec61850dotnet.dll\"); FinalResult=\"PASS\"";
        private const string SAMPLE_IMPORT_VARIABLES = "import sys; sys.stdout.write(conf['test1'].Value)";
        private const string SAMPLE_FINAL_RESULT_PASS = "import clr; FinalResult=\"PASS\"";
        private const string SAMPLE_FINAL_RESULT_FAIL = "import clr; raise Exception('Error')";

        private DirectoryInfo assetRoot = new DirectoryInfo("../../../assets");

        [TestMethod]
        public void Should_CaptureExceptionRaised()
        {
            this.context = new PythonTestContext("Should_CaptureExceptionRaised", SAMPLE_RAISED_EXCEPTION);
            var results = context.Execute();
            Assert.IsNotNull(results);
            Assert.AreEqual<int>(1, results.Results.Count);
            Assert.IsInstanceOfType(results.Results[0], typeof(ExceptionTestResult));
            Assert.AreEqual<string>("Exception", ((ExceptionTestResult)results.Results[0]).Message);
        }

        [TestMethod]
        public void Should_CaptureLogMessages()
        {
            this.context = new PythonTestContext("Should_CaptureLogMessages", SAMPLE_LOG_MESSAGE);
            var results = context.Execute();
            Assert.IsNotNull(results);
            Assert.AreEqual<int>(1, results.Results.Count);
            Assert.IsInstanceOfType(results.Results[0], typeof(LogTestResult));
            Assert.AreEqual<string>("Logging", ((LogTestResult)results.Results[0]).Message);
        }

        [TestMethod]
        public void Should_CaptureErrorMessages()
        {
            this.context = new PythonTestContext("Should_CaptureErrorMessages", SAMPLE_ERROR_MESSAGE);
            var results = context.Execute();
            Assert.IsNotNull(results);
            Assert.AreEqual<int>(1, results.Results.Count);
            Assert.IsInstanceOfType(results.Results[0], typeof(ErrorTestResult));
            Assert.AreEqual<string>("Error", ((ErrorTestResult)results.Results[0]).Message);
        }

        [TestMethod]
        public void Should_AggregateErrors()
        {
            this.context = new PythonTestContext("Should_AggregateErrors", SAMPLE_AGGREGATE_ERRORS);
            var results = context.Execute();
            Assert.IsNotNull(results);
            Assert.AreEqual<int>(3, results.Results.Count);
        }

        [TestMethod]
        public void Should_CaptureMultipleLogAndErrorsInSequence()
        {
            this.context = new PythonTestContext("Should_CaptureMultipleLogAndErrorsInSequence", SAMPLE_MULTI_MESSAGES);
            var results = context.Execute();
            Assert.IsNotNull(results);
            Assert.AreEqual<int>(4, results.Results.Count);
            Assert.IsInstanceOfType(results.Results[0], typeof(LogTestResult));
            Assert.IsInstanceOfType(results.Results[1], typeof(LogTestResult));
            Assert.IsInstanceOfType(results.Results[2], typeof(ErrorTestResult));
            Assert.IsInstanceOfType(results.Results[3], typeof(ErrorTestResult));
            Assert.AreEqual<string>("Log 1", ((LogTestResult)results.Results[0]).Message);
            Assert.AreEqual<string>("Log 2", ((LogTestResult)results.Results[1]).Message);
            Assert.AreEqual<string>("Error 1", ((ErrorTestResult)results.Results[2]).Message);
            Assert.AreEqual<string>("Error 2", ((ErrorTestResult)results.Results[3]).Message);
        }

        [TestMethod]
        public void Should_CaptureSyntaxErrors()
        {
            this.context = new PythonTestContext("Should_CaptureSyntaxErrors", SAMPLE_SYNTAX_ERROR);
            var results = context.Execute();
            Assert.IsNotNull(results);
            Assert.AreEqual<int>(1, results.Results.Count);
            Assert.IsInstanceOfType(results.Results[0], typeof(SyntaxExceptionTestResult));
            var testResult = ((SyntaxExceptionTestResult)results.Results[0]);
            Assert.AreEqual<int>(1, testResult.Line);
            Assert.AreEqual<int>(7, testResult.Column);
            Assert.IsTrue(testResult.Message.StartsWith("Line: 1, Column: 7,"));
        }
        
        [TestMethod]
        public void Should_ImportPythonModules()
        {
            var settings = UserSettings.GetInstance();
            settings.Reset();
            settings.AddPath(Path.Combine(assetRoot.FullName, "dll"));
            settings.AddPath(Path.Combine(assetRoot.FullName, "lib"));
            this.context = new PythonTestContext("Should_ImportModules", SAMPLE_IMPORT_MODULE);
            var results = context.Execute();
            Assert.AreEqual<int>(0, results.Results.Count);
        }

        [TestMethod]
        public void Should_ImportClrModules()
        {
            var settings = UserSettings.GetInstance();
            settings.Reset();
            settings.AddPath(Path.Combine(assetRoot.FullName, "dll"));
            settings.AddPath(Path.Combine(assetRoot.FullName, "lib"));
            this.context = new PythonTestContext("Should_ImportClrModules", SAMPLE_IMPORT_CLR_MODULE);
            var results = context.Execute();
            Assert.AreEqual<int>(0, results.Results.Count);
        }

        [TestMethod]
        public void Should_PickUpFinalResult()
        {
            this.context = new PythonTestContext("Should_PickUpFinalResult_PASS", SAMPLE_FINAL_RESULT_PASS);
            var results = context.Execute();
            Assert.AreEqual<string>("PASS", results.Status.Value);
            this.context = new PythonTestContext("Should_PickUpFinalResult_FAIL", SAMPLE_FINAL_RESULT_FAIL);
            results = context.Execute();
            Assert.AreEqual<string>("FAIL", results.Status.Value);
        }

        [TestMethod]
        public void Should_ExecuteTestScript()
        {
            var reader = new TestScriptReader();
            var script = reader.Read(Path.Combine(assetRoot.FullName, "scripts/Ass1.py"));
            var settings = UserSettings.GetInstance();
            settings.Reset();
            settings.AddPath(Path.Combine(assetRoot.FullName, "dll"));
            settings.AddPath(Path.Combine(assetRoot.FullName, "lib"));
            this.context = new PythonTestContext(script.Name, script.Content);
            var results = this.context.Execute();
            Assert.IsTrue(results.Results.Count > 1);
        }
        
        [TestMethod]
        public void Should_ImportVariables()
        {
            var variables = new List<Variable>()
            {
                new Variable("test1", "test1 desc", "\"1\"", "group"),
                new Variable("test2", "test2 desc", "1", "group")
            };
            var project = new TestProject();
            project.ProjectVariables.Variables = variables;
            this.context = new PythonTestContext("Should_ImportProvidedVariables", SAMPLE_IMPORT_VARIABLES, project);
            var results = this.context.Execute();
            Assert.AreEqual<string>("\"1\"", results.Results[0].Message);
        }
    }
}
