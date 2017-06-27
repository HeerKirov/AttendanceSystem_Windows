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

namespace AttendanceSystem_Windows.PageFolder {
    /// <summary>
    /// 用于生成导航能力的基类页，自带导航事件。
    /// </summary>
    public abstract class BasePage : Page {
        public BasePage() { }

        /// <summary>
        /// 判别是否具有访问该页面的权限。
        /// </summary>
        /// <returns></returns>
        protected bool HasAuthority() {
            if (default_auth == null) return true;
            foreach(var i in default_auth) {
                if (i.relation_auth) {//关系型权限
                    if (User.Get().BelongQuery(i.type, i.id) == i.relation) return true;
                }else {//绝对权限
                    if (User.Get().HasAuthorityOrRoot(i.auth)) return true;
                }
            }
            return false;
        }
        protected AuthSetting[] default_auth;

        /// <summary>
        /// 当导航框架即将导航到本页面时发生。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="kwargs">传递的页面参数。</param>
        /// <returns>自定义是否允许本次导航。如果你返回了false，表示取消本次导航。</returns>
        public virtual bool NavigatingToEvent(object sender, IDictionary<string,object> kwargs) {
            return HasAuthority();
        }
        /// <summary>
        /// 导航框架已经导航到本页面完毕时发生。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="kwargs"></param>
        public virtual void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) { }
        /// <summary>
        /// 导航框架即将从本页面导航离开时发生。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public virtual void NavigatingFromEvent(object sender, object args) { }
    }
    public struct AuthSetting {
        public AuthSetting(Authority auth) {
            this.auth = auth;
            relation_auth = false;
            relation = default(BelongRelation);
            type = null;
            id = null;
        }
        public AuthSetting(BelongRelation relation, string type, string id) {
            this.relation = relation;
            this.type = type;
            this.id = id;
            relation_auth = true;
            auth = default(Authority);
        }
        public readonly bool relation_auth;
        public readonly Authority auth;
        public readonly BelongRelation relation;
        public readonly string type, id;
    }
}
