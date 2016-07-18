using Autofac;
using GalaSoft.MvvmLight;
using Sahara.Core;
using Sahara.Core.Utils;
using Sahara.Core.Configuration;
using Sahara.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Container = Sahara.Core.IoC.Container;

namespace Sahara.ViewModel
{
    internal sealed class TestProgressReport
    {
        public TestScriptViewModel Node { get; set; }
        public AggregateTestResult Result { get; set; }
    }

    public class MainViewModel : ViewModelBase
    {
        private BackgroundWorker worker;
        private TestProjectManager _testProjectManager;
        private UserSettings settings;
        private ViewModelLocator viewModelLocator;
        private FileSystemTestSuiteManager _testSuiteManager;
        private TestScriptViewModel currentScript;

        public TestProject TestProject
        {
            get { return this._testProjectManager.Current; }
            set
            {
                this._testProjectManager.Current = value;
                this.RaisePropertyChanged("TestProject");
            }
        }

        /// <summary>
        /// Version number is auto-generated according to these rules:
        /// https://msdn.microsoft.com/en-us/library/system.reflection.assemblyversionattribute.aspx
        /// </summary>
        public string Version
        {
            get
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                AssemblyName asmName = asm.GetName();        
                Version ver = asmName.Version;
                return "v" + ver.ToString();
            }
        }

        public DateTime BuildDateTime
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetLinkerTime();
            }
        }

        #region Treeview models
        private ObservableCollection<ITreeViewNode> _targets;
        public ObservableCollection<ITreeViewNode> Targets
        {
            get { return this._targets; }
            set
            {
                this._targets = value;
                this.RaisePropertyChanged("Targets");
            }
        }

        private TestScriptViewModel _selectedTestScript;
        public TestScriptViewModel SelectedTestScript
        {
            get { return this._selectedTestScript; }
            set
            {
                this._selectedTestScript = value;
                if (this._contentViewModel != null)
                {
                    this._contentViewModel.CurrentTestScript = value;
                }
                RaisePropertyChanged("SelectedTestScript");
            }
        }

        private ITreeViewNode _selectedNode;
        public ITreeViewNode SelectedNode
        {
            get { return this._selectedNode; }
            set
            {
                this._selectedNode = value;
                RaisePropertyChanged("SelectedNode");
            }
        }
        #endregion

        private IFormatter formatter = new BinaryFormatter();

        public PreferencePaneViewModel PreferencePaneViewModel { get; private set; }

        private BaseResultViewModel _contentViewModel;
        public BaseResultViewModel ContentViewModel
        {
            get
            {
                return _contentViewModel;
            }
            set
            {
                _contentViewModel = value;
                RaisePropertyChanged("ContentViewModel");
            }
        }

        public string WindowTitle
        {
            get { return "Sahara"; }
        }

        public List<Encoding> SupportedEncodings
        {
            get
            {
                return new List<Encoding>() { Encoding.UTF8, Encoding.GetEncoding("gbk") };
            }
        }

        public Encoding SelectedEncoding
        {
            get
            {
                return settings.Encoding;
            }
            set
            {
                settings.Encoding = value;
                this.ContentViewModel.CurrentTestScript = this.SelectedTestScript;
            }
        }

        public MainViewModel()
        {
            this.settings = UserSettings.GetInstance();
            this._testProjectManager = Container.Current.Resolve<TestProjectManager>();
            this._testSuiteManager = new FileSystemTestSuiteManager();
            this.viewModelLocator = Container.Current.Resolve<ViewModelLocator>();
            this.ContentViewModel = GetResultViewModel(settings.Mode);
            this.PreferencePaneViewModel = this.viewModelLocator.PreferencePane;

            this.CreateProjectCommand = new CommandViewModel("New Project", "Images\\new.png", () =>
            {
                var dialog = new TestProjectSettingsDialog();
                dialog.Owner = Application.Current.MainWindow;
                var result = dialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    this.OpenTestProject(this._testProjectManager.Current.ProjectConfigPath);
                }
            });

            this.OpenProjectCommand = new CommandViewModel("Open Project", "Images\\folder.png", this.OnOpenTestProject);
            this.CloseProjectCommand = new CommandViewModel("Close Project", "Images\\delete.png", this.OnCloseTestProject);
            this.RunSelectedCommand = new CommandViewModel("Run Selected", "Images\\run.png", this.OnRunSelected);
            this.RunAllCommand = new CommandViewModel("Run All", "Images\\run.png", this.OnRunAll);
            this.SettingCommand = new CommandViewModel("Settings", "Images\\setting.png", () =>
            {
                if (this.TestProject == null)
                {
                    System.Windows.MessageBox.Show("请打开一个测试项目", "提示");
                    return;
                }

                var wnd = new TestProjectSettingsDialog(this.TestProject);
                wnd.Owner = Application.Current.MainWindow;
                wnd.ShowDialog();

            });

            this.SaveCommand = new CommandViewModel("Save", "Images\\save.png", () =>
            {
                if (SelectedTestScript != null)
                {
                    SelectedTestScript.Save();
                }
            });

            this.StopCommand = new CommandViewModel("Stop", "Images\\stop.png", this.OnStop);
            this.SwitchToSimpleCommand = new CommandViewModel("Report", "Images\\report.png", () =>
            {
                this.ToggleMode(ResultViewMode.Simple);
            });
            this.SwitchToAdvancedCommand = new CommandViewModel("Debug", "Images\\debug.png", () =>
            {
                this.ToggleMode(ResultViewMode.Advanced);
            });

            this.ExitCommand = new CommandViewModel("Exit", "Images\\exit.png", Application.Current.Shutdown);

            this.CreateFolderCommand = new CommandViewModel("", "Images\\add_folder.png", () =>
            {
                var context = this.GetContext(this.SelectedNode);

                var directory = Path.Combine(this.TestProject.ProjectRoot, "TestSuite");

                if (this.SelectedNode != null)
                {
                    directory = this.SelectedNode is TestScriptGroupViewModel ?
                        (this.SelectedNode as TestScriptGroupViewModel).DataItem.FilePath :
                        Path.GetDirectoryName((this.SelectedNode as TestScriptViewModel).TestScript.FilePath);
                }

                var dialog = new FileNameDialog("未命名", true, true);
                if (dialog.ShowDialog() == true)
                {
                    var filePath = Path.Combine(directory, dialog.Path);
                    var testScriptGroup = new TestScriptGroup(filePath);
                    var vm = new TestScriptGroupViewModel(testScriptGroup);

                    var result = this._testSuiteManager.CreateTestScriptGroup(testScriptGroup);
                    if (result == null)
                    {
                        MessageBox.Show("该测试分组已存在", "错误");
                        return;
                    }
                    context.Add(vm);
                }

                this.SaveIndex();
            });

            this.CreateFileCommand = new CommandViewModel("", "Images\\add_file.png", () =>
            {
                var context = this.GetContext(this.SelectedNode);

                var directory = Path.Combine(this.TestProject.ProjectRoot, "TestSuite");

                if (this.SelectedNode != null)
                {
                    directory = this.SelectedNode is TestScriptGroupViewModel ?
                        (this.SelectedNode as TestScriptGroupViewModel).DataItem.FilePath :
                        Path.GetDirectoryName((this.SelectedNode as TestScriptViewModel).TestScript.FilePath);
                }

                var dialog = new FileNameDialog("未命名", true);
                if (dialog.ShowDialog() == true)
                {
                    var filePath = Path.Combine(directory, dialog.Path + ".py");
                    var testScript = new PythonTestScript(dialog.Path, "", filePath);
                    var vm = new TestScriptViewModel(testScript);

                    var result = this._testSuiteManager.CreateTestScript(testScript);
                    if (!result)
                    {
                        MessageBox.Show("该测试脚本已存在", "错误");
                        return;
                    }
                    context.Add(vm);
                }

                this.SaveIndex();
            });

            this.DeleteCommand = new CommandViewModel("", "Images\\delete.png", () =>
            {
                var warningMessage = "";
                if (SelectedNode != null)
                {
                    warningMessage = SelectedNode is TestScriptGroupViewModel
                        ? "确认删除此测试集？此操作将级联删除该测试集中的测试脚本。"
                        : "确认删除此测试脚本？";

                    var confirmation = System.Windows.Forms.MessageBox.Show(warningMessage, "提示",
                    System.Windows.Forms.MessageBoxButtons.OKCancel,
                    System.Windows.Forms.MessageBoxIcon.Exclamation);

                    if (confirmation == System.Windows.Forms.DialogResult.OK)
                    {
                        if (SelectedNode is TestScriptGroupViewModel)
                        {
                            var dataItem = SelectedNode as TestScriptGroupViewModel;
                            this._testSuiteManager.Delete(dataItem.DataItem);
                        }
                        else
                        {
                            var dataItem = SelectedNode as TestScriptViewModel;
                            this._testSuiteManager.Delete(dataItem.TestScript);
                        }

                        var context = this.FindParent(this.Targets, SelectedNode);
                        if (context != null)
                        {
                            context.Remove(SelectedNode);
                            SelectedNode = null;
                        }
                    }
                    this.SaveIndex();
                }
            });

            this.MoveUpCommand = new CommandViewModel("", "Images\\up.png", () =>
            {
                if (SelectedNode != null)
                {
                    var parent = this.FindParent(this.Targets, SelectedNode);
                    if (parent != null) // Silent fail is sub-optimal
                    {
                        var index = parent.IndexOf(SelectedNode);
                        if (index > 0)
                        {
                            parent.Move(index, index - 1);
                        }
                    }
                    this.SaveIndex();
                }
            });

            this.MoveDownCommand = new CommandViewModel("", "Images\\down.png", () =>
            {
                if (SelectedNode != null)
                {
                    var parent = this.FindParent(this.Targets, SelectedNode);
                    if (parent != null) // Silent fail is sub-optimal
                    {
                        var index = parent.IndexOf(SelectedNode);
                        if (index < parent.Count - 1)
                        {
                            parent.Move(index, index + 1);
                        }
                    }
                    this.SaveIndex();
                }
            });

            this.RenameCommand = new CommandViewModel("", "", () =>
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode is TestScriptViewModel)
                    {
                        var scriptViewModel = SelectedNode as TestScriptViewModel;
                        var originalPath = scriptViewModel.TestScript.FilePath;
                        var directory = Path.GetDirectoryName(originalPath);
                        var extension = Path.GetExtension(originalPath);

                        var dialog = new FileNameDialog(Path.GetFileNameWithoutExtension(originalPath));
                        if (dialog.ShowDialog() == true)
                        {
                            var newFilePath = Path.Combine(directory, dialog.Path + extension);
                            var reader = new TestScriptReader();
                            System.IO.File.Move(originalPath, newFilePath);

                            var context = this.FindParent(this.Targets, SelectedNode);
                            var index = context.IndexOf(SelectedNode);
                            context[index] = new TestScriptViewModel(reader.Read(newFilePath));
                        }
                    }
                    else
                    {
                        var scriptGroupViewModel = SelectedNode as TestScriptGroupViewModel;
                        var originalPath = scriptGroupViewModel.DataItem.FilePath;
                        var directory = Path.GetDirectoryName(originalPath);

                        var dialog = new FileNameDialog(Path.GetFileName(originalPath));
                        if (dialog.ShowDialog() == true)
                        {
                            var newDirPath = Path.Combine(directory, dialog.Path);
                            var reader = new FileSystemTestSuiteManager();
                            Directory.Move(originalPath, newDirPath);

                            reader.TestSuiteRoot = newDirPath;
                            var context = this.FindParent(this.Targets, SelectedNode);
                            var index = context.IndexOf(SelectedNode);
                            context[index] = new TestScriptGroupViewModel(reader.Read() as TestScriptGroup);
                        }
                    }
                    this.SaveIndex();
                }
            });
        }

        public bool IsSimpleMode { get { return settings.Mode == ResultViewMode.Simple; } }

        public bool IsAdvancedMode { get { return settings.Mode == ResultViewMode.Advanced; } }

        private void ToggleMode(ResultViewMode mode)
        {
            if (settings.Mode == mode) { return; }
            settings.Mode = mode;

            //var selectedTestScript = this.ContentViewModel.SelectedTestScript;
            //var targets = this.ContentViewModel.Targets;
            this.ContentViewModel = GetResultViewModel(settings.Mode);
            this.ContentViewModel.CurrentTestScript = this.SelectedTestScript;
            //this.ContentViewModel.Targets = targets;

            //var selectedNode = FindTestScript(this.ContentViewModel.Targets,
            //    selectedTestScript.Header);
            //this.ContentViewModel.SelectedNode = selectedNode;

            //RaisePropertyChanged("IsSimpleMode");
            //RaisePropertyChanged("IsAdvancedMode");
        }

        private BaseResultViewModel GetResultViewModel(ResultViewMode mode)
        {
            BaseResultViewModel resultViewModel = null;

            if (settings.Mode == ResultViewMode.Simple)
            {
                resultViewModel = new SimpleResultViewModel();
            }
            else
            {
                resultViewModel = new AdvancedResultViewModel();
            }

            RaisePropertyChanged("IsSimpleMode");
            RaisePropertyChanged("IsAdvancedMode");

            return resultViewModel;
        }

        public CommandViewModel SwitchToSimpleCommand { get; private set; }

        public CommandViewModel SwitchToAdvancedCommand { get; private set; }

        public CommandViewModel SaveCommand { get; private set; }

        public CommandViewModel RunAllCommand { get; private set; }

        public CommandViewModel RunSelectedCommand { get; private set; }

        public CommandViewModel CreateProjectCommand { get; private set; }

        public CommandViewModel OpenProjectCommand { get; private set; }

        public CommandViewModel CloseProjectCommand { get; set; }

        public CommandViewModel SettingCommand { get; private set; }

        public CommandViewModel StopCommand { get; private set; }

        public CommandViewModel ExitCommand { get; private set; }

        public bool IsProjectOpened
        {
            get
            {
                return this.TestProject != null;
            }
        }

        #region Treeview commands
        public CommandViewModel CreateFolderCommand { get; set; }

        public CommandViewModel CreateFileCommand { get; set; }

        public CommandViewModel DeleteCommand { get; set; }

        public CommandViewModel MoveUpCommand { get; set; }

        public CommandViewModel MoveDownCommand { get; set; }

        public CommandViewModel RenameCommand { get; set; }
        #endregion

        public TestProject OpenTestProject(string path)
        {
            this.TestProject = this._testProjectManager.LoadFromXml(path);
            if (this.TestProject != null)
            {
                var testSuite = this.TestProject.TestSuite as TestScriptGroup;
                this.SelectedTestScript = new TestScriptViewModel();

                IEnumerable<TestResultState> resultStates = null;
                if (File.Exists(this.TestProject.ResultStatePath))
                {
                    try
                    {
                        using (var stream = new FileStream(this.TestProject.ResultStatePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            resultStates = formatter.Deserialize(stream) as IEnumerable<TestResultState>;
                        }
                    }
                    catch (Exception)
                    {
                        // Exception when loading result status shouldn't break the application
                    }
                }

                var vms = testSuite.Nodes.Select(node =>
                {
                    if (node is TestScriptGroup)
                    {
                        return new TestScriptGroupViewModel(node as TestScriptGroup);
                    }
                    else
                    {
                        var testScriptVM = new TestScriptViewModel(node as BaseTestScript);

                        if (resultStates != null)
                        {
                            var prevState = resultStates.FirstOrDefault(s => s.IsBelongTo(testScriptVM));
                            if (prevState != null)
                            {
                                testScriptVM.RestoreResultState(prevState);
                            }
                        }
                        return (ITreeViewNode)(testScriptVM);
                    }
                });
                this.Targets = new ObservableCollection<ITreeViewNode>(vms);
                Properties.Settings.Default.LastOpenedProject = path;
            }
            RaisePropertyChanged("IsProjectOpened");
            return this.TestProject;
        }

        private IEnumerable<TestResultState> GetResultStates(IEnumerable<ITreeViewNode> collection)
        {
            var states = new List<TestResultState>();

            foreach (var item in collection)
            {
                if (item is TestScriptGroupViewModel)
                {
                    states.AddRange(GetResultStates(((TestScriptGroupViewModel)item).Children));
                }
                else
                {
                    states.Add(((TestScriptViewModel)item).Serialize());
                }
            }
            return states;
        }

        private void RunTestScript(TestScriptViewModel testScript, BackgroundWorker worker)
        {
            if (worker.CancellationPending) return;

            this.currentScript = testScript;
            testScript.Execute();

            (worker as BackgroundWorker).ReportProgress(0,
                new TestProgressReport() { Node = testScript, Result = testScript.TestResults });
        }

        private void RunTestSuite(ITreeViewNode node, BackgroundWorker worker)
        {
            if (node is TestScriptGroupViewModel)
            {
                var group = node as TestScriptGroupViewModel;
                foreach (var child in group.Children)
                {
                    RunTestSuite(child, worker);
                }
            }
            else
            {
                var script = node as TestScriptViewModel;
                RunTestScript(script, worker);
            }
        }

        private void OnRunAll()
        {
            this.worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.DoWork += (s, e) =>
            {
                foreach (var group in Targets)
                {
                    this.RunTestSuite(group, worker);
                }
            };

            worker.ProgressChanged += (s, e) =>
            {
                var report = e.UserState as TestProgressReport;
                report.Node.Status = report.Result.Status;
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                this.SaveRunStates();
            };

            worker.RunWorkerAsync();
        }

        private void OnRunSelected()
        {
            this.worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            var selectedTestScripts = this.FindSelectedTestScripts(this.Targets);

            if (selectedTestScripts != null && selectedTestScripts.Count() > 0)
            {
                this.SelectedTestScript = selectedTestScripts.ElementAt(0);

                worker.DoWork += (s, e) =>
                {
                    foreach (var script in selectedTestScripts)
                    {
                        this.RunTestScript(script, worker);
                    }
                };

                worker.ProgressChanged += (s, e) =>
                {
                    var report = e.UserState as TestProgressReport;
                    report.Node.Status = report.Result.Status;
                };

                worker.RunWorkerCompleted += (s, e) =>
                {
                    this.SaveRunStates();
                };

                if (!worker.IsBusy)
                {
                    worker.RunWorkerAsync();
                }
            }
        }

        private void OnOpenTestProject()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "XML Files|*.xml",
                Title = "请选择项目配置文件"
            };
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.OpenTestProject(dialog.FileName);
            }
        }

        private void OnCloseTestProject()
        {
            // Notify unsaved change
            var result = MessageBox.Show("确认关闭项目？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                this.SelectedNode = null;
                this.SelectedTestScript = null;
                this.TestProject = null;
                this.Targets = null;
                Properties.Settings.Default.LastOpenedProject = null;
                RaisePropertyChanged("IsProjectOpened");
            }
        }

        private void OnStop()
        {
            if (this.currentScript != null)
            {
                this.currentScript.Abort();
            }

            if (this.worker.WorkerSupportsCancellation &&
                !this.worker.CancellationPending)
            {
                this.worker.CancelAsync();
            }
        }

        private void SaveRunStates()
        {
            var resultStates = GetResultStates(this.Targets);
            var stream = new FileStream(this.TestProject.ResultStatePath, FileMode.Create, FileAccess.Write, FileShare.None);
            try
            {
                formatter.Serialize(stream, resultStates);
            }
            catch (Exception)
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }

                File.Delete(this.TestProject.ResultStatePath);
            }
        }

        #region Utilities
        private IEnumerable<TestScriptViewModel> FindSelectedTestScripts(TestScriptGroupViewModel group)
        {
            return this.FindSelectedTestScripts(group.Children);
        }

        private IEnumerable<TestScriptViewModel> FindSelectedTestScripts(IEnumerable<ITreeViewNode> nodes)
        {
            var selectedTestScript = new List<TestScriptViewModel>();

            foreach (var child in nodes)
            {
                if (child is TestScriptViewModel)
                {
                    if (child.IsSelected)
                    {
                        selectedTestScript.Add(child as TestScriptViewModel);
                    }
                }
                else
                {
                    if (child.IsSelected)
                    {
                        selectedTestScript.AddRange(FindSelectedTestScripts(child as TestScriptGroupViewModel));
                    }
                }
            }

            return selectedTestScript;
        }

        private TestScriptViewModel FindTestScript(IEnumerable<ITreeViewNode> nodes, string header)
        {
            foreach (var child in nodes)
            {
                if (child is TestScriptViewModel)
                {
                    if (child.Header == header)
                    {
                        return child as TestScriptViewModel;
                    }
                }
                else
                {
                    var group = child as TestScriptGroupViewModel;
                    FindTestScript(group.Children, header);
                }
            }

            return null;
        }

        public ObservableCollection<ITreeViewNode> FindParent(ObservableCollection<ITreeViewNode> context, ITreeViewNode node)
        {
            if (context.FirstOrDefault(n => n == node) != null)
            {
                return context as ObservableCollection<ITreeViewNode>;
            }

            foreach (var item in context)
            {
                if (item is TestScriptGroupViewModel)
                {
                    var subContext = item as TestScriptGroupViewModel;
                    var result = FindParent(subContext.Children, node);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null; // Not found
        }

        private ObservableCollection<ITreeViewNode> GetContext(ITreeViewNode node)
        {
            if (node == null)
            {
                return this.Targets;
            }
            else if (node is TestScriptGroupViewModel)
            {
                var vm = node as TestScriptGroupViewModel;
                return vm.Children;
            }
            else
            {
                return this.FindParent(this.Targets, node);
            }
        }

        private void WriteIndexFile(ObservableCollection<ITreeViewNode> collection, string path)
        {
            var sb = new StringBuilder();
            foreach (var item in collection)
            {
                if (item is TestScriptGroupViewModel)
                {
                    var data = item as TestScriptGroupViewModel;
                    WriteIndexFile(data.Children, data.DataItem.FilePath);
                }
                sb.AppendLine(item.Header);
            }
            using (var writer = new StreamWriter(File.OpenWrite(Path.Combine(path, ".saharaIndex"))))
            {
                writer.Write(sb.ToString());
            }
        }

        private void SaveIndex()
        {
            this.WriteIndexFile(this.Targets, Path.Combine(this._testProjectManager.Current.ProjectRoot, "TestSuite"));
        }
        #endregion

    }
}
