using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sahara.Core
{
    public enum VariableType
    {
        ARRAY,
        BOOLEAN,
        STRING,
        UINT,
        INT
    }

    [Serializable]
    [XmlType("Variable")]
    public sealed class Variable
    {
        private static string INT_REG_EXP = "^(?:\\+|\\-)[0-9]+$";
        private static string UINT_REG_EXP = "^[0-9]+$";
        private static string BOOLEAN_REG_EXP = "^(?:T|F)$";
        private static string ARRAY_REG_EXP = "^\\{(.*)\\}$";
        private static string STRING_REG_EXP = "^\".*\"$";
        private string FLOAT_REG_EXP = "^[0-9]+\\.[0-9]+$";

        public Variable() { }

        public Variable(string name, string description, string value, string group)
        {
            this.Name = name;
            this.Description = description;
            this.Value = value;
            this.Group = group;
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public string Value { get; set; }

        [XmlAttribute]
        public string Group { get; set; }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(this.Name) && (
                Regex.IsMatch(this.Value, BOOLEAN_REG_EXP, RegexOptions.IgnoreCase) ||
                Regex.IsMatch(this.Value, FLOAT_REG_EXP) ||
                Regex.IsMatch(this.Value, INT_REG_EXP) ||
                Regex.IsMatch(this.Value, UINT_REG_EXP) ||
                Regex.IsMatch(this.Value, ARRAY_REG_EXP) ||
                Regex.IsMatch(this.Value, STRING_REG_EXP)
                );
        }
    }
}
