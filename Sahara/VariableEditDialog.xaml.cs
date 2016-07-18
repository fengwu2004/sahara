using Sahara.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sahara
{
    internal delegate void VariableEditEventHandler(object sender, Variable var);

    public partial class VariableEditDialog : Window
    {
        private Variable variable;

        internal event VariableEditEventHandler Save;

        public VariableEditDialog()
        {
            InitializeComponent();
            this.variable = new Variable();
        }

        public VariableEditDialog(Variable variable)
            : this()
        {
            if (variable != null)
            {
                this.variable = new Variable(variable.Name, variable.Description, variable.Value, variable.Group);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this.variable;
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            var toSave = this.DataContext as Variable;

            if (this.Save != null && toSave != null)
            {
                if (!toSave.Validate())
                {
                    MessageBox.Show("请检查变量格式");
                    return;
                }

                this.Save(this, this.DataContext as Variable);
            }
        }
    }
}
