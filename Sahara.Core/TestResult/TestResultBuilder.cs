using Autofac;
using Sahara.Core.IoC;
using Sahara.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sahara.Core
{
    public class TestResultBuilder : ITestResultBuilder
    {
        //private TemplateEngine engine;

        public IList<BaseTestResult> Results { get; private set; }

        public TestResultStatus Status { get; set; }

        public string Message { get; set; }

        public TestResultBuilder()
        {
            //this.engine = Container.Current.Resolve<TemplateEngine>();
            this.Results = new List<BaseTestResult>();
            this.Status = TestResultStatus.Inconclusive;
        }

        public void AddResult(BaseTestResult result)
        {
            this.Results.Add(result);
        }

        public void Save(string path)
        {
            //using (var writer = new StreamWriter(path))
            //{
            //    writer.Write(this.engine.Render(new PersistableTestResult() { Results = this.Build() }));
            //}
        }
    }
}
