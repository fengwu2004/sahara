using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    [Serializable]
    public class ErrorTestResult : BaseTestResult
    {
        public ErrorTestResult(string message)
            : base(message) { }
    }
}
