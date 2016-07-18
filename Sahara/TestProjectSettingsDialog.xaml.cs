using Autofac;
using Sahara.Core;
using Sahara.Core.IoC;
using Sahara.Infrastructure;
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
    /// Interaction logic for TestProjectSettingsDialog.xaml
    /// </summary>
    public partial class TestProjectSettingsDialog : Window
    {
        private ViewModelLocator viewModelLocator;

        private TestProjectSettingsDialog()
        {
            InitializeComponent();
            viewModelLocator = Container.Current.Resolve<ViewModelLocator>();
        }

        public TestProjectSettingsDialog(TestProject project = null) 
            : this()
        {
            var viewModel = viewModelLocator.TestProjectSettings;
            viewModel.TestProject = project ?? new TestProject();
            viewModel.AllowDeviceSelection = (project == null);
            this.Title = project == null ? "新建测试项目" : "测试项目配置";
            this.DataContext = viewModel;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OnConfirm(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedItem = e.AddedItems[0];
                var dataGrid = (DataGrid)sender;
                dataGrid.ScrollIntoView(selectedItem);
            }
        }
    }
}
