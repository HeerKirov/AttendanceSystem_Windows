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
    /// 用作表头的一项。
    /// </summary>
    public partial class ListItemplateHeader : UserControl {
        public ListItemplateHeader() {
            InitializeComponent();
        }
        public ListItemplateHeader(string Text, bool Orderingable = false, string OrderingName = null, HeaderOrderingEvent Triggered = null) {
            InitializeComponent();
            this.Text = Text;
            this.Orderingable = Orderingable;
            orderingname = OrderingName;
            if (Triggered != null) OrderingTriggered += Triggered;
        }
        /// <summary>
        /// 可排序的项目。
        /// </summary>
        public bool Orderingable {
            get { return orderingable; }
            set {
                orderingable = value;
                OrderingButton.Visibility = orderingable ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private bool orderingable = false;
        /// <summary>
        /// 显示名称。
        /// </summary>
        public string Text {
            get { return Caption.Text; }
            set { Caption.Text = value; }
        }
        /// <summary>
        /// 字号。
        /// </summary>
        public double AllFontSize {
            get { return Caption.FontSize; }
            set { Caption.FontSize = value; }
        }
        /// <summary>
        /// 用作排序的名称。
        /// </summary>
        public string orderingname = "";
        /// <summary>
        /// 当前的排序状态。0=无排序，1=正向，2=反向
        /// </summary>
        private int orderingstate = 0;

        /// <summary>
        /// 触发排序事件。
        /// </summary>
        public event HeaderOrderingEvent OrderingTriggered;

        public delegate void HeaderOrderingEvent(ListItemplateHeader sender, HeaderOrderingArgs args);

        public class HeaderOrderingArgs : EventArgs {
            public readonly string orderingname;
            public readonly bool ordering;
            public HeaderOrderingArgs(bool ordering = false, string orderingname = null) {
                this.ordering = ordering;
                this.orderingname = orderingname;
            }
        }

        /// <summary>
        /// 点击了排序按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderingButton_Click(object sender, RoutedEventArgs e) {
            if (Orderingable) {
                orderingstate = (orderingstate + 1) % 3;
                OrderingButton.Content = buttoncontent[orderingstate];
                OrderingTriggered?.Invoke(this, new HeaderOrderingArgs(orderingstate!=0,
                    orderingstate==0?null:
                    orderingstate==1?orderingname:
                    "-"+orderingname
                ));
            }
        }
        private string[] buttoncontent = new string[] { "", "", "" };//- up down
    }
}
