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
using AttendanceSystem_Windows.Services;

namespace AttendanceSystem_Windows.PageFolder.Template {
    /// <summary>
    /// ListTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class ListTemplate : UserControl {
        public ListTemplate() {
            InitializeComponent();
        }
        /// <summary>
        /// 标题文字。
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
        public ListArgs Args {
            get { return args; }
        }
        private ListArgs args;
        /// <summary>
        /// 用于保存全部item实例的数组。
        /// </summary>
        private UIElement[] itemlist;
        /// <summary>
        /// 用于保存所有列的排序参数的数组。
        /// </summary>
        private string[] orderingconfigs;
        private string[] getorderings() {
            var ans = new List<string>();
            foreach (var i in orderingconfigs)
                if (i != null && i != "") ans.Add(i);
            return ans.Count > 0 ? ans.ToArray() : null;
        }

        /// <summary>
        /// 缓存列分布配置。
        /// </summary>
        private GridLength[] gridlength;
        /// <summary>
        /// 缓存创建项目页面。
        /// </summary>
        private CreateTemplate createpage;
        /// <summary>
        /// 获取/设置自定义页面的内容。
        /// </summary>
        public UIElement CustomPage {
            get { return custompage; }
            set {
               CustomUI.Children.Clear();
               if (value != null) CustomUI.Children.Add(value);
               custompage = value;
            }
        }
        private UIElement custompage;

        /// <summary>
        /// 对模板进行构造，构造内容仅包括表头。
        /// </summary>
        /// <param name="args"></param>
        public void Construct(ListArgs args) {
            if (args == null) return;
            this.args = args;
            Caption = args.caption;
            SearchModule.Visibility = args.searchable ? Visibility.Visible : Visibility.Collapsed;
            CreateButton.Visibility = args.createable ? Visibility.Visible : Visibility.Collapsed;
            //如果存在，生成创建页。
            if (args.createable) {
                createpage = new CreateTemplate();
                createpage.Construct(args.createargs);
            }
            //生成排序参数数组。
            orderingconfigs = new string[args.columns.Count];
            for (int i = 0; i < orderingconfigs.Length; ++i) orderingconfigs[i] = "";
            //制作列分布配置。
            gridlength = new GridLength[args.columns.Count];
            for (int i = 0; i < args.columns.Count; ++i)
                gridlength[i] = args.columns[i].width;
            //制作表头项目并生成表头。
            var headers = new UIElement[args.columns.Count];
            for(int i = 0; i < args.columns.Count; ++i) {
                var c = args.columns[i];
                var lambda_i = i;//局部闭包
                var hs = c.ordering != null && c.ordering != "" ? new ListItemplateHeader(c.header, true, c.ordering, (s, a) => {
                    orderingconfigs[lambda_i] = a.orderingname;
                    UpdateData();
                }) : new ListItemplateHeader(c.header);
                hs.AllFontSize = c.fontsize ?? 18;
                headers[i] = hs;
            }
            ListHeader.SetChildren(gridlength, headers);
        }

        /// <summary>
        /// 链接生成最新内容，伴随查询参数。
        /// </summary>
        public void UpdateData() {
            
            itemlist = null;
            var search = SearchBox.Text;
            if (search == "") search = null;
            Task.Run(() => {
                var res = new Response[args.urls.Length];
                for (int i = 0; i < res.Length; ++i) res[i] = args.api[args.urls[i].url].List(args.getFilter(), getorderings(), search);
                var flag = true;
                foreach (var i in res)
                    if (!i.statuslike("2**")) {
                        flag = false;
                        break;
                    }
                if (flag) {
                    var datas = res.map(t => t.list) as JArray[];
                    Dispatcher.Invoke(() => {
                        //var Items = this.Items;
                        var brush_white = new SolidColorBrush(Colors.White);
                        var brush_gray = new SolidColorBrush(Colors.LightGray);
                        Items.Clear();
                        int index = 0;
                        foreach(var data in datas[0]) {
                            //构造每一个条目的记录。
                            var row = new ListTemplateItem();
                            var objects = new UIElement[args.columns.Count];
                            for(int i = 0; i < objects.Length; ++i) {//构造一条记录的每一个项。
                                var c = args.columns[i];
                                
                                var item = new TextBlock();
                                item.HorizontalAlignment = HorizontalAlignment.Left;
                                item.VerticalAlignment = VerticalAlignment.Center;
                                item.FontSize = c.fontsize ?? args.fontsize;
                                item.Margin = c.margin ?? args.margin;
                                //首先过滤名称，检查是主键内容还是副URL的内容。
                                JToken jdata = null;
                                if (c.name.Contains("/")) {//这表示使用斜线分割的url's name和json's name。
                                    var sp = c.name.Split('/');
                                    var url_index = args.urls.firstAt(u => u.name == sp[0]);//得知这是第几条URL。
                                    if (url_index < 0) throw new Exception("找不到对应的URL。Column的name书写错误。");
                                    var second_url = args.urls[url_index];//获得副url的引用。

                                    foreach(var seconddata in datas[url_index]) {
                                        if (seconddata[second_url.primarykey].ToString() == data[args.urls[0].primarykey].ToString()) {//这个标志相当于二者对等。
                                            jdata = seconddata[sp[1]];
                                            break;
                                        }
                                    }
                                }else {//这表示是主键的内容。
                                    jdata = data[c.name];
                                }
                                //按类型创建UI内容。
                                if (c.type == "text") item.Text = jdata.ToString();
                                else if (c.type == "custom") item.Text = (c.data as Func<JToken, string>)(jdata);
                                else if (c.type == "choice") item.Text = (c.data as IDictionary<string, string>)[jdata.ToString()];
                                if (c.customaction != null) {
                                    item.PreviewMouseLeftButtonDown += (s, e) => {
                                        c.customaction(data);
                                    };
                                }
                                if (c.hyperlink != null){
                                    var goal = c.hyperlink(data);//局部闭包
                                    item.PreviewMouseLeftButtonDown += (s, e) => {//点击超链接事件。
                                        NavigatorPage.NavigatorGoto(goal.Item1, goal.Item2);
                                    };
                                }
                                objects[i] = item;
                            }
                            row.SetChildren(gridlength,objects);
                            row.Background = index % 2 == 0 ? brush_gray : brush_white;
                            //itemlist[index] = row;
                            Items.Add(row);
                            ++index;
                        }
                    });
                }else {//抛出错误，存在不能正确连接的url。
                    NavigatorPage.MsgSystem.Show(null, "错误", res[0].content);
                }
            });
        }

        private void SearhcButton_Click(object sender, RoutedEventArgs e) {
            UpdateData();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.IsDown && e.Key == Key.Enter) UpdateData();
        }
        /// <summary>
        /// 按下了新建按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateButton_Click(object sender, RoutedEventArgs e) {
            //在可用的情况下，这个按钮会开关创建页面。
            if (CustomPage == createpage) {//关闭,同时试图进行一次数据刷新.
                CustomPage = null;
                UpdateData();
            }else {
                CustomPage = createpage;
            }
        }
    }
    /// <summary>
    /// 用于列表模板构造的参数类。
    /// </summary>
    public class ListArgs {
        public ListArgs(string URL,RestApi Api, string Caption, 
            bool Searchable = false, double FontSize = 18, Thickness Margin = new Thickness(), bool Createable = false, CreateArgs Createargs = null) {
            urls = new iURL[] { new iURL(URL) };
            api = Api;
            caption = Caption;
            fontsize = FontSize;
            margin = Margin;
            searchable = Searchable;
            columns = new List<Column>();
            createable = Createable;
            createargs = Createargs;
        }
        public ListArgs(iURL[] URL, RestApi Api, string Caption,
            bool Searchable = false, double FontSize = 18, Thickness Margin = new Thickness(), bool Createable = false, CreateArgs Createargs = null) {
            urls = URL;
            api = Api;
            caption = Caption;
            fontsize = FontSize;
            margin = Margin;
            searchable = Searchable;
            columns = new List<Column>();
            createable = Createable;
            createargs = Createargs;
        }
        /// <summary>
        /// url。
        /// </summary>
        public iURL[] urls;
        /// <summary>
        /// api类。
        /// </summary>
        public RestApi api;
        /// <summary>
        /// 模板标题。
        /// </summary>
        public string caption;
        /// <summary>
        /// 字体大小。
        /// </summary>
        public double fontsize;
        /// <summary>
        /// 统一的边距。
        /// </summary>
        public Thickness margin;
        /// <summary>
        /// 允许搜索。
        /// </summary>
        public bool searchable;
        /// <summary>
        /// 允许创建新项目。如果允许创建，那么创建新项目的模板应该非空。
        /// </summary>
        public bool createable;
        /// <summary>
        /// 构造新项目的子页面模板。
        /// </summary>
        public CreateArgs createargs;

        /// <summary>
        /// 列项目列表。
        /// </summary>
        public List<Column> columns;

        public struct iURL {
            public string url;
            public string name;
            public string primarykey;
            public iURL(string url, string name = null, string primarykey = null) {
                this.url = url;
                this.name = name;
                this.primarykey = primarykey;
            }
            public override string ToString() {
                return url;
            }
        }
        /// <summary>
        /// 为过滤器追加一个条件。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ListArgs filter(string key,string value) {
            if (filter_dict == null) filter_dict = new Dictionary<string, string>();
            filter_dict[key] = value;
            return this;
        }
        public IDictionary<string,string> getFilter() {
            return filter_dict;
        }
        private Dictionary<string, string> filter_dict = null;


        /// <summary>
        /// 列。
        /// </summary>
        public struct Column {
            /* 项目类型
             * text 直接作为文本显示。默认。
             * choice 根据内容映射。要求data传入一个IDictionary<string,string>(值-显示)的字典用于显示。
             * custom 自定义。要求data传入一个Func<JToken,string>类型的委托以处理对象的JToken数据。
             */
            public Column(string name,string header, GridLength? width = null, string ordering = null,
                double?fontsize = null, Thickness? margin = null, 
                string type="text",object data = null,
                Func<JToken, Tuple<string, IDictionary<string, object>>> hyperlink = null,
                Action<JToken> customaction = null) {
                this.name = name;
                this.header = header;
                this.fontsize = fontsize;
                this.ordering = ordering;
                this.margin = margin;
                this.type = type;
                this.data = data;
                this.hyperlink = hyperlink;
                this.width = width ?? new GridLength(1, GridUnitType.Star);
                this.customaction = customaction;

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
            /// 允许以此项目为基准排序。该参数是实际的排序参数名称。
            /// </summary>
            public string ordering;
            /// <summary>
            /// 字体大小。
            /// </summary>
            public double? fontsize;
            /// <summary>
            /// 自己的边距。
            /// </summary>
            public Thickness? margin;
            /// <summary>
            /// 列的宽度比率。
            /// </summary>
            public GridLength width;
            /// <summary>
            /// 附加数据，根据需要转换类型。
            /// </summary>
            public object data;
            /// <summary>
            /// 超链接构造委托。如果这个委托不为空，那么目标字段转化为超链接。
            /// 传入参数：instance,name
            /// 返回参数：Goto的Uri和kwargs。
            /// </summary>
            public Func<JToken, Tuple<string,IDictionary<string,object>>> hyperlink;
            /// <summary>
            /// 自定义构造委托。如果这个委托不为空，那么在点击目标字段时执行下属委托。
            /// </summary>
            public Action<JToken> customaction;
        }

        public static AutoHyperLinkConstructor autohyperlink(string name) {
            return new AutoHyperLinkConstructor(name);
        }
        public class AutoHyperLinkConstructor {
            private string name;
            private Dictionary<string, object> dict = new Dictionary<string, object>();
            public AutoHyperLinkConstructor(string name) {
                this.name = name;
            }
            public AutoHyperLinkConstructor add(string key,string value) {
                dict.Add(key, value);
                return this;
            }
            public AutoHyperLinkConstructor add(string kv) {
                dict.Add(kv, kv);
                return this;
            }
            private Dictionary<string,object> getj_args(JToken j) {
                var ans = new Dictionary<string, object>();
                foreach(var i in dict) {
                    ans.Add(i.Key, j[i.Value].ToString());
                }
                return ans;
            }
            public Func<JToken, Tuple<string, IDictionary<string, object>>> lambda
                => j => new Tuple<string, IDictionary<string, object>>(name, getj_args(j));
        }
    }
}
