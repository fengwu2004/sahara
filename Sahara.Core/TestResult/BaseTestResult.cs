using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    [Serializable]
    public abstract class BaseTestResult : ITestResult
    {
        public BaseTestResult(string message)
        {
            this.Message = message;
            this.Timestamp = DateTime.Now;
        }

        public DateTime Timestamp { get; private set; }

        public virtual string Message { get; protected set; }
    }
}
