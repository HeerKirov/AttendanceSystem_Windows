using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AttendanceSystem_Windows.PageFolder.Utils {
    /// <summary>
    /// 用作ListTemplate的表项或表头。
    /// </summary>
    public partial class ListTemplateItem : UserControl {
        public ListTemplateItem() {
            InitializeComponent();
        }
        /// <summary>
        /// 确定的列数。
        /// </summary>
        public int Length {
            get { return columns?.Length ?? 0; }
        }
        public GridLength[] Columns {
            get { return columns; }
            set {
                if (value != null) {
                    columns = value;
                    ContentUI.ColumnDefinitions.Clear();
                    foreach(var i in value) {
                        var c = new ColumnDefinition();
                        c.Width = i;
                        ContentUI.ColumnDefinitions.Add(c);
                    }
                    Clear();
                    items = new UIElement[columns.Length];
                }
            }
        }
        public void SetChildren(GridLength[] Columns, UIElement[] Items) {
            this.Columns = Columns;
            for (int i = 0; i < Items.Length; ++i) this[i] = Items[i];
        }
        private GridLength[] columns;
        /// <summary>
        /// 列中的元素。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public UIElement this[int i] {
            get { return items[i]; }
            set {
                if (i < 0 || i >= items.Length) throw new IndexOutOfRangeException();
                if (items[i] != null) ContentUI.Children.Remove(items[i]);
                items[i] = value;
                ContentUI.Children.Add(value);
                Grid.SetColumn(value, i);
            }
        }
        private UIElement[] items;
        /// <summary>
        /// 清除所有子元素。
        /// </summary>
        public void Clear() {
            ContentUI.Children.Clear();
            if(items != null) for (int i = 0; i < items.Length; ++i) items[i] = null;
        }
    }
}
