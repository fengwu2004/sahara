using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sahara.Core
{
    public interface ITestResultBuilder
    {
        TestResultStatus Status { get; set; }

        void AddResult(BaseTestResult result);
    }
}
