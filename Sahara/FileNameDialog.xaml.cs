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
    /// <summary>
    /// Interaction logic for FileNameDialog.xaml
    /// </summary>
    public partial class FileNameDialog : Window
    {
        public FileNameDialog(string path, bool isCreating = false, bool isFolder = false)
        {
            InitializeComponent();
            var targetType = isFolder ? "文件夹" : "文件";
            this.txtAnswer.Text = path;
            this.lblQuestion.Content = isCreating ? 
                string.Format("请输入{0}名：", targetType) : 
                string.Format("将此{0}重命名为：", targetType);
            this.Title = isCreating ? "新建测试脚本" : "重命名测试脚本";
        }

        public string Path
        {
            get
            {
                return this.txtAnswer.Text;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.txtAnswer.SelectAll();
            this.txtAnswer.Focus();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
