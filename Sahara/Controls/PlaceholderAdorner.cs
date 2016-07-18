using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Globalization;

namespace Sahara.Controls
{
    public class PlaceholderAdorner : Adorner
    {
        private VisualCollection _visCollec;
        private TextBlock _tb;
        private TextBox _txt;

        protected override int VisualChildrenCount
        {
            get
            {
                return _visCollec.Count;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _tb.Arrange(new Rect(new Point(8, 5), finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visCollec[index];
        }

        public PlaceholderAdorner(UIElement ele, string placeholder) : base(ele)
        {
            _txt = ele as TextBox;
            if (_txt == null) return;

            Binding bd = new Binding("IsVisible");
            bd.Source = _txt;
            bd.Mode = BindingMode.OneWay;
            bd.Converter = new BoolToVisibilityConverter();
            this.SetBinding(TextBox.VisibilityProperty, bd);
            _visCollec = new VisualCollection(this);
            _tb = new TextBlock();
            _tb.Style = null;
            _tb.FontSize = _txt.FontSize;
            _tb.FontFamily = _txt.FontFamily;
            _tb.FontWeight = _txt.FontWeight;
            _tb.FontStretch = _txt.FontStretch;
            _tb.FontStyle = FontStyles.Normal;
            _tb.Foreground = Brushes.Gray;
            _tb.Text = placeholder;
            _tb.IsHitTestVisible = false;
            _visCollec.Add(_tb);
        }
    }
}
