using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlockProgramming
{
    /// <summary>
    /// FunctionBlock.xaml の相互作用ロジック
    /// </summary>
    public partial class FunctionBlock : UserControl
    {
        public FunctionBlock()
        {
            InitializeComponent();
        }

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            var arg = (sender as FrameworkElement)?.DataContext as FuncArg;
            arg?.ValueChangeSequenceStart();
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            var arg = (sender as FrameworkElement)?.DataContext as FuncArg;
            arg?.ValueChangeSequenceEnd();
        }

        private void PathDialog_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "ファイルを選択";
            dialog.Filter = "画像(*.jpg)|*.jpg";
            dialog.InitialDirectory = @"C:\";
            if (dialog.ShowDialog() == true)
            {
                Debug.WriteLine(sender.ToString());
                Debug.WriteLine(e.Source.ToString() + e.OriginalSource.ToString() + e.RoutedEvent.OwnerType);
                Debug.WriteLine(dialog.FileName);
            }
            else
            {
            }
        }
    }

    /// <summary>
    /// 型名と対応するテンプレートを保持するクラス
    /// </summary>
    public class DataTemplateHolder
    {
        public string TypeName { get; set; }
        public DataTemplate Template { get; set; }
    }

    class ContentTypeDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ContentTypeDataTemplateSelector()
        {
            Templates = new List<DataTemplateHolder>();
        }

        /// <summary>
        /// 変換先テンプレート
        /// </summary>
        public List<DataTemplateHolder> Templates { get; }

        /// <summary>
        /// itemに対応してDataTemplateを返す
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (element != null && item as FuncArg != null)
            {
                switch (((FuncArg)item).ArgValue)
                {
                    case string _:
                        return element.TryFindResource("pathTypeTemplate") as DataTemplate;

                    case bool _:
                        return element.TryFindResource("boolTypeTemplate") as DataTemplate;

                    case ValueType _:
                        return element.TryFindResource("valueTypeTemplate") as DataTemplate;
                }
            }
            return null;
        }
    }

    public static class ExtentionMethods
    {
        public static T DeepCopy<T>(this T source) where T : class
        {
            var ret = Activator.CreateInstance(typeof(T), true) as T;
            var type = source.GetType();
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            foreach(var field in fields)
            {
                field.SetValue(ret, field.GetValue(source));
            }

            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    if(property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(String)))
                    {
                        property.SetValue(ret, property.GetValue(source, null), null);
                    }
                    else
                    {
                        object propertySourceValue = property.GetValue(source, null);
                        if (propertySourceValue == null)
                        {
                            property.SetValue(ret, null, null);
                        }
                        else
                        {
                            property.SetValue(ret, DeepCopy(propertySourceValue), null);
                        }
                    }
                }

            }

            return ret;
        }
    }

    public class BooleanToBrushConverter : IValueConverter
    {
        public Color TrueColor { get; set; }
        public Color FalseColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new SolidColorBrush(TrueColor) : new SolidColorBrush(FalseColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
