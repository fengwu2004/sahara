using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sahara.Core.Utils
{
    public static class VariableTypeConverter
    {
        private static string INT_REG_EXP = "^(?:\\+|\\-)[0-9]+$";
        private static string UINT_REG_EXP = "^[0-9]+$";
        private static string FLOAT_REG_EXP = "^[0-9]+\\.[0-9]+$";
        private static string BOOLEAN_REG_EXP = "^(?:T|F)$";
        private static string ARRAY_REG_EXP = "^\\{(.*)\\}$";

        /// <summary>
        /// Parse and convert the variable to a concrete type
        /// </summary>
        /// <param name="variable">The variable to be converted</param>
        /// <returns>The converted value</returns>
        public static object Convert(string variable)
        {
            if (Regex.IsMatch(variable, BOOLEAN_REG_EXP, RegexOptions.IgnoreCase))
            {
                return string.Equals(variable, "T", StringComparison.InvariantCultureIgnoreCase);
            }

            if (Regex.IsMatch(variable, INT_REG_EXP))
            {
                return System.Convert.ToInt32(variable);
            }

            if (Regex.IsMatch(variable, FLOAT_REG_EXP))
            {
                return System.Convert.ToDouble(variable);
            }

            if (Regex.IsMatch(variable, UINT_REG_EXP))
            {
                return System.Convert.ToUInt32(variable);
            }

            if (Regex.IsMatch(variable, ARRAY_REG_EXP))
            {
                var matches = Regex.Matches(variable, ARRAY_REG_EXP);
                var contents = matches[0].Groups[1].Value;
                var members = contents.Explode();
                var arr = new List<object>();
                foreach (var member in members)
                {
                    arr.Add(Convert(member));
                }
                return arr;
            }

            return variable.Trim('"');
        }
    }
}
