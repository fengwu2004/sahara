using Autofac;
using GalaSoft.MvvmLight;
using Sahara.Core;

namespace Sahara.ViewModel
{
    public abstract class BaseResultViewModel : ViewModelBase
    {
        private TestProjectManager _testProjectManager;

        public BaseResultViewModel()
        {
            this._testProjectManager = Core.IoC.Container.Current.Resolve<TestProjectManager>();
            this.CurrentTestScript = new TestScriptViewModel();
        }

        private TestScriptViewModel _currentTestScript;
        public TestScriptViewModel CurrentTestScript
        {
            get { return this._currentTestScript; }
            set
            {
                this._currentTestScript = value;
                RaisePropertyChanged("CurrentTestScript");
            }
        }
    }
}
