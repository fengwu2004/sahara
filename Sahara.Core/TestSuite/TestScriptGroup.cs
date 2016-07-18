using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    public class TestScriptGroup : ITestSuiteNode
    {
        private string _name;
        private int _sequence;

        public TestScriptGroup(string path)
        {
            // TODO: Reprensents order in it's parent tree
            this._sequence = 0;
            this._name = Path.GetFileName(path);
            this.FilePath = path;
            this.Nodes = new List<ITestSuiteNode>();
        }

        public List<ITestSuiteNode> Nodes { get; set; }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public int Sequence
        {
            get { return this._sequence; }
        }

        public string FilePath { get; protected set; }
    }
}
