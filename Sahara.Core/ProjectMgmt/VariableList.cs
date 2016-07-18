using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sahara.Core
{
    [Serializable]
    public sealed class VariableList
    {
        public VariableList()
        {
            this.Variables = new List<Variable>();
        }

        public List<Variable> Variables { get; set; }

        public void UpdateGroupName(string oldName, string newName)
        {
            foreach (var variable in this.Variables.Where(v => v.Group == oldName))
            {
                variable.Group = newName;
            }
        }

        public void AddVariable(Variable variable)
        {
            this.Variables.Add(variable);
        }

        public void UpdateVariable(Variable variable)
        {
            var targetVar = this.Variables.FirstOrDefault(v => v.Name == variable.Name);
            if (targetVar != null)
            {
                targetVar = variable;
            }
        }

        public void RemoveVariable(string name)
        {
            var targetVariable = this.Variables.FirstOrDefault(v => v.Name == name);
            if (targetVariable != null)
            {
                this.Variables.Remove(targetVariable);
            }
        }
    }
}
