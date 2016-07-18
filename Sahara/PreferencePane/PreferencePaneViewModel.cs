using GalaSoft.MvvmLight;
using Sahara.Core;
using Sahara.Core.Configuration;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows.Forms;
using System.Net;

namespace Sahara.ViewModel
{
    public class PreferencePaneViewModel : ViewModelBase
    {
        private UserSettings settings;

        public ObservableCollection<string> SystemPaths { get; private set; }

        public string SelectedPath { get; set; }

        public string SelectedBootstrappingScript
        {
            get
            {
                return settings.BootstrappingScript;
            }
            set
            {
                settings.BootstrappingScript = value;
                RaisePropertyChanged("SelectedBootstrappingScript");
            }
        }

        public IEnumerable<NetworkInterface> NetworkInterfaces
        {
            get
            {   
                return NetworkInterface.GetAllNetworkInterfaces();
            }
        }

        public IEnumerable<string> IPAddressList1
        {
            get
            {
                if (SelectedNIC1 == null) return new List<string>();
                return SelectedNIC1.GetIPProperties().UnicastAddresses
                    .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(ip => ip.Address.ToString());
            }
        }

        public IEnumerable<string> IPAddressList2
        {
            get
            {
                if (SelectedNIC2 == null) return new List<string>();
                return SelectedNIC2.GetIPProperties().UnicastAddresses
                    .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(ip => ip.Address.ToString());
            }
        }

        private NetworkInterface nic1;
        public NetworkInterface SelectedNIC1
        {
            get { return nic1; }
            set
            {
                nic1 = value;
                RaisePropertyChanged("SelectedNIC1");
                RaisePropertyChanged("IPAddressList1");
            }
        }

        private NetworkInterface nic2;
        public NetworkInterface SelectedNIC2
        {
            get
            {
                return nic2;
            }
            set
            {
                nic2 = value;
                RaisePropertyChanged("SelectedNIC2");
                RaisePropertyChanged("IPAddressList2");
            }
        }

        private string selectedIp1;
        public string SelectedIP1
        {
            get { return selectedIp1; }
            set
            {
                selectedIp1 = value;
                RaisePropertyChanged("SelectedIP1");
            }
        }

        private string selectedIp2;
        public string SelectedIP2
        {
            get { return selectedIp2; }
            set
            {
                selectedIp2 = value;
                RaisePropertyChanged("SelectedIP2");
            }
        }

        public ObservableCollection<Device> Devices { get; private set; }

        private Device selectedDevice;
        public Device SelectedDevice
        {
            get
            {
                return this.selectedDevice;
            }
            set
            {
                this.selectedDevice = value;
                RaisePropertyChanged("SelectedDevice");
            }
        }

        public CommandViewModel AddSystemPathCommand { get; private set; }

        public CommandViewModel DelSystemPathCommand { get; private set; }

        public CommandViewModel AddDeviceCommand { get; private set; }

        public CommandViewModel RemDeviceCommand { get; private set; }

        public CommandViewModel SaveDeviceCommand { get; private set; }

        public CommandViewModel SelectBootstrappingScriptCommand { get; private set; }

        public CommandViewModel SaveNetworkInterfaceCommand { get; set; }

        public PreferencePaneViewModel()
        {
            settings = UserSettings.GetInstance();

            SelectedIP1 = settings.NIC1 == null ? "" : settings.NIC1.IP;

            SelectedIP2 = settings.NIC2 == null ? "" : settings.NIC2.IP;

            SelectedNIC1 = settings.NIC1 == null ?
                NetworkInterfaces.ElementAt(0) :
                NetworkInterfaces.SingleOrDefault(nic => nic.Id == settings.NIC1.ID);

            SelectedNIC2 = settings.NIC2 == null ?
                NetworkInterfaces.ElementAt(0) :
                NetworkInterfaces.SingleOrDefault(nic => nic.Id == settings.NIC2.ID);

            this.AddSystemPathCommand = new CommandViewModel(OnAddingSystemPath);
            this.DelSystemPathCommand = new CommandViewModel(OnDeletingSystemPath);
            this.AddDeviceCommand = new CommandViewModel(OnAddingDevice);
            this.RemDeviceCommand = new CommandViewModel(OnRemovingDevice);
            this.SaveDeviceCommand = new CommandViewModel(SaveDeviceList);
            this.SelectBootstrappingScriptCommand = new CommandViewModel(OnSelectingBootstrappingScript);
            this.SaveNetworkInterfaceCommand = new CommandViewModel(OnSaveNetworkInterface);

            this.SystemPaths = new ObservableCollection<string>(settings.Paths);
            this.SystemPaths.CollectionChanged += SaveSystemPaths;

            if (settings.Devices == null)
            {
                settings.Devices = new List<Device>();
            }

            this.Devices = new ObservableCollection<Device>(settings.Devices);
        }

        private void SaveSystemPaths(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var path in e.OldItems)
                {
                    settings.RemovePath(path.ToString());
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var path in e.NewItems)
                {
                    settings.AddPath(path.ToString());
                }
            }
        }

        private void OnAddingSystemPath()
        {
            var dialog = new FolderBrowserDialog()
            {
                Description = "请选择路径",
                RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = false
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    SystemPaths.Add(dialog.SelectedPath);
                    MessageBox.Show("系统路径已添加");
                }
            }
        }

        private void OnDeletingSystemPath()
        {
            if (!string.IsNullOrEmpty(SelectedPath))
            {
                SystemPaths.Remove(SelectedPath);
                MessageBox.Show("系统路径已删除");
            }
        }

        private void OnSelectingBootstrappingScript()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Python脚本|*.py",
                Title = "请选择脚本"
            };

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.SelectedBootstrappingScript = dialog.FileName;
                MessageBox.Show("初始化脚本保存成功");
            }
        }

        private void OnSaveNetworkInterface()
        {
            settings.NIC1 = new NIC()
            {
                ID = SelectedNIC1.Id,
                IP = SelectedIP1
            };

            settings.NIC2 = new NIC()
            {
                ID = SelectedNIC2.Id,
                IP = SelectedIP2
            };
            MessageBox.Show("网卡设置保存成功");
        }

        private void OnAddingDevice()
        {
            var newDevice = new Device() { Name = "未命名设备" };
            this.Devices.Add(newDevice);
            this.SelectedDevice = newDevice;
        }

        private void OnRemovingDevice()
        {
            var result = MessageBox.Show("是否确认删除此设备", "提示", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                this.Devices.Remove(this.SelectedDevice);
                this.SaveDeviceList();
                MessageBox.Show("设备已删除");
            }
        }

        private void SaveDeviceList()
        {
            settings.Devices = this.Devices.ToList();
        }
    }
}
