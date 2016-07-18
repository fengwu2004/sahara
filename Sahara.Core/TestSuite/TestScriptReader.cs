using Sahara.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    public class TestScriptReader : ITestScriptReader
    {
        public BaseTestScript Read(string path)
        {
            return Read(path, Encoding.UTF8);
        }

        public BaseTestScript Read(string path, Encoding encoding)
        {
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var script = File.ReadAllText(path, encoding); //Encoding.GetEncoding("gbk");
            var ext = Path.GetExtension(path);
            var name = Path.GetFileNameWithoutExtension(path);

            switch (ext)
            {
                case ".py":
                    {
                        return new PythonTestScript(name, script, path);
                    }
                    //case ".rb":
                    //    {
                    //        return new RubyTestScript(name, script, ns);
                    //    }
            }
            return null;
        }
    }
}
