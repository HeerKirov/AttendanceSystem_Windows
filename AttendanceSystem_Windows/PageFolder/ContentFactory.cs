using AttendanceSystem_Windows.HttpConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AttendanceSystem_Windows.PageFolder.Template;

namespace AttendanceSystem_Windows.PageFolder {
    /// <summary>
    /// 用于批量化快速构建列表与内容的类。
    /// </summary>
    public static class ContentFactory {
        public static UIElement InstanceGrid(InstanceArgs args) {
            var grid = new InstanceTemplate();

            return grid;
        }
    }
    
    
}
