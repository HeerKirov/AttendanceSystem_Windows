using AttendanceSystem_Windows.HttpConnection;
using AttendanceSystem_Windows.PageFolder.Utils;
using Newtonsoft.Json.Linq;
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

namespace AttendanceSystem_Windows.PageFolder.Template {
    /// <summary>
    /// CreateTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class CreateTemplate : UserControl {
        public CreateTemplate() {
            InitializeComponent();
        }
        public string Caption {
            get { return args?.caption; }
        }
        public CreateArgs Args {
            get { return args; }
        }
        private CreateArgs args;
        /// <summary>
        /// 该模板下的子项目的表项。
        /// </summary>
        public UIElementCollection Items {
            get { return ContentUI.Children; }
        }
        /// <summary>
        /// 用于保存全部item实例的数组。
        /// </summary>
        private object[] itemlist;
        /// <summary>
        /// 对UI进行构造。
        /// </summary>
        /// <param name="args"></param>
        public void Construct(CreateArgs args) {
            if (args == null) return;
            this.args = args;
            Items.Clear();
            itemlist = new object[args.items.Count];
            for (int i = 0; i < args.items.Count; ++i) {
                var cachei = i;
                var thi = args.items[i];
                if (thi.type == "text") {
                    var item = new HeaderSuperText();
                    item.Header = thi.header;
                    item.Editable = EditAbleStatus.Writeonly;
                    item.ColumnWidth = thi.columnwidth ?? args.columnwidth;
                    item.AllFontSize = thi.fontsize ?? args.fontsize;
                    item.Margin = thi.margin ?? args.margin;
                    itemlist[i] = item;
                    Items.Add(item);
                } else if (thi.type == "choice") {
                    var item = new HeaderSuperChoice();
                    item.Header = thi.header;
                    item.Editable = EditAbleStatus.Writeonly;
                    item.ColumnWidth = thi.columnwidth ?? args.columnwidth;
                    item.AllFontSize = thi.fontsize ?? args.fontsize;
                    item.Margin = thi.margin ?? args.margin;
                    itemlist[i] = item;
                    item.Choices = thi.data as KeyValuePair<string, string>[];
                    Items.Add(item);
                }else if (thi.type == "multi") {
                    var item = new HeaderSuperMultiChoice();
                    item.Header = thi.header;
                    item.Editable = EditAbleStatus.Writeonly;
                    item.ColumnWidth = thi.columnwidth ?? args.columnwidth;
                    item.AllFontSize = thi.fontsize ?? args.fontsize;
                    item.Margin = thi.margin ?? args.margin;
                    itemlist[i] = item;
                    item.ItemSource = thi.data as KeyValuePair<string, string>[];
                    Items.Add(item);
                }else if (thi.type == "multitext") {
                    var item = new HeaderSuperText();
                    item.Header = thi.header;
                    item.Multiable = true;
                    item.Editable = EditAbleStatus.Writeonly;
                    item.ColumnWidth = thi.columnwidth ?? args.columnwidth;
                    item.AllFontSize = thi.fontsize ?? args.fontsize;
                    item.Margin = thi.margin ?? args.margin;
                    itemlist[i] = item;
                    Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 按下了提交按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            SubmitButton.IsEnabled = false;
            //首先构造出所需要的data.
            var jdata = new JObject();
            for (int i = 0; i < itemlist.Length; ++i) {
                var item = itemlist[i];//UI项
                var thi = args.items[i];//模板中的数据项
                if (thi.type == "text") {
                    jdata.Add(thi.name, new JValue((item as HeaderSuperText).Text));
                } else if (thi.type == "choice") {
                    jdata.Add(thi.name, new JValue((item as HeaderSuperChoice).Key));
                } else if (thi.type == "multi") {
                    var ja = new JArray();
                    foreach (var s in (item as HeaderSuperMultiChoice).toArray()) ja.Add(new JValue(s));
                    jdata.Add(thi.name, ja);
                }else if (thi.type == "multitext") {
                    var ja = new JArray();
                    foreach (var s in (item as HeaderSuperText).MultiText) ja.Add(new JValue(s));
                    jdata.Add(thi.name, ja);
                }else if (thi.type == "hidden") {
                    jdata.Add(thi.name, JToken.Parse(Args.param[thi.data as string]));
                }
            }
            //然后进行连接。
            Task.Run(() => {
                var str = jdata.ToString();
                var res = args.api[args.url].Create(str);
                Dispatcher.Invoke(() => {
                    if (res.statuslike("2**")) {
                        NavigatorPage.MsgSystem.Show(null, "通知", "提交成功。");
                        //需要清空所有的内容。
                        for (int i = 0; i < itemlist.Length; ++i) {
                            var item = itemlist[i];//UI项
                            var thi = args.items[i];//模板中的数据项
                            if (thi.type == "text" || thi.type == "multitext") (item as HeaderSuperText).ClearContent();
                            else if (thi.type == "choice") (item as HeaderSuperChoice).ClearContent();
                            else if (thi.type == "multi") (item as HeaderSuperMultiChoice).ClearContent();
                            else if (thi.type == "hidden") {/*do nothing*/}
                        }
                    } else {
                        NavigatorPage.MsgSystem.Show(null, "错误", res.content);
                    }
                    SubmitButton.IsEnabled = true;
                });
            });
        }
    }
    public class CreateArgs {
        public CreateArgs(string URL,RestApi Api, string Caption,
            double ColumnWidth = 3, double FontSize = 18, Thickness Margin = new Thickness()) {
            url = URL;
            api = Api;
            items = new List<Item>();
            caption = Caption;
            columnwidth = ColumnWidth;
            fontsize = FontSize;
            margin = Margin;
        }
        /// <summary>
        /// URL.
        /// </summary>
        public string url;
        /// <summary>
        /// API.
        /// </summary>
        public RestApi api;
        /// <summary>
        /// 构造的列表项。
        /// </summary>
        public List<Item> items;
        public string caption;
        /// <summary>
        /// 文本部分宽度比例。
        /// </summary>
        public double columnwidth;
        /// <summary>
        /// 字体大小。
        /// </summary>
        public double fontsize;
        /// <summary>
        /// 统一的边距。
        /// </summary>
        public Thickness margin;
        /// <summary>
        /// 给createargs用的数据项。
        /// </summary>
        public struct Item {
            public Item(string name,string header, string type,
                double? ColumnWidth = null, double? FontSize = null, Thickness? Margin = null, object data = null) {
                this.name = name;
                this.header = header;
                this.type = type;
                this.columnwidth = ColumnWidth;
                this.fontsize = FontSize;
                this.margin = Margin;
                this.data = data;
            }
            /// <summary>
            /// 在json数据中的名称显示。
            /// </summary>
            public string name;
            /// <summary>
            /// 显示头。
            /// </summary>
            public string header;
            /// <summary>
            /// 构造的项目类型。
            /// </summary>
            public string type;
            /// <summary>
            /// 文本部分宽度比例。
            /// </summary>
            public double? columnwidth;
            /// <summary>
            /// 字体大小。
            /// </summary>
            public double? fontsize;
            /// <summary>
            /// 自己的边距。
            /// </summary>
            public Thickness? margin;
            /// <summary>
            /// 附加数据，根据需要转换类型。
            /// </summary>
            public object data;
        }
        /// <summary>
        /// 备选参数表。
        /// </summary>
        public Dictionary<string, string> param { get; private set; } = new Dictionary<string, string>();
    }
}
