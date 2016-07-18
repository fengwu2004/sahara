using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    public interface ITestScriptReader
    {
        BaseTestScript Read(string path);
    }
}
