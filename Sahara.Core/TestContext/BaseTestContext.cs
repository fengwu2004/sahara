using Sahara.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sahara.Core
{
    [Serializable]
    public class TestResultEventArgs : EventArgs
    {
        public BaseTestResult Result { get; set; }
    }

    public delegate void TestContextMessageEventHandler(object sender, TestResultEventArgs args);

    public abstract class BaseTestContext : ITestContext<AggregateTestResult>
    {
        private const string CONFIG_SECTION_NAME = "sahara";

        public string Name { get; private set; }

        public string Source { get; private set; }

        public IList<Variable> Variables { get; set; }

        protected static IDictionary<string, string> GlobalVariables;
        protected static IList<string> GlobalSearchPaths;
        protected static UserSettings Settings;

        private BaseTestContext()
        {
            Settings = UserSettings.GetInstance();

            GlobalSearchPaths = new List<string>();
            foreach (var element in Settings.Paths)
            {
                GlobalSearchPaths.Add(element);
            }
        }

        public BaseTestContext(string name, string source) : this()
        {
            this.Name = name;
            this.Source = source;
        }

        public AggregateTestResult Execute()
        {
            var testResultBuilder = new TestResultBuilder();
            var mre = new ManualResetEvent(false);
            this.OnExecute(this.Source, testResultBuilder, mre);
            mre.WaitOne();
            return new AggregateTestResult(this.Name, testResultBuilder.Results, testResultBuilder.Status);
        }

        protected abstract void OnExecute(string script, ITestResultBuilder builder, ManualResetEvent mre);
        public abstract void Abort();
    }
}
