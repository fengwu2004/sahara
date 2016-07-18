using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace Sahara
{
    public class AvalonEditBehaviour : Behavior<TextEditor>
    {
        public static readonly DependencyProperty AvalonTextProperty =
            DependencyProperty.Register("AvalonText", typeof(string), typeof(AvalonEditBehaviour),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public string AvalonText
        {
            get { return (string)GetValue(AvalonTextProperty); }
            set { SetValue(AvalonTextProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor != null)
            {
                if (textEditor.Document != null)
                    AvalonText = textEditor.Document.Text;
            }
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonEditBehaviour;
            if (behavior.AssociatedObject != null)
            {
                var editor = behavior.AssociatedObject as TextEditor;
                if (editor.Document != null)
                {
                    var caretOffset = editor.CaretOffset;
                    
                    editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue != null ? dependencyPropertyChangedEventArgs.NewValue.ToString() : "";

                    if (caretOffset >= editor.Document.Text.Length)
                    {
                        caretOffset = editor.Document.Text.Length;
                    }

                    if (editor.Document.Text.Length == 0)
                    {
                        caretOffset = 0;
                    }
                    editor.CaretOffset = caretOffset;
                }
            }
        }
    }
}
