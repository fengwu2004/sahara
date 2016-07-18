using Autofac;
using Sahara.Core.IoC;
using Sahara.Infrastructure;
using Sahara.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sahara
{
    /// <summary>
    /// Interaction logic for PreferencePaneView.xaml
    /// </summary>
    public partial class PreferencePaneView : UserControl
    {
        private ViewModelLocator viewModelLocator;
        private PreferencePaneViewModel vm;

        public PreferencePaneView()
        {
            InitializeComponent();
            this.viewModelLocator = Container.Current.Resolve<ViewModelLocator>();
            this.vm = this.viewModelLocator.PreferencePane;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = vm;
        }
    }
}
