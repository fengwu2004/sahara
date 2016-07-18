using Autofac;
using Sahara.Core.IoC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    internal class TestSuiteNodeComparer : Comparer<ITestSuiteNode>
    {
        private List<string> _index;

        public TestSuiteNodeComparer(IEnumerable<string> index)
        {
            _index = index.ToList();
        }

        public override int Compare(ITestSuiteNode x, ITestSuiteNode y)
        {
            var ix = _index.Exists(i => i.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase)) ?
                _index.IndexOf(x.Name) : 999;
            var iy = _index.Exists(i => i.Equals(y.Name, StringComparison.InvariantCultureIgnoreCase)) ?
                _index.IndexOf(y.Name) : 999;
            return ix.CompareTo(iy);
        }
    }

    public class FileSystemTestSuiteManager : ITestSuiteReader
    {
        private ITestScriptReader _testScriptReader;

        public List<string> Ignored { get; private set; }

        public Dictionary<string, string> Manifest { get; private set; }

        public FileSystemTestSuiteManager()
        {
            this._testScriptReader = Container.Current.Resolve<TestScriptReader>();
        }

        public ITestSuiteNode Read()
        {
            if (this.TestSuiteRoot == null)
                throw new ArgumentNullException("Source");

            return this.Read(this.TestSuiteRoot);
        }

        private string _testSuiteRoot;

        /// <summary>
        /// The source folder from which the FileSystemTestSuiteReader should read. While setting the source, 
        /// Sahara will look for .saharaIgnore and .saharaTestSuite, and parse them to get test suite configuration
        /// </summary>
        public string TestSuiteRoot
        {
            get
            {
                return this._testSuiteRoot;
            }
            set
            {
                this._testSuiteRoot = value;
                var files = Directory.GetFiles(this._testSuiteRoot, "*", SearchOption.AllDirectories);
                var saharaIgnore = files.FirstOrDefault(name => Path.GetFileName(name) == ".saharaIgnore");
                var saharaTestSuite = files.FirstOrDefault(name => Path.GetFileName(name) == ".saharaTestSuite");
                files = files.Where(name => name != saharaIgnore && name != saharaTestSuite).ToArray();

                this.Ignored = new List<string>();
                if (saharaIgnore != null)
                {
                    this.Ignored = File.ReadAllLines(saharaIgnore)
                        .Select(relPath => Path.Combine(this._testSuiteRoot, relPath)).ToList();
                }

                this.Manifest = new Dictionary<string, string>();
                if (saharaTestSuite != null)
                {
                    var properties = File.ReadAllLines(saharaTestSuite);
                    foreach (var property in properties)
                    {
                        var index = property.IndexOf('=');
                        var key = property.Substring(0, index).ToUpperInvariant();
                        var val = property.Substring(index + 1);
                        this.Manifest.Add(key, val);
                    }
                }
            }
        }

        private ITestSuiteNode Read(string path)
        {
            var group = new TestScriptGroup(path);
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);

            foreach (var dir in directories)
            {
                if (!Ignored.Exists(pattern => dir.StartsWith(pattern)))
                {
                    group.Nodes.Add(Read(dir));
                }
            }

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (!Ignored.Exists(pattern => file.StartsWith(pattern)) &&
                    !fileName.StartsWith("."))
                {
                    group.Nodes.Add(_testScriptReader.Read(file));
                }
            }

            var indexFilePath = Path.Combine(path, ".saharaIndex");
            if (File.Exists(indexFilePath))
            {
                var index = File.ReadAllLines(indexFilePath);
                group.Nodes.Sort(new TestSuiteNodeComparer(index));
            }

            return group;
        }

        public DirectoryInfo CreateTestScriptGroup(TestScriptGroup item)
        {
            if (Directory.Exists(item.FilePath))
            {
                return null;
            }
            return Directory.CreateDirectory(item.FilePath);
        }

        public bool CreateTestScript(BaseTestScript item)
        {
            FileStream stream = null;
            if (File.Exists(item.FilePath))
            {
                return false;
            }

            try
            {
                stream = File.Create(item.FilePath);
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
        
        [Obsolete]
        public void Create(ITestSuiteNode item)
        {
            if (item is BaseTestScript)
            {
                var script = item as BaseTestScript;
                FileStream stream = null;
                try
                {
                    if (!File.Exists(script.FilePath))
                    {
                        stream = File.Create(script.FilePath);
                    }
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
            }
            else
            {
                var group = item as TestScriptGroup;
                if (!Directory.Exists(group.FilePath))
                {
                    Directory.CreateDirectory(group.FilePath);
                }
            }
        }

        public void Delete(ITestSuiteNode item)
        {
            if (item is BaseTestScript)
            {
                var script = item as BaseTestScript;
                File.Delete(script.FilePath);
            }
            else
            {
                var group = item as TestScriptGroup;
                Directory.Delete(group.FilePath, true);
            }
        }
    }
}
