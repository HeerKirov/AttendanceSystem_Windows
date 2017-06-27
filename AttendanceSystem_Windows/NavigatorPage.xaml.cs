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
using AttendanceSystem_Windows.PageFolder;
using AttendanceSystem_Windows.PageFolder.Utils;

namespace AttendanceSystem_Windows {
    /// <summary>
    /// NavigatorPage.xaml 的交互逻辑
    /// </summary>
    public partial class NavigatorPage : Page {
        public static NavigatorPage Get() => reference == null ? reference = new NavigatorPage() : reference;
        public static MessageSystem MsgSystem => Get().msgSys;
        public static bool NavigatorGoto(string name, IDictionary<string,object> kwargs = null) {
            return Get()?.frameSys.Goto(name, kwargs) ?? false;
        }
        private static NavigatorPage reference = null;
        private void InitPageForCharacter() {
            funcSys = new FunctionSystem(frameSys, CharacterHeader, CharacterStackPanel, FunctionStackPanel,
                Resources["CharacterListButton"] as Style, Resources["FunctionListButton"] as Style,
                new FunctionSystem.CharacterTemplate[] {
                    new FunctionSystem.CharacterTemplate("general","一般用户", u => u.HasNoAuthority(), new FunctionSystem.FunctionTemplate[] {
                        new FunctionSystem.FunctionTemplate("document","个人资料","basic-self-document"),
                    }),
                    new FunctionSystem.CharacterTemplate("student", "学生", new Authority[] { Authority.Student },new FunctionSystem.FunctionTemplate[] {
                        new FunctionSystem.FunctionTemplate("document","基本信息","student-self-document"),
                        new FunctionSystem.FunctionTemplate("coursetable","课程表","student-self-coursetable"),
                        new FunctionSystem.FunctionTemplate("attendances","出勤统计","student-self-attendances"),
                        new FunctionSystem.FunctionTemplate("classroomrecords","教室使用记录","student-self-classroomrecords"),
                        new FunctionSystem.FunctionTemplate("leaves","请假记录","student-self-leaves"),
                    }),
                    new FunctionSystem.CharacterTemplate("teacher","教师",new Authority[] { Authority.Teacher},new FunctionSystem.FunctionTemplate[] {
                        new FunctionSystem.FunctionTemplate("document","基本信息","teacher-self-document"),
                        new FunctionSystem.FunctionTemplate("coursetable","课程表","teacher-self-coursetable"),
                        new FunctionSystem.FunctionTemplate("courses","课程列表","generalmanager-courses"),
                        new FunctionSystem.FunctionTemplate("exchanges","调课记录","teacher-self-exchanges"),
                    }),
                    new FunctionSystem.CharacterTemplate("instructors","辅导员",new Authority[] { Authority.Instructor},new FunctionSystem.FunctionTemplate[] {
                        new FunctionSystem.FunctionTemplate("document","基本信息","instructor-self-document"),
                        new FunctionSystem.FunctionTemplate("classes","班级列表","generalmanager-classes"),
                        new FunctionSystem.FunctionTemplate("approveleaves","请假审批","generalmanager-approveleaves"),
                    }),
                    new FunctionSystem.CharacterTemplate("office","教务处",new Authority[] { Authority.Office},new FunctionSystem.FunctionTemplate[] {
                        new FunctionSystem.FunctionTemplate("document","基本信息","office-self-document"),
                        new FunctionSystem.FunctionTemplate("students","学生列表","office-self-students"),
                        new FunctionSystem.FunctionTemplate("teachers","教师列表","office-self-teachers"),
                        new FunctionSystem.FunctionTemplate("instructors","辅导员列表","office-self-instructors"),
                        new FunctionSystem.FunctionTemplate("classes","班级列表","generalmanager-classes"),
                        new FunctionSystem.FunctionTemplate("courses","课程列表","generalmanager-courses"),
                        new FunctionSystem.FunctionTemplate("approveleaves","请假审批","generalmanager-approveleaves"),
                        new FunctionSystem.FunctionTemplate("approvexchanges","调课审批","generalmanager-approvexchanges"),
                    }),
                    new FunctionSystem.CharacterTemplate("admin", "管理", new Authority[] {
                        Authority.Admin,Authority.ClassManage,Authority.ClassroomManage,Authority.CourseManage,Authority.InstructorManage,
                        Authority.Root,Authority.StudentManage,Authority.TeacherManage,Authority.UserManage
                    }, new FunctionSystem.FunctionTemplate[] {
                        new FunctionSystem.FunctionTemplate("document","管理员资料","admin-self-document",null,u=>u.HasAuthorityOrRoot(Authority.Admin)),
                        new FunctionSystem.FunctionTemplate("root-auth","全权限管理","admin-root-auth",null,u=>u.HasAuthority(Authority.Root)),
                        new FunctionSystem.FunctionTemplate("admin-schedule","时间表管理","admin-admin-schedule",null,u=>u.HasAuthorityOrRoot(Authority.Admin)),
                        new FunctionSystem.FunctionTemplate("user-list","用户管理","admin-user-list",null,u=>u.HasAuthorityOrRoot(Authority.UserManage)),
                        new FunctionSystem.FunctionTemplate("student-list","学生管理","admin-student-list",null,u=>u.HasAuthorityOrRoot(Authority.StudentManage)),
                        new FunctionSystem.FunctionTemplate("teacher-list","教师管理","admin-teacher-list",null,u=>u.HasAuthorityOrRoot(Authority.TeacherManage)),
                        new FunctionSystem.FunctionTemplate("instructor-list","辅导员管理","admin-instructor-list",null,u=>u.HasAuthorityOrRoot(Authority.InstructorManage)),
                        new FunctionSystem.FunctionTemplate("course-list","课程管理","admin-course-list",null,u=>u.HasAuthorityOrRoot(Authority.CourseManage)),
                        new FunctionSystem.FunctionTemplate("class-list","班级管理","admin-class-list",null,u=>u.HasAuthorityOrRoot(Authority.ClassManage)),
                        new FunctionSystem.FunctionTemplate("classroom-list","教室管理","admin-classroom-list",null,u=>u.HasAuthorityOrRoot(Authority.ClassroomManage)),
                    })
                }
            );
        }
        private void InitPageForNavigator() {
            frameSys = new NavigatorSystem(Navigator, NavigatorBar, NavigatorBackBtn, Resources["NavigateListButton"] as Style, new PageFactory());
            frameSys.Navigated += (s, e) => { //完成一次导航。
                MainWindow.reference.setTitle(e.NowPage.Title);
            };
            frameSys.Goto("welcome");
        }
        public void InitPageForMessage() {
            msgSys = new MessageSystem(MsgBoxGrid);
        }

        private NavigatorPage() {
            InitializeComponent();
            User.Get().UpdateAuthority();
            User.SetApiConfig(User.Get().authentication);
            InitPageForNavigator();
            InitPageForCharacter();
            InitPageForMessage();
            //setCharacterList();
        }

        //导航维护部分。
        public NavigatorSystem frameSys { get; private set; }
        //功能维护部分。
        public FunctionSystem funcSys { get; private set; }
        //消息框部分。
        public MessageSystem msgSys { get; private set; }


        private void OptionBtn_Click(object sender, RoutedEventArgs e) {
            OptionMenu.Visibility = OptionMenu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Func_Schedule_Click(object sender, RoutedEventArgs e) {
            frameSys.Goto("basic-schedule");
        }
    }
    /// <summary>
    /// 维护消息框的类。
    /// </summary>
    public class MessageSystem {
        public MessageSystem(Grid paint) {
            uipaint = paint;
        }
        public void Show(MsgBox.MsgBoxEvent func, string title = "", string contenttext = "", Visibility?back = null, string[] buttonname = null) {
            var collections = boxes.Where(m => !m.BeingUsed);
            if (collections.Count() > 0) collections.First().Show(func, title, contenttext, Visibility.Visible, buttonname);
            else uipaint.Dispatcher.Invoke(() => {
                var nw = new MsgBox();
                nw.Show(func, title, contenttext, Visibility.Visible, buttonname);
                boxes.Add(nw);
                uipaint.Children.Add(nw);
            });
        }

        private Grid uipaint;
        private HashSet<MsgBox> boxes = new HashSet<MsgBox>();

    }
    /// <summary>
    /// 专门独立出来的维护左侧功能区的类。
    /// </summary>
    public class FunctionSystem {

        public FunctionSystem(NavigatorSystem navigatorsystem, Expander expander, StackPanel charactercontent, StackPanel functioncontent, Style characterstyle, Style functionstyle, CharacterTemplate[] template) {
            this.template = template;
            characterlist = charactercontent;
            functionlist = functioncontent;
            this.expander = expander;
            this.characterstyle = characterstyle;
            this.functionstyle = functionstyle;
            navigator_system = navigatorsystem;
            characterlist.Children.Clear();
            functionlist.Children.Clear();
            CreateElements();
        }
        private void CreateElements() {
            //首先按照最长长度生成职能与功能列表的最大程度数组，并进行默认填充。
            characterbutton = new Button[template.Length];
            for(int i = 0; i < characterbutton.Length; ++i) {
                var btn = new Button();
                btn.Style = characterstyle;
                btn.Visibility = Visibility.Collapsed;
                btn.Click += CharacterListButtonClick;
                btn.Content = template[i].title;
                characterbutton[i] = btn;
                characterlist.Children.Add(btn);
            }
            functionbutton = new Button[GetMaxLengthOfFunction()];
            for(int i = 0; i < functionbutton.Length; ++i) {
                var btn = new Button();
                btn.Style = functionstyle;
                btn.Visibility = Visibility.Collapsed;
                btn.Click += FunctionListButtonClick;
                functionbutton[i] = btn;
                functionlist.Children.Add(btn);
            }
            UpdateCharacter();
        }

        private void FunctionListButtonClick(object sender, RoutedEventArgs e) {
            var bindex = functionbutton.indexOf(sender as Button);
            //这样一来定位的character/function的位置就是select_character/bindex了。
            var cha = template[select_character];
            var fun = cha.function[bindex];
            //在执行之前再做一次可行性检查，character也要检查。
            if (cha.visible?.Invoke(User.Get()) ?? User.Get().HasAnyAuthority(cha.needauth)) {
                if (fun.visible?.Invoke(User.Get()) ?? true) {
                    //可以执行。
                    if (fun.customsize != null) fun.customsize(navigator_system);
                    else navigator_system.Goto(fun.pagetogo, fun.kwargstogo);
                }
            }
        }
        private void CharacterListButtonClick(object sender, RoutedEventArgs e) {
            var bindex = characterbutton.indexOf(sender as Button);
            SelectCharacter(bindex);
        }

        /// <summary>
        /// 更新一遍职能区的显示条件。
        /// </summary>
        public void UpdateCharacter() {
            var user = User.Get();
            for(int i = 0; i < template.Length; ++i) {
                var temp = template[i];
                //布尔判定：在visible自定义条件不为空时使用自定义，否则使用权限判断，判断是否存在任意一个权限。
                characterbutton[i].Visibility = (temp.visible?.Invoke(User.Get()) ?? user.HasAnyAuthority(temp.needauth)) ? Visibility.Visible : Visibility.Collapsed;
            }
            SelectCharacter(select_character);
        }
        /// <summary>
        /// 代码控制选择某一职能。如果当前职能不允许选择，自动跳转列表中第一个允许被选择的职能。
        /// </summary>
        /// <param name="name"></param>
        public void SelectCharacter(int index) {
            if (index >= 0 && index < template.Length && characterbutton[index].Visibility == Visibility.Visible) {
                select_character = index;
                var temp = template[index];
                expander.Header = string.Format("- {0} -", temp.title);
                //首先将functionbutton全部刷成不可见，
                for (int i = 0; i < functionbutton.Length; ++i)
                    functionbutton[i].Visibility = Visibility.Collapsed;
                //然后按照序列，仅修改并显示可用的项.
                for(int i = 0; i < temp.function.Length; ++i) {
                    var fun = temp.function[i];
                    if (fun.visible?.Invoke(User.Get()) ?? true) {
                        functionbutton[i].Content = fun.title;
                        functionbutton[i].Visibility = Visibility.Visible;
                    }
                }
            }else {
                for(int i=0;i<template.Length;++i)
                    if (characterbutton[i].Visibility == Visibility.Visible) {
                        SelectCharacter(i);
                        break;
                    }
            }
        }

        

        private int select_character;
        /// <summary>
        /// 包括了characterlist的扩展域。这个东西需要改名字。
        /// </summary>
        private Expander expander;
        /// <summary>
        /// 职能列表按钮风格与功能列表按钮风格。
        /// </summary>
        private Style characterstyle, functionstyle;
        /// <summary>
        /// 职能区与功能区.
        /// </summary>
        private StackPanel characterlist, functionlist;
        /// <summary>
        /// 对导航功能模块的引用。
        /// </summary>
        private NavigatorSystem navigator_system;
        /// <summary>
        /// 职能列表的按钮列表。
        /// </summary>
        private Button[] characterbutton;
        /// <summary>
        /// 功能列表的按钮列表。
        /// </summary>
        private Button[] functionbutton;
        /// <summary>
        /// 生成内容的模板与方法。
        /// </summary>
        private CharacterTemplate[] template;

        private int GetMaxLengthOfFunction() {
            int max = 0;
            foreach (var i in template)
                if (i.function.Length > max) max = i.function.Length;
            return max;
        }

        public struct CharacterTemplate {
            public CharacterTemplate(string name,string title, Func<User, bool> visible, FunctionTemplate[] function) {
                this.name = name;
                this.title = title;
                this.function = function;
                needauth = null;
                this.visible = visible;
            }
            public CharacterTemplate(string name, string title, Authority[] auth, FunctionTemplate[] function) {
                this.name = name;
                this.title = title;
                needauth = auth;
                this.function = function;
                visible = null;
            }
            public string name, title;
            /// <summary>
            /// 非自定义行为时，使用该权限列表判断是否显示本项。
            /// </summary>
            public Authority[] needauth;
            /// <summary>
            /// 下属功能列表。
            /// </summary>
            public FunctionTemplate[] function;
            /// <summary>
            /// 自定义的显示行为。当这一项不为null时以此为标准判断是否显示。
            /// </summary>
            public Func<User, bool> visible;

        }
        public struct FunctionTemplate {
            public FunctionTemplate(string name,string title, string page, IDictionary<string,object> kwargs = null, Func<User, bool> visible = null) {
                this.name = name;
                this.title = title;
                customsize = null;
                pagetogo = page;
                kwargstogo = kwargs;
                this.visible = visible;
            }
            public FunctionTemplate(string name,string title,Action<NavigatorSystem> customsize, Func<User, bool> visible = null) {
                this.name = name;
                this.title = title;
                this.customsize = customsize;
                pagetogo = null;
                kwargstogo = null;
                this.visible = visible;
            }
            /// <summary>
            /// 名称和标题。
            /// </summary>
            public string name, title;
            /// <summary>
            /// 定义是否显示的判定行为。
            /// </summary>
            public Func<User, bool> visible;
            /// <summary>
            /// 自定义的按钮行为。
            /// </summary>
            public Action<NavigatorSystem> customsize;
            /// <summary>
            /// 非自定义时，定义跳转的页面。
            /// </summary>
            public string pagetogo;
            /// <summary>
            /// 非自定义时，定义跳转页面的参数字典。
            /// </summary>
            public IDictionary<string, object> kwargstogo;
        }
    }

    /// <summary>
    /// 专门独立出来维护主界面导航栏的类。
    /// </summary>
    public class NavigatorSystem {

        public NavigatorSystem(Frame frame, StackPanel navigatorbar,Button backbutton, Style navigatorbuttonstyle, SimpleFactory<BasePage> factory) {
            this.frame = frame;
            this.navigatorbar = navigatorbar;
            this.backbutton = backbutton;
            this.navigatorbuttonstyle = navigatorbuttonstyle;
            this.factory = factory;
            backbutton.Click += Backbutton_Click;
            setBackButtonEnable();
            navigatorbar.Children.Clear();
        }
        /// <summary>
        /// 按下回退按钮所触发的事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Backbutton_Click(object sender, RoutedEventArgs e) {
            GoBack();
            setBackButtonEnable();
        }

        /// <summary>
        /// 使用的页面工厂。
        /// </summary>
        public SimpleFactory<BasePage> factory { get; private set; }
        /// <summary>
        /// 导航框架体。
        /// </summary>
        public Frame frame { get; private set; }
        /// <summary>
        /// 导航栏的引用。
        /// </summary>
        public StackPanel navigatorbar { get; private set; }
        /// <summary>
        /// 导航按钮的资源模板。
        /// </summary>
        private Style navigatorbuttonstyle { get; set; }
        /// <summary>
        /// 回退按钮的引用。
        /// </summary>
        public Button backbutton { get; private set; }

        /// <summary>
        /// 保存所有内含页的标识名和显示名的列表。
        /// </summary>
        private List<string> navigation = new List<string>();
        private List<string> navigation_name = new List<string>();
        /// <summary>
        /// 包含所有导航按钮的实例的列表。
        /// </summary>
        private List<Button> navigation_button = new List<Button>();
        /// <summary>
        /// 内含的正在使用的项的指针。
        /// </summary>
        private int index = -1;
        private BasePage now = null;

        /// <summary>
        /// 自动设置回退按钮是否可用。
        /// </summary>
        private void setBackButtonEnable() {
            backbutton.IsEnabled = CanGoBack();
        }

        /// <summary>
        /// 返回是否可以返回。
        /// </summary>
        /// <returns></returns>
        public bool CanGoBack() {
            return index > 0 && index <= navigation.Count;
        }
        /// <summary>
        /// 回退一页。
        /// </summary>
        /// <returns></returns>
        public bool GoBack() {
            return Goto(index - 1);
        }
        /// <summary>
        /// 跳转到指定栈序号中的页。如果该页不存在，那么返回false。在历史记录中跳转不会影响现有历史记录及其排序。
        /// </summary>
        /// <param name="goal"></param>
        /// <returns></returns>
        public bool Goto(int goal) {
            if (goal < 0 || goal >= navigation.Count) return false;
            var before = now;
            var after = factory.Create(navigation[index = goal]);
            if (after != null) {
                if (!after.NavigatingToEvent(this, null)) return false;
                var args = new NavigationEventArgs(before, after);
                Navigating?.Invoke(this, args);
                before?.NavigatingFromEvent(this, null);

                frame.NavigationService.Content = after;
                after.NavigatedToEvent(this, null);
                index = goal;
                now = after;
                //更新名称列表中的名称。
                UpdateName_One(goal, now.Title);
                Navigated?.Invoke(this, args);
                setBackButtonEnable();
                return true;
            } else return false;
        }
        /// <summary>
        /// 跳转到具有指定代号的新页面。这将会将新页面压栈，并影响现有历史记录。
        /// </summary>
        /// <param name="name">页面的标识名称.</param>
        /// <param name="kwargs">要跳转到某个页面时，向页面传递的请求参数。</param>
        /// <returns></returns>
        public bool Goto(string name, IDictionary<string,object> kwargs = null) {
            var before = now;
            var after = factory.Create(name);
            //此处涉及历史记录的覆盖逻辑：新的页面总是从当前页面位号的后一位开始覆盖。
            if (after != null) {
                var args = new NavigationEventArgs(before, after);
                if (!after.NavigatingToEvent(this, kwargs)) return false;//如果该事件返回了false，表示取消本次导航操作。
                if(index >= 0 && index+1 < navigation.Count) {
                    navigation.RemoveRange(index+1, navigation.Count - index - 1);//将从before开始的所有项目全部删除。
                    navigation_name.RemoveRange(index+1, navigation_name.Count - index - 1);
                }
                if (navigation.Contains(name)) {
                    var name_index = navigation.IndexOf(name);
                    navigation.RemoveRange(name_index, navigation.Count - name_index);//如果已有当前项目，那么从该项目开始的所有项目全部删除。
                    navigation_name.RemoveRange(name_index, navigation_name.Count - name_index);
                }
                Navigating?.Invoke(this, args);
                before?.NavigatingFromEvent(this, null);
                
                frame.NavigationService.Content = after;
                after.NavigatedToEvent(this, kwargs);
                navigation.Add(name);
                navigation_name.Add(after.Title);
                index = navigation.IndexOf(name);
                now = after;
                UpdateName_All();
                Navigated?.Invoke(this, args);
                setBackButtonEnable();
                return true;
            } else return false;
        }
        /// <summary>
        /// 点击了一个导航按钮。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationButtonClick(object sender, RoutedEventArgs e) {
            var th = sender as Button;
            var search = navigation_button.IndexOf(th);
            if (search >= 0 && search < navigation_button.Count) {
                Goto(search);
            }
        }
        /// <summary>
        /// 集成的更新方法，仅更新列表中的单个项目并刷新到显示。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        private bool UpdateName_One(int index, string name) {
            if (index < 0 || index >= navigation_name.Count) return false;
            navigation_name[index] = name;
            //总是假设btn实例已经与namelist对应，仅需要更新goal。
            navigation_button[index].Content = name;
            navigation_button[index].Visibility = Visibility.Visible;
            return true;
        }
        /// <summary>
        /// 更新的通用方法，获取namelist列表并完全刷新所有的显示。
        /// </summary>
        private void UpdateName_All() {
            //按钮的显示使用隐藏逻辑，即不使用的按钮进行隐藏而不是删除。
            for(int i = navigation_button.Count; i < navigation_name.Count; ++i) {//如果name的数量更多，需要增加新的button。
                var exe = new Button();
                exe.Click += NavigationButtonClick;
                exe.Style = navigatorbuttonstyle;
                navigatorbar.Children.Add(exe);
                navigation_button.Add(exe);
            }
            for(int i = 0; i < navigation_name.Count; ++i) {//这些是需要显示的button。
                navigation_button[i].Visibility = Visibility.Visible;
                navigation_button[i].Content = navigation_name[i];
            }
            for(int i = navigation_name.Count; i < navigation_button.Count; ++i) {//这些是需要隐藏的button。
                navigation_button[i].Visibility = Visibility.Collapsed;
            }
        }

        public delegate void NavigationEventHandler(object sender, NavigationEventArgs e);

        /// <summary>
        /// 导航之前与导航完成的事件。
        /// </summary>
        public event NavigationEventHandler Navigating, Navigated;

        public class NavigationEventArgs : EventArgs {
            public readonly BasePage NowPage, BeforePage;
            public NavigationEventArgs(BasePage BeforePage, BasePage NowPage) {
                this.NowPage = NowPage;
                this.BeforePage = BeforePage;
            }
        }
    }
}
