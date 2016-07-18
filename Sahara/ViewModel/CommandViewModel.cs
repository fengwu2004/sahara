using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sahara.ViewModel
{
    public class CommandViewModel : ViewModelBase
    {
        public string SmallIcon { get; private set; }

        public string LargeIcon { get; private set; }

        public string Header { get; private set; }

        public ICommand Command { get; private set; }

        public CommandViewModel(Action onExecute)
            : this("", onExecute) { }

        public CommandViewModel(string header, Action onExecute)
            : this(header, "", onExecute) { }

        public CommandViewModel(string header, string icon, Action onExecute)
            : this(header, icon, icon, onExecute) { }

        public CommandViewModel(string header, string smallIcon, string largeIcon, Action onExecute)
        {
            this.SmallIcon = smallIcon;
            this.LargeIcon = largeIcon;
            this.Header = header;
            this.Command = new RelayCommand(onExecute);
        }
    }
}
