using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Sahara.Controls
{
    public class TextBoxHelper
    {
        public static string GetPlaceholder(DependencyObject obj)
        {
            return (string)obj.GetValue(PlaceholderProperty);
        }

        public static void SetPlaceholder(DependencyObject obj, string value)
        {
            obj.SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(TextBoxHelper),
                new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnPlaceholderChanged)));

        public static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox txt = d as TextBox;
            if (txt == null || e.NewValue.ToString().Trim().Length == 0) return;

            RoutedEventHandler loadHandler = null;
            loadHandler = (s1, e1) =>
            {
                txt.Loaded -= loadHandler;

                var lay = AdornerLayer.GetAdornerLayer(txt);
                if (lay == null) return;

                Adorner[] ar = lay.GetAdorners(txt);
                if (ar != null)
                {
                    for (int i = 0; i < ar.Length; i++)
                    {
                        if (ar[i] is PlaceholderAdorner)
                        {
                            lay.Remove(ar[i]);
                        }
                    }
                }

                if (txt.Text.Length == 0)
                    lay.Add(new PlaceholderAdorner(txt, e.NewValue.ToString()));

            };
            txt.Loaded += loadHandler;
            txt.TextChanged += (s1, e1) =>
            {
                bool isShow = txt.Text.Length == 0;

                var lay = AdornerLayer.GetAdornerLayer(txt);
                if (lay == null) return;

                if (isShow)
                {
                    lay.Add(new PlaceholderAdorner(txt, e.NewValue.ToString()));
                }
                else
                {
                    Adorner[] ar = lay.GetAdorners(txt);
                    if (ar != null)
                    {
                        for (int i = 0; i < ar.Length; i++)
                        {
                            if (ar[i] is PlaceholderAdorner)
                            {
                                lay.Remove(ar[i]);
                            }
                        }
                    }
                }
            };
        }
    }
}
