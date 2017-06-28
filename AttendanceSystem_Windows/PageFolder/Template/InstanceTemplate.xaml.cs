using AttendanceSystem_Windows.HttpConnection;
using AttendanceSystem_Windows.PageFolder.Utils;
using AttendanceSystem_Windows.Services;
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
    /// InstanceTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class InstanceTemplate : UserControl {
        public InstanceTemplate() {
            InitializeComponent();
        }
        /// <summary>
        /// 项目的标题文字。
        /// </summary>
        public string Caption {
            get { return CaptionText.Text; }
            set { CaptionText.Text = value; }
        }
        /// <summary>
        /// 该模板下的子项目的表项。
        /// </summary>
        public UIElementCollection Items {
            get { return ContentUI.Children; }
        }
        /// <summary>
        /// 构造参数。
        /// </summary>
        public InstanceArgs Args {
            get { return args; }
        }
        private InstanceArgs args;
        /// <summary>
        /// 用于保存全部item实例的数组。
        /// </summary>
        private object[] itemlist;
        /// <summary>
        /// 对模板进行构造。
        /// </summary>
        /// <param name="args"></param>
        public void Construct(InstanceArgs args) {
            if (args == null) return;
            this.args = args;
            Items.Clear();
            Caption = args.caption;
            itemlist = new object[args.items.Count];
            DeleteButton.Visibility = args.deleteable ? Visibility.Visible : Visibility.Collapsed;
            SaveButton.Visibility = args.onlywrite ? Visibility.Visible : Visibility.Collapsed;
            ConstructCustomButton();
            for(int i = 0; i < args.items.Count; ++i) {
                var cachei = i;
                var thi = args.items[i];
                if (thi.type == "text") {
                    var item = new HeaderSuperText();
                    item.Header = thi.header;
                    item.Editable = args.onlywrite ? EditAbleStatus.Writeonly : thi.editable;
                    item.ColumnWidth = thi.columnwidth ?? args.columnwidth;
                    item.AllFontSize = thi.fontsize ?? args.fontsize;
                    item.Margin = thi.margin ?? args.margin;
                    itemlist[i] = item;
                    Items.Add(item);
                    item.Submit += (s, e) => {
                        var data = $@"{{
                            ""{args.items[cachei].name}"":""{e.text}""
                        }}";
                        Task.Run(() => {
                            var res = args.api[args.url][args.instance].Update(data);
                            if (!res.statuslike("2**")) {
                                NavigatorPage.MsgSystem.Show((sr, er) => { }, "发生错误", res.content, Visibility.Hidden, new string[] { "确认" });
                                Dispatcher.Invoke(() => (s as Utils.HeaderSuperText).Text = "");
                            }
                        });
                        return e.text;
                    };
                }else if (thi.type == "multitext") {
                    var item = new HeaderSuperText();
                    item.Header = thi.header;
                    item.Editable = args.onlywrite ? EditAbleStatus.Writeonly : thi.editable;
                    item.ColumnWidth = thi.columnwidth ?? args.columnwidth;
                    item.AllFontSize = thi.fontsize ?? args.fontsize;
                    item.Margin = thi.margin ?? args.margin;
                    item.Multiable = true;
                    itemlist[i] = item;
                    Items.Add(item);
                    item.Submit += (s, e) => {
                        var data = $@"{{
                            ""{args.items[cachei].name}"":""{(s as HeaderSuperText).toJsonString()}""
                        }}";
                        Task.Run(() => {
                            var res = args.api[args.url][args.instance].Update(data);
                            if (!res.statuslike("2**")) {
                                NavigatorPage.MsgSystem.Show((sr, er) => { }, "发生错误", res.content, Visibility.Hidden, new string[] { "确认" });
                                Dispatcher.Invoke(() => (s as Utils.HeaderSuperText).Text = "");
                            }
                        });
                        return e.text;
                    };
                } else if (thi.type == "choice") {
                    var item = new HeaderSuperChoice();
                    item.Header = thi.header;
                    item.Editable = args.onlywrite ? EditAbleStatus.Writeonly : thi.editable;
                    item.ColumnWidth = thi.columnwidth ?? args.columnwidth;
                    item.AllFontSize = thi.fontsize ?? args.fontsize;
                    item.Margin = thi.margin ?? args.margin;
                    itemlist[i] = item;
                    item.Choices = thi.data as KeyValuePair<string, string>[];
                    Items.Add(item);
                    item.Submit += (s, e) => {
                        var data = $@"{{
                            ""{args.items[cachei].name}"":""{e.key}""
                        }}";
                        Task.Run(() => {
                            var res = args.api[args.url][args.instance].Update(data);
                            if (!res.statuslike("2**")) {
                                NavigatorPage.MsgSystem.Show((sr, er) => { }, "发生错误", res.content, Visibility.Hidden, new string[] { "确认" });
                                Dispatcher.Invoke(() => (s as Utils.HeaderSuperChoice).SelectedIndex = -1);
                            }
                        });
                        return e.selected;
                    };
                } else if (thi.type == "multi") {
                    var item = new HeaderSuperMultiChoice();
                    item.Header = thi.header;
                    item.Editable = args.onlywrite ? EditAbleStatus.Writeonly : thi.editable;
                    item.ColumnWidth = thi.columnwidth ?? args.columnwidth;
                    item.AllFontSize = thi.fontsize ?? args.fontsize;
                    item.Margin = thi.margin ?? args.margin;
                    itemlist[i] = item;
                    item.ItemSource = thi.data as KeyValuePair<string, string>[];
                    Items.Add(item);
                    item.Submit += (s, e) => {
                        var data = $@"{{
                            ""{args.items[cachei].name}"":""{(s as HeaderSuperMultiChoice).toJsonString()}""
                        }}"; 
                        Task.Run(() => {
                            var res = args.api[args.url][args.instance].Update(data);
                            if (!res.statuslike("2**")) {
                                NavigatorPage.MsgSystem.Show((sr, er) => { }, "发生错误", res.content, Visibility.Hidden, new string[] { "确认" });
                                Dispatcher.Invoke(() => (s as Utils.HeaderSuperMultiChoice).ClearContent());
                            }
                        });
                        return e.value;
                    };
                }
            }
        }

        public void ConstructCustomButton() {
            //首先清除自定义按钮。
            CustomButtonStack.Children.Clear();
            //构造自定义按钮。
            if (args.custombutton != null) {
                foreach (var i in args.custombutton) {
                    var btn = new Button();
                    var action = i.action;
                    btn.Style = Resources["FuncButtonStyle"] as Style;
                    btn.FontFamily = new FontFamily(i.fontfamily);
                    btn.Content = i.title;
                    btn.ToolTip = i.tips;
                    btn.Click += (s, e) => action();
                    CustomButtonStack.Children.Add(btn);
                }
            }
        }
        /// <summary>
        /// 连接查询最新数据。
        /// </summary>
        public void UpdateData() {
            //进行url连接。
            Task.Run(() => {
                var res = args.api[args.url][args.instance].Retrieve();
                if (res.statuslike("2**")) {
                    var data = res.instance;
                    Dispatcher.Invoke(() => {
                        for (int i = 0; i < args.items.Count; ++i) {
                            if (args.items[i].type == "text") {
                                var item = itemlist[i] as HeaderSuperText;
                                item.Text = data[args.items[i].name].ToString();
                            } else if (args.items[i].type == "choice") {
                                var item = itemlist[i] as HeaderSuperChoice;
                                item.Key = data[args.items[i].name].ToString();
                            } else if (args.items[i].type == "multi") {
                                var item = itemlist[i] as HeaderSuperMultiChoice;
                                var ja = data[args.items[i].name] as JArray;
                                var sa = new string[ja.Count];
                                int index = 0;
                                foreach(var j in ja)sa[index++] = j.ToString();
                                item.setByItem(sa);
                            }else if (args.items[i].type == "multitext") {
                                var item = itemlist[i] as HeaderSuperText;
                                var ja = data[args.items[i].name] as JArray;
                                var sa = new string[ja.Count];
                                int index = 0;
                                foreach (var j in ja) sa[index++] = j.ToString();
                                item.MultiText = sa;
                            }
                        }
                    });
                } else {
                    NavigatorPage.MsgSystem.Show(null, "错误", res.content);
                }
            });
        }
        /// <summary>
        /// 按下删除按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e) {
            NavigatorPage.MsgSystem.Show((s, r) => {
                if (r.CheckIndex == 0) {
                    //执行删除操作并回退。
                    var res = args.api[args.url][args.instance].Destroy();
                    if(res.statuslike("204")) {
                        if (args.deleteaction == null) {
                            if (NavigatorPage.Get().NavigationService.CanGoBack)
                                NavigatorPage.Get().NavigationService.GoBack();
                        } else args.deleteaction();
                    }else {
                        NavigatorPage.MsgSystem.Show(null, "错误", res.content);
                    }
                }
            }, "确认", "你确定要删除该条目吗？", buttonname: new string[] { "是", "否" });
        }
        /// <summary>
        /// 按下统一保存按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            SaveButton.IsEnabled = false;
            //首先构造出所需要的data.
            var jdata = new JObject();
            for(int i = 0; i < itemlist.Length; ++i) {
                var item = itemlist[i];//UI项
                var thi = args.items[i];//模板中的数据项
                if (thi.type == "text") {
                    jdata.Add(thi.name, new JValue((item as HeaderSuperText).Text));
                }else if(thi.type == "choice") {
                    jdata.Add(thi.name, new JValue((item as HeaderSuperChoice).Key));
                }else if (thi.type == "multi") {
                    jdata.Add(thi.name, JArray.Parse((item as HeaderSuperMultiChoice).toJsonString()));
                }else if (thi.type == "multitext") {
                    jdata.Add(thi.name, JArray.Parse((item as HeaderSuperText).toJsonString()));
                }
            }

            //然后进行请求连接。
            Task.Run(() => {
                var res = args.api[args.url][args.instance].Update(jdata.ToString());
                Dispatcher.Invoke(() => {
                    if (res.statuslike("2**")) {
                        NavigatorPage.MsgSystem.Show(null, "通知", "保存成功。");
                    } else {
                        NavigatorPage.MsgSystem.Show(null, "错误", res.content);
                    }
                    SaveButton.IsEnabled = true;
                });
            });
            
        }
    }

    /// <summary>
    /// 用于构建实例内容的参数类。
    /// </summary>
    public class InstanceArgs {
        public InstanceArgs(string URL, string InstanceId, RestApi Api, string Caption, 
            double ColumnWidth = 3, double FontSize = 18, Thickness Margin = new Thickness(), 
            bool Deleteable = true, bool Onlywrite = false, Action Deleteaction = null,
            CustomButton[] Custombutton = null) {
            url = URL;
            instance = InstanceId;
            api = Api;
            items = new List<Item>();
            caption = Caption;
            columnwidth = ColumnWidth;
            fontsize = FontSize;
            margin = Margin;
            deleteable = Deleteable;
            onlywrite = Onlywrite;
            deleteable = Deleteable;
            custombutton = Custombutton;
        }
        /// <summary>
        /// url与instance。
        /// </summary>
        public string url, instance;
        /// <summary>
        /// api类。
        /// </summary>
        public RestApi api;
        /// <summary>
        /// 项目列表。
        /// </summary>
        public List<Item> items;
        /// <summary>
        /// 模板标题。
        /// </summary>
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
        /// 允许删除。
        /// </summary>
        public bool deleteable;
        /// <summary>
        /// 仅用于修改，所有的条目都是writeonly状态且有统一的保存按钮。
        /// </summary>
        public bool onlywrite;
        /// <summary>
        /// 自定义删除操作的动作。
        /// </summary>
        public Action deleteaction;
        /// <summary>
        /// 自定义的按钮序列。
        /// </summary>
        public CustomButton[] custombutton;

        public struct Item {
            /* 项目类型
             * text 文本。
             * choice 选择框。需要data为一个可作为ItemSource的集。
             */
            public Item(string Name, string Header, string Type, 
                EditAbleStatus Editable = EditAbleStatus.Readonly,
                double? ColumnWidth = null,double? FontSize = null, Thickness? Margin = null, object data = null) {
                name = Name;
                header = Header;
                editable = Editable;
                type = Type;
                columnwidth = ColumnWidth;
                fontsize = FontSize;
                margin = Margin;
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
            /// 可编辑的状态。
            /// </summary>
            public EditAbleStatus editable;
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
    }
}
