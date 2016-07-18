using Autofac;
using GalaSoft.MvvmLight;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Sahara.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sahara.ViewModel
{
    public class AdvancedResultViewModel : BaseResultViewModel
    {
        public AdvancedResultViewModel()
            : base()
        {
            using (var stream = new FileStream("Data/python.syntax.highlighting.xml", FileMode.Open))
            using (var reader = XmlReader.Create(stream))
            {
                this.SyntaxHighlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        public IHighlightingDefinition SyntaxHighlightingDefinition { get; private set; }
    }
}
