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

namespace AttendanceSystem_Windows.PageFolder.Basic {
    /// <summary>
    /// schedule_timeitem.xaml 的交互逻辑
    /// </summary>
    public partial class schedule_timeitem : UserControl {
        public Tuple<int, string, string> content {
            get { return obj; }
            set {
                obj = value;
                L1.Text = obj.Item1.ToString();
                L2.Text = obj.Item2.ToString();
                L3.Text = obj.Item3.ToString();
                BG.Background = new SolidColorBrush(obj.Item1 % 2 == 0 ? Colors.LightGray : Colors.White);
            }
        }
        Tuple<int, string, string> obj;
        public schedule_timeitem() {
            InitializeComponent();
        }
    }
}
