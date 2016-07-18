using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    [Serializable]
    public class AggregateTestResult : ITestResult
    {
        public AggregateTestResult(string name, IList<BaseTestResult> results, TestResultStatus status = null)
        {
            this.Results = results.ToList();
            this.Status = status;
        }

        public List<BaseTestResult> Results { get; private set; }

        public TestResultStatus Status { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var result in Results)
            {
                sb.AppendFormat("{0:HH:mm:ss} {1}\r\n", result.Timestamp, result.Message);
            }
            return sb.ToString();
        }
    }
}
