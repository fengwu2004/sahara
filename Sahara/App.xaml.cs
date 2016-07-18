using Autofac;
using Sahara.Core.Configuration;
using Sahara.Core.IoC;
using Sahara.Core.Bootstrapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Sahara.Infrastructure;

namespace Sahara
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var diBootstrapper = new DependencyInjectionBootstrapper();
            diBootstrapper.Initialize();

            App.Current.Resources.Add("Locator", Container.Current.Resolve<ViewModelLocator>());
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.GetCultureInfo("zh-CN");

            App.Current.DispatcherUnhandledException += HandleUnexpectedException;
        }

        private void HandleUnexpectedException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            string errorMessage = string.Format("发生未知错误，是否关闭应用程序。\n\n错误:{0}\n\n",

                e.Exception.Message + (e.Exception.InnerException != null ? "\n" +
                e.Exception.InnerException.Message : null));

            if (MessageBox.Show(errorMessage, "应用程序错误", MessageBoxButton.YesNoCancel, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                if (MessageBox.Show("提示：应用程序即将关闭", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Sahara.Properties.Settings.Default.Save();
        }

    }
}
