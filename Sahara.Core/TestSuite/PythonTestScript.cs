using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    public class PythonTestScript : BaseTestScript
    {
        private string _name;
        private int _sequence;

        public PythonTestScript(string name, string content, string filePath)
        {
            // TODO: Represents order in the script's group
            this._sequence = 0;
            this._name = name;
            this.Content = content;
            this.FilePath = filePath;
        }

        public override Language Language
        {
            get { return Sahara.Core.Language.IronPython; }
        }

        public override string Name
        {
            get { return this._name; }
        }

        public override int Sequence
        {
            get { return this._sequence; }
        }
    }
}
