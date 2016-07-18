using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    public abstract class BaseTestScript : ITestSuiteNode
    {
        public string Content { get; set; }

        public abstract Language Language { get; }

        public abstract string Name { get; }

        public string FilePath { get; protected set; }

        public abstract int Sequence { get; }

        public virtual void Save()
        {
            using (var writer = new StreamWriter(FilePath, false))
            {
                writer.Write(Content);
            }
        }
    }
}
