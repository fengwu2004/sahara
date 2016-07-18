using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using Sahara.Core.IoC;
using Sahara.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Infrastructure
{
    /// <summary>
    /// Instantiate view models
    /// </summary>
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(Container.Current));
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public PreferencePaneViewModel PreferencePane
        {
            get { return ServiceLocator.Current.GetInstance<PreferencePaneViewModel>(); }
        }
        
        public TestProjectSettingViewModel TestProjectSettings
        {
            get { return ServiceLocator.Current.GetInstance<TestProjectSettingViewModel>(); }
        }
    }
}
