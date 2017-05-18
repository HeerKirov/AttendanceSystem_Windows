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

namespace AttendanceSystem_Windows {
    /// <summary>
    /// NavigatorPage.xaml 的交互逻辑
    /// </summary>
    public partial class NavigatorPage : Page {
        public static NavigatorPage Get() => reference == null ? reference = new NavigatorPage() : reference;
        private static NavigatorPage reference = null;

        private NavigatorPage() {
            InitializeComponent();
            //var x = new Button();
            
        }

        private void OptionBtn_Click(object sender, RoutedEventArgs e) {
            OptionMenu.IsOpen = true;
        }
    }
}
