using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    [Serializable]
    public class ExceptionTestResult : BaseTestResult
    {
        public Exception Error { get; private set; }

        public ExceptionTestResult(Exception ex)
            : base(ex.Message)
        {
            this.Error = new Exception(ex.Message);
        }
    }
}
