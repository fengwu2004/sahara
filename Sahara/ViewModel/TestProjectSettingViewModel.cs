using Autofac;
using GalaSoft.MvvmLight;
using Sahara.Core;
using Sahara.Core.Configuration;
using Sahara.Core.IoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Sahara.ViewModel
{
    public class TestProjectSettingViewModel : ViewModelBase
    {
        private TestProjectManager _testProjectManager;
        private UserSettings settings;

        private TestProject _testProject;
        public TestProject TestProject
        {
            get { return _testProject; }
            set
            {
                _testProject = value;
                _testProjectVariables = new ObservableCollection<Variable>(_testProject.ProjectVariables.Variables);
                //_testProjectEnvPaths = new ObservableCollection<string>(_testProject.Paths);
                RaisePropertyChanged("TestProject");
            }
        }

        public IEnumerable<Device> Devices
        {
            get { return settings.Devices; }
        }

        private ObservableCollection<Variable> _testProjectVariables;
        public ObservableCollection<Variable> TestProjectVariables
        {
            get { return _testProjectVariables; }
            private set
            {
                _testProjectVariables = value;
                RaisePropertyChanged("TestProjectVariables");
            }
        }

        //private ObservableCollection<string> _testProjectEnvPaths;
        //public ObservableCollection<string> TestProjectEnvPaths
        //{
        //    get
        //    {
        //        return _testProjectEnvPaths;
        //    }
        //    private set
        //    {
        //        _testProjectEnvPaths = value;
        //        RaisePropertyChanged("TestProjectEnvPaths");
        //    }
        //}

        private Variable _selectedVariable;
        public Variable SelectedVariable
        {
            get { return _selectedVariable; }
            set
            {
                _selectedVariable = value;
                RaisePropertyChanged("SelectedVariable");
            }
        }

        public string SelectedPath { get; set; }

        public Device SelectedDevice
        {
            get
            {
                if (string.IsNullOrEmpty(this.TestProject.Device))
                {
                    return new Device();
                }
                return this.Devices.FirstOrDefault(dev => dev.Name == this.TestProject.Device);
            }
            set
            {
                if (value == null) return;
                this.TestProject.Device = value.Name;
                RaisePropertyChanged("SelectedDevice");
            }
        }

        public bool AllowDeviceSelection { get; set; }

        private string _query;
        public string Query
        {
            get { return _query; }
            set
            {
                _query = value;
                RaisePropertyChanged("Query");
            }
        }

        public string TestSuitePath
        {
            get { return TestProject.TestSuitePath; }
            set
            {
                TestProject.TestSuitePath = value;
                RaisePropertyChanged("TestSuitePath");
            }
        }

        public TestProjectSettingViewModel()
        {
            _testProjectManager = Container.Current.Resolve<TestProjectManager>();
            settings = UserSettings.GetInstance();

            TestProject = new TestProject();

            SaveCommand = new CommandViewModel("OK", "", OnSave);
            SearchCommand = new CommandViewModel("", "", OnSearch);
            BrowseTestSuiteCommand = new CommandViewModel("...", "", OnBrowseTestSuite);
            ImportCommand = new CommandViewModel("Import", "", OnImportVariableList);
            ExportCommand = new CommandViewModel("Export", "", OnExportVariableList);
            AddCommand = new CommandViewModel("Add", "", () =>
            {
                var dialog = new VariableEditDialog();
                dialog.Save += (sender, newVariable) =>
                {
                    var valueDialog = sender as Window;
                    if (this.TestProjectVariables.Count(v => v.Name == newVariable.Name) > 0)
                    {
                        MessageBox.Show("变量已存在", "错误");
                    }
                    else
                    {
                        this.TestProjectVariables.Add(newVariable);
                        this.SelectedVariable = newVariable;
                        if (valueDialog != null)
                        {
                            valueDialog.Close();
                        }
                    }
                };
                dialog.ShowDialog();
            });

            UpdateCommand = new CommandViewModel("Update", "", () =>
            {
                var dialog = new VariableEditDialog(SelectedVariable);
                dialog.Save += (sender, updatedVariable) =>
                {
                    var valueDialog = sender as Window;
                    this.TestProjectVariables.Remove(SelectedVariable);
                    this.TestProjectVariables.Add(SelectedVariable = updatedVariable);
                    if (valueDialog != null)
                    {
                        valueDialog.Close();
                    }
                };
                dialog.ShowDialog();
            });

            DeleteCommand = new CommandViewModel("", "Images\\delete.png", () =>
            {
                if (SelectedVariable != null)
                {
                    var result = System.Windows.Forms.MessageBox.Show("是否确认删除此变量？", "提示",
                        System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        TestProjectVariables.Remove(SelectedVariable);
                        Persist();
                    }
                }
            });

            //AddPathCommand = new CommandViewModel("", "", OnAddPath);
            //DeletePathCommand = new CommandViewModel("", "", OnDeletePath);
        }

        public CommandViewModel AddCommand { get; private set; }

        public CommandViewModel UpdateCommand { get; private set; }

        public CommandViewModel DeleteCommand { get; private set; }

        public CommandViewModel SaveCommand { get; private set; }

        public CommandViewModel BrowseTestSuiteCommand { get; private set; }

        public CommandViewModel SearchCommand { get; private set; }

        public CommandViewModel ImportCommand { get; private set; }

        public CommandViewModel ExportCommand { get; private set; }

        //public CommandViewModel AddPathCommand { get; private set; }

        //public CommandViewModel DeletePathCommand { get; private set; }

        private void OnSave()
        {
            if (string.IsNullOrEmpty(TestProject.ProjectConfigPath))
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog()
                {
                    Description = "请选择一个文件夹保存测试项目",
                    RootFolder = Environment.SpecialFolder.MyComputer,
                    ShowNewFolderButton = true
                };
                var result = dialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                TestProject.ProjectConfigPath = System.IO.Path.Combine(dialog.SelectedPath, "project-config.xml");
            }
            Persist();
        }

        private void OnBrowseTestSuite()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = "请选择一个包含测试集的文件夹",
                RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = false
            };
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    TestSuitePath = dialog.SelectedPath;
                }
            }
        }

        private string prevQuery;
        private void OnSearch()
        {
            var rgx = new Regex(Query, RegexOptions.IgnoreCase);

            var startIndex = -1;

            // Searching for the same text
            if (SelectedVariable != null && prevQuery == Query)
            {
                startIndex = TestProjectVariables.IndexOf(SelectedVariable);
            }

            var found = TestProjectVariables.FirstOrDefault(v =>
            {
                return (rgx.IsMatch(v.Name) || rgx.IsMatch(v.Description)) &&
                    TestProjectVariables.IndexOf(v) > startIndex;
            });

            prevQuery = Query;

            if (found == null)
            {
                System.Windows.Forms.MessageBox.Show("未找到匹配变量");
                return;
            }

            SelectedVariable = found;
        }

        private void OnImportVariableList()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "XML Files|*.xml",
                Title = "请选择变量列表文件"
            };
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var confirmation = System.Windows.Forms.MessageBox.Show(
                    "导入变量操作将重置所有已定义的变量，请确认。", "提示",
                    System.Windows.Forms.MessageBoxButtons.OKCancel,
                    System.Windows.Forms.MessageBoxIcon.Exclamation);
                if (confirmation == System.Windows.Forms.DialogResult.OK)
                {
                    TestProject.ImportVariables(dialog.FileName);
                    TestProjectVariables = new ObservableCollection<Variable>(TestProject.ProjectVariables.Variables);
                }
            }
        }

        private void OnExportVariableList()
        {
            var dialog = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = "XML Files|*.xml"
            };
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TestProject.ExportVariables(dialog.FileName);
            }
        }

        //private void OnAddPath()
        //{
        //    var dialog = new FolderBrowserDialog()
        //    {
        //        Description = "请选择路径",
        //        SelectedPath = this.TestProject.ProjectRoot,
        //        ShowNewFolderButton = false
        //    };
        //    var result = dialog.ShowDialog();
        //    if (result == DialogResult.OK)
        //    {
        //        if (!string.IsNullOrEmpty(dialog.SelectedPath))
        //        {
        //            if (this.TestProjectEnvPaths.Contains(dialog.SelectedPath))
        //            {
        //                MessageBox.Show("系统路径已存在");
        //                return;
        //            }

        //            this.TestProjectEnvPaths.Add(dialog.SelectedPath);
        //            MessageBox.Show("系统路径已添加");
        //        }
        //    }
        //}

        //private void OnDeletePath()
        //{
        //    if (!string.IsNullOrEmpty(SelectedPath))
        //    {
        //        this.TestProjectEnvPaths.Remove(SelectedPath);
        //        MessageBox.Show("系统路径已删除");
        //    }
        //}

        private void Persist()
        {
            try
            {
                TestProject.ProjectVariables.Variables = TestProjectVariables.ToList();
                //TestProject.Paths = TestProjectEnvPaths.ToList();
                _testProjectManager.Save(TestProject);
                _testProjectManager.Current = TestProject;
                MessageBox.Show("项目设置保存成功", "提示");
            }
            catch (Exception e)
            {
                MessageBox.Show("项目设置保存失败，错误详情：\n" + e.Message, "错误");
            }
        }
    }
}
