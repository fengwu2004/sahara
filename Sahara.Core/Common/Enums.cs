using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    public sealed class Language
    {
        public static readonly Language IronPython = new Language("ironPython");
        public static readonly Language IronRuby = new Language("ironRuby");
        public static readonly Language None = new Language("none");

        public readonly string Name;

        private Language(string name)
        {
            this.Name = name;
        }
    }

    [Serializable]
    public sealed class TestResultStatus
    {
        public static readonly TestResultStatus Pass = new TestResultStatus("PASS", "通过");
        public static readonly TestResultStatus Fail = new TestResultStatus("FAIL", "失败");
        public static readonly TestResultStatus Inconclusive = new TestResultStatus("INCONCLUSIVE", "未完成");

        public readonly string Name;
        public readonly string Value;


        private TestResultStatus(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public static TestResultStatus Parse(string value)
        {
            switch (value.ToUpper())
            {
                case "PASS":
                    return TestResultStatus.Pass;
                case "FAIL":
                    return TestResultStatus.Fail;
                default:
                    return TestResultStatus.Inconclusive;
            }
        }

        public override string ToString()
        {
            return this.Value;
        }
    }

    public sealed class ResultViewMode
    {
        public static readonly ResultViewMode Simple = new ResultViewMode("simple");
        public static readonly ResultViewMode Advanced = new ResultViewMode("advanced");

        public readonly string Name;

        public static ResultViewMode Parse(string name)
        {
            switch (name)
            {
                case "simple":
                    return Simple;
                case "advanced":
                    return Advanced;
                default:
                    return Advanced;
            }
        }

        public ResultViewMode(string mode)
        {
            this.Name = mode;
        }
    }
}
