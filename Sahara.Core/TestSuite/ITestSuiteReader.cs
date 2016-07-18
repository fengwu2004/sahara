using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    public interface ITestSuiteReader
    {
        string TestSuiteRoot { get; set; }
        ITestSuiteNode Read();
    }
}
