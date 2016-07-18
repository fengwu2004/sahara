using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Sahara.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sahara.Core
{
    [Serializable]
    public class PythonVariable
    {
        public object Value { get; set; }
    }

    public class PythonTestContext : BaseTestContext
    {
        public event TestContextMessageEventHandler OnMessage;
        
        private Thread runner;

        public IList<string> ProjectSearchPaths;
        public Device Device { get; private set; }

        public PythonTestContext(string name, string source)
            : base(name, source)
        {
            this.Variables = new List<Variable>();
            this.ProjectSearchPaths = new List<string>();
        }

        public PythonTestContext(string name, string source, TestProject project)
            : base(name, source)
        {
            this.Variables = project.ProjectVariables.Variables;
            this.ProjectSearchPaths = project.Paths;
            this.Device = Settings.Devices.FirstOrDefault(d => d.Name == project.Device);
        }

        public override void Abort()
        {
            if (runner != null && runner.IsAlive)
            {
                runner.Abort();
            }
        }

        protected override void OnExecute(string script, ITestResultBuilder builder, ManualResetEvent mre)
        {
            var py = Python.CreateEngine();

            var modulePath = !string.IsNullOrEmpty(Settings.BootstrappingScript) ?
                Path.GetDirectoryName(Settings.BootstrappingScript) : "";

            var moduleName = !string.IsNullOrEmpty(Settings.BootstrappingScript) ?
                Path.GetFileNameWithoutExtension(Settings.BootstrappingScript) : "";

            var searchPaths = py.GetSearchPaths();
            searchPaths.Add(modulePath);
            searchPaths = searchPaths.Concat(GlobalSearchPaths
                .Select(path => Path.Combine(Directory.GetCurrentDirectory(), path)))
                .ToList();

            // Add project specific paths
            foreach (var path in this.ProjectSearchPaths)
            {
                searchPaths.Add(path);
            }
            py.SetSearchPaths(searchPaths);

            var scope = !string.IsNullOrEmpty(moduleName) ? py.ImportModule(moduleName) : py.CreateScope();

            // Set up conf
            var conf = new Dictionary<string, object>();
            foreach (var v in Variables)
            {
                conf.Add(v.Name, new PythonVariable() { Value = VariableTypeConverter.Convert(v.Value) });
            }
            scope.SetVariable("conf", conf);

            // Set up network interfaces
            var intfs = new List<NIC>() { Settings.NIC1, Settings.NIC2 };
            scope.SetVariable("nics", intfs);

            scope.SetVariable("device", Device);

            using (var stdOut = new MemoryStream())
            using (var stdErr = new MemoryStream())
            using (var stdOutWriter = new TestResultWriter(builder, typeof(LogTestResult)))
            using (var stdErrWriter = new TestResultWriter(builder, typeof(ErrorTestResult)))
            {
                stdOutWriter.OnWrite += (s, e) =>
                {
                    OnMessage?.Invoke(this, new TestResultEventArgs() { Result = e.TestResult });
                };

                stdErrWriter.OnWrite += (s, e) =>
                {
                    OnMessage?.Invoke(this, new TestResultEventArgs() { Result = e.TestResult });
                };

                py.Runtime.IO.SetOutput(stdOut, stdOutWriter);
                py.Runtime.IO.SetErrorOutput(stdErr, stdErrWriter);

                runner = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        py.Execute(script, scope);
                        builder.Status = TestResultStatus.Parse(scope.GetVariable("FinalResult"));
                    }
                    catch (SyntaxErrorException syntaxErrorException)
                    {
                        builder.AddResult(new SyntaxExceptionTestResult(syntaxErrorException));
                        builder.Status = TestResultStatus.Parse("FAIL");
                    }
                    catch (Exception ex)
                    {
                        builder.AddResult(new ExceptionTestResult(ex));
                        builder.Status = TestResultStatus.Parse("FAIL");
                    }
                    finally
                    {
                        mre.Set();
                    }
                }));
                runner.Start();
            }
        }
    }
}
