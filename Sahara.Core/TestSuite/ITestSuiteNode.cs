using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    public interface ITestSuiteNode
    {
        string Name { get; }
        int Sequence { get; }
        string FilePath { get; }
    }
}
