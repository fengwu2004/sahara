using GalaSoft.MvvmLight;
using Sahara.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.ViewModel
{
    public interface ITreeViewNode
    {
        string Header { get; }
        bool IsExpanded { get; }
        string Content { get; set; }
        bool IsSelected { get; set; }
    }

    [Serializable]
    public class TestScriptGroupViewModel : ViewModelBase, ITreeViewNode
    {
        private TestScriptGroup scriptGroup;
        private ObservableCollection<ITreeViewNode> children;
        private bool isExpanded;

        public TestScriptGroupViewModel(TestScriptGroup scriptGroup, bool isExpanded = false)
        {
            this.children = new ObservableCollection<ITreeViewNode>();
            this.scriptGroup = scriptGroup;
            this.isExpanded = isExpanded;

            foreach (var item in scriptGroup.Nodes)
            {
                if (item is TestScriptGroup)
                {
                    var testScriptGroup = item as TestScriptGroup;
                    this.children.Add(new TestScriptGroupViewModel(testScriptGroup));
                }

                if (item is BaseTestScript)
                {
                    var testScript = item as BaseTestScript;
                    this.children.Add(new TestScriptViewModel(testScript));
                }
            }
        }

        public int NumOfScripts
        {
            get { return this.scriptGroup.Nodes.Count(node => node is BaseTestScript); }
        }

        public ObservableCollection<ITreeViewNode> Children
        {
            get { return this.children; }
        }

        public string Header
        {
            get { return this.scriptGroup.Name; }
        }

        public TestScriptGroup DataItem
        {
            get { return this.scriptGroup; }
        }

        public bool IsExpanded
        {
            get { return this.isExpanded; }
        }

        public string Content
        {
            get { return ""; }
            set { }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return this._isSelected; }
            set
            {
                this._isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
    }
}
