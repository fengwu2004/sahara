using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    [Serializable]
    public class LogTestResult : BaseTestResult
    {
        public LogTestResult(string message)
            : base(message) { }
    }
}
