using Autofac;
using Fluent;
using Sahara.Core;
using Sahara.Core.IoC;
using Sahara.Core.Utils;
using Sahara.Infrastructure;
using Sahara.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Sahara
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private MainViewModel _viewModel;
        private ViewModelLocator _locator;

        public MainWindow()
        {
            InitializeComponent();
            this._locator = Container.Current.Resolve<ViewModelLocator>();
            this._viewModel = _locator.Main;
        }

        private void tvTestBrowser_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (e.NewValue is TestScriptViewModel)
                {
                    this._viewModel.SelectedTestScript = e.NewValue as TestScriptViewModel;
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _viewModel;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastOpenedProject))
            {
                this._viewModel.OpenTestProject(Properties.Settings.Default.LastOpenedProject);
            }
        }

        private void tvTestBrowser_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (item != null)
            {
                this._viewModel.SelectedNode = item.DataContext as ITreeViewNode;
            }
        }

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);
            return source as TreeViewItem;
        } 
    }
}
