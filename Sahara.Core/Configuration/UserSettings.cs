using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core.Configuration
{
    public sealed class UserSettings
    {
        private static UserSettings settings;

        public static UserSettings GetInstance()
        {
            if (settings == null)
            {
                settings = new UserSettings();
            }
            return settings;
        }

        private UserSettings()
        {
            this.Paths = new HashSet<string>(SaharaUserSettings.Default.Paths.Split(new[] { ';' },
                StringSplitOptions.RemoveEmptyEntries));
        }

        private void SavePaths()
        {
            SaharaUserSettings.Default.Paths = string.Join(";", this.Paths);
            SaharaUserSettings.Default.Save();
        }
        
        public void AddPath(string path)
        {
            this.Paths.Add(path);
            this.SavePaths();
        }

        public void RemovePath(string path)
        {
            if (this.Paths.Contains(path))
            {
                this.Paths.Remove(path);
                this.SavePaths();
            }
        }

        public void Reset()
        {
            this.Paths.Clear();
            //this.Variables.Clear();
            SaharaUserSettings.Default.Reset();
        }

        public ISet<string> Paths { get; private set; }

        public NIC NIC1
        {
            get
            {
                return JsonConvert.DeserializeObject<NIC>(SaharaUserSettings.Default.NIC1);
            }
            set
            {
                SaharaUserSettings.Default.NIC1 = JsonConvert.SerializeObject(value);
                SaharaUserSettings.Default.Save();
            }
        }

        public NIC NIC2
        {
            get
            {
                return JsonConvert.DeserializeObject<NIC>(SaharaUserSettings.Default.NIC2);
            }
            set
            {
                SaharaUserSettings.Default.NIC2 = JsonConvert.SerializeObject(value);
                SaharaUserSettings.Default.Save();
            }
        }

        public IEnumerable<Device> Devices
        {
            get
            {
                return JsonConvert.DeserializeObject<List<Device>>(SaharaUserSettings.Default.Devices);
            }
            set
            {
                SaharaUserSettings.Default.Devices = JsonConvert.SerializeObject(value);
                SaharaUserSettings.Default.Save();
            }
        }

        public ResultViewMode Mode
        {
            get
            {
                return ResultViewMode.Parse(SaharaUserSettings.Default.Mode);
            }
            set
            {
                SaharaUserSettings.Default.Mode = value.Name;
                SaharaUserSettings.Default.Save();
            }
        }

        public Encoding Encoding
        {
            get
            {
                var codePage = SaharaUserSettings.Default.Encoding == 0 ? 
                    Encoding.UTF8.CodePage : SaharaUserSettings.Default.Encoding;
                return Encoding.GetEncoding(codePage);
            }
            set
            {
                SaharaUserSettings.Default.Encoding = value.CodePage;
                SaharaUserSettings.Default.Save();
            }
        }

        public string BootstrappingScript
        {
            get { return SaharaUserSettings.Default.BootstrappingScript; }
            set
            {
                SaharaUserSettings.Default.BootstrappingScript = value;
                SaharaUserSettings.Default.Save();
            }
        }
    }
}
