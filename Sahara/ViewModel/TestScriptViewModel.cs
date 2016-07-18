using System;
using Autofac;
using GalaSoft.MvvmLight;
using Sahara.Core;
using Sahara.Core.IoC;
using System.IO;
using Sahara.Core.Utils;
using System.Windows;
using Sahara.Core.Configuration;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Sahara.ViewModel
{
    [Serializable]
    public sealed class TestLog
    {
        public string Status { get; set; }
        public BaseTestScript TestScript { get; set; }
        public TestProject TestProject { get; set; }
        public AggregateTestResult TestResults { get; set; }
        public Device Device { get; set; }
    }

    [Serializable]
    public sealed class TestResultState
    {
        public TestResultState(TestScriptViewModel vm)
        {
            this.Hash = HashGen.GenerateMd5(vm.Header + vm.Content);
            this.TestResults = vm.TestResults;
        }

        public bool IsBelongTo(TestScriptViewModel vm)
        {
            return this.Hash == HashGen.GenerateMd5(vm.Header + vm.Content);
        }

        public string Hash { get; set; }
        public AggregateTestResult TestResults { get; set; }
    }

    [Serializable]
    public class TestScriptViewModel : ViewModelBase, ITreeViewNode
    {
        private BaseTestContext context;
        private TemplateEngine _templateEngine;
        private UserSettings settings;

        public TestScriptViewModel()
        {
            _status = TestResultStatus.Inconclusive; // TestResultStatusResolver.Resolve(script.Name)
            this.IsChanged = false;
            this.settings = UserSettings.GetInstance();
        }

        public TestScriptViewModel(BaseTestScript script)
            : this()
        {
            _templateEngine = Container.Current.Resolve<TemplateEngine>();
            _testScript = script;

            this.TestProject = Container.Current.Resolve<TestProjectManager>().Current;
        }

        public TestProject TestProject { get; private set; }

        public string Header
        {
            get
            {
                if (_testScript == null)
                {
                    return string.Empty;
                }
                return _testScript.Name;
            }
        }

        public string Content
        {
            get
            {
                if (_testScript == null)
                {
                    return string.Empty;
                }

                if (settings.Encoding != null && settings.Encoding != Encoding.UTF8)
                {
                    var bytes = Encoding.UTF8.GetBytes(_testScript.Content);
                    return settings.Encoding.GetString(bytes);
                }

                return _testScript.Content;
            }
            set
            {
                _testScript.Content = value;
                IsChanged = true;
            }
        }

        public string Message
        {
            get
            {
                if (this.TestResults == null) return "";

                var results = this.TestResults.Results;

                if (results.Count(r => r.GetType() == typeof(SyntaxExceptionTestResult) ||
                    r.GetType() == typeof(ErrorTestResult)) > 0)
                {
                    var error = results.First(r => r.GetType() == typeof(SyntaxExceptionTestResult) ||
                        r.GetType() == typeof(ErrorTestResult));
                    return string.Format("测试脚本错误，请联系测试开发人员，错误信息：{0}", error.Message);
                }

                if (results.Count(r => r.GetType() == typeof(ExceptionTestResult)) > 0)
                {
                    var error = results.First(r => r.GetType() == typeof(ExceptionTestResult));
                    return string.Format("系统内部错误，请联系开发人员，异常信息：{0}", error.Message);
                }

                return "";
            }
        }

        private BaseTestScript _testScript;
        public BaseTestScript TestScript
        {
            get { return _testScript; }
        }

        private TestResultStatus _status;
        public TestResultStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        private AggregateTestResult _testResults;
        public AggregateTestResult TestResults
        {
            get { return this._testResults; }
            set
            {
                this._testResults = value;
                RaisePropertyChanged("TestResults");
                RaisePropertyChanged("Message");
            }
        }

        public bool IsExpanded
        {
            get { return false; }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                this._isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        private bool _isChanged;
        public bool IsChanged
        {
            get { return _isChanged; }
            private set
            {
                _isChanged = value;
                RaisePropertyChanged("IsChanged");
                RaisePropertyChanged("Indicator");
            }
        }

        public string Indicator
        {
            get
            {
                return this._isChanged ? "*" : "";
            }
        }

        public void Execute()
        {
            this.Save();
            var results = new List<BaseTestResult>();

            var pyContext = new PythonTestContext(_testScript.Name, _testScript.Content, TestProject);
            pyContext.OnMessage += (sender, arg) =>
            {
                results.Add(arg.Result);
                TestResults = new AggregateTestResult(context.Name, results, TestResultStatus.Inconclusive);
            };

            context = pyContext;

            TestResults = context.Execute();
            this.WriteTestLog();
        }

        /// <summary>
        /// Save test script changes
        /// </summary>
        public void Save()
        {
            if (this.TestScript == null) return;
            this.TestScript.Save();
            this.IsChanged = false;
        }

        public void Abort()
        {
            if (this.context != null)
            {
                this.context.Abort();
            }
        }

        /// <summary>
        /// Write test logs
        /// </summary>
        private void WriteTestLog()
        {
            var fileName = string.Format("{0}_{1}.txt", TestScript.Name, DateTime.Now.ToString("yyyy.dd.MM-HH.mm.ss"));
            var testResultsDir = Path.Combine(TestProject.ProjectRoot, "TestResults");
            var filePath = Path.Combine(TestProject.ProjectRoot, "TestResults", fileName);

            if (!Directory.Exists(testResultsDir))
            {
                Directory.CreateDirectory(testResultsDir);
            }

            var logItem = new TestLog()
            {
                Device = new Device(),
                Status = this.Status.Value,
                TestScript = this.TestScript,
                TestProject = this.TestProject,
                TestResults = this.TestResults
            };

            if (!string.IsNullOrEmpty(TestProject.Device) && 
                settings.Devices.Count(dev => dev.Name == this.TestProject.Device) > 0)
            {
                logItem.Device = settings.Devices.First(dev => dev.Name == TestProject.Device);
            }

            try
            {
                var log = this._templateEngine.Render(logItem, this.TestProject.RawResultTemplate);
                using (var writer = new StreamWriter(filePath))
                {
                    writer.Write(log);
                }
            }
            catch (TemplateException ex)
            {
                var errorMessage = string.Format("请检查日志模板格式\n\n错误详情：{0}\n", ex.InnerException.Message);
                MessageBox.Show(errorMessage, "错误");
            }
        }

        public void RestoreResultState(TestResultState state)
        {
            this.TestResults = state.TestResults;
            if (this.TestResults != null)
            {
                this.Status = this.TestResults.Status;
            }
        }

        public TestResultState Serialize()
        {
            return new TestResultState(this);
        }
    }
}
