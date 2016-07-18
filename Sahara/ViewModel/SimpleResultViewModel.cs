using Autofac;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Scripting;
using Sahara.Core;

namespace Sahara.ViewModel
{
    public class SimpleResultViewModel : BaseResultViewModel
    {
        public SimpleResultViewModel()
            : base()
        {
            this.PropertyChanged += Refresh;
        }

        private void Refresh(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentTestScript")
            {
                if (this.CurrentTestScript == null)
                {
                    this.Author = "";
                    this.Title = "";
                    this.Version = "";
                    this.Description = "";
                    return;
                }

                this.Author = this.ParseTag("author", this.CurrentTestScript.Content);
                this.Title = this.ParseTag("title", this.CurrentTestScript.Content);
                this.Version = this.ParseTag("version", this.CurrentTestScript.Content);
                var desc = this.ParseBlockTag("description", this.CurrentTestScript.Content);
                this.Description = desc.Replace("# ", "").Replace("#", "");
            }
        }

        private string _description;
        public string Description
        {
            get { return this._description; }
            set
            {
                this._description = value;
                RaisePropertyChanged("Description");
            }
        }

        private string _title;
        public string Title
        {
            get { return this._title; }
            set
            {
                this._title = value;
                RaisePropertyChanged("Title");
            }
        }

        private string _version;
        public string Version
        {
            get { return this._version; }
            set
            {
                this._version = value;
                RaisePropertyChanged("Version");
            }
        }

        private string _author;
        public string Author
        {
            get { return this._author; }
            set
            {
                this._author = value;
                RaisePropertyChanged("Author");
                RaisePropertyChanged("HasAuthor");
            }
        }

        public bool HasAuthor
        {
            get { return !string.IsNullOrEmpty(this._author); }
        }

        private string ParseTag(string tag, string content)
        {
            var regex = new Regex("@" + tag + "\\s(.+)\\r?\\n");
            var match = regex.Match(content);
            if (match.Groups.Count > 1)
            {
                return match.Groups[1].Value.Trim('\r', '\n');
            }
            return "";
        }

        private string ParseBlockTag(string tag, string content)
        {

            var start = content.IndexOf("@" + tag) + ("@" + tag).Length;
            if (start < ("@" + tag).Length)
            {
                return "";
            }
            var end = content.Substring(start).IndexOf("@end") + start;
            return content.Substring(start, end - start);
        }
    }
}
