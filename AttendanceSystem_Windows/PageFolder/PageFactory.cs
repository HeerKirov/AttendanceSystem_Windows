using AttendanceSystem_Windows.PageFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AttendanceSystem_Windows.PageFolder {
    public interface SimpleFactory<T> {
        T Create(string name);
    }
    /// <summary>
    /// 生产导航页面的简单工厂。
    /// </summary>
    public class PageFactory : SimpleFactory<BasePage> {
        public PageFactory() { }

        public BasePage Create(string name) {
            if (dict.ContainsKey(name))
                return dict[name].Create();
            else return null;
        }

        private IDictionary<string, IProductionMiddleware<BasePage>> dict = new Dictionary<string, IProductionMiddleware<BasePage>>() {
                { "welcome", new ProductionSingle<BasePage>().setProduction(typeof(WelcomePage)) },
                { "basic-schedule", new ProductionSingle<BasePage>().setProduction(typeof(Basic.schedule))},

                { "admin-self-document", new ProductionSingle<BasePage>().setProduction(typeof(Admin.self_document))},
                { "admin-root-auth", new ProductionSingle<BasePage>().setProduction(typeof(Admin.root_auth))},
                { "admin-user-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.user_list))},
                { "admin-student-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.student_list))},
                { "admin-teacher-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.teacher_list))},
                { "admin-instructor-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.instructor_list))},
                { "admin-course-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.course_list))},
                { "admin-class-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.class_list))},
                { "admin-classroom-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.classroom_list))},
                { "admin-admin-schedule",new ProductionSingle<BasePage>().setProduction(typeof(Admin.admin_schedule))},
                { "admin-admin-schedule-instance",new ProductionSingle<BasePage>().setProduction(typeof(Admin.admin_schedule_instance))},

            };
    }
    /// <summary>
    /// 简单工厂的生产中间件，用于控制产品的形成方式。
    /// </summary>
    public interface IProductionMiddleware<PRODUCT> {
        IProductionMiddleware<PRODUCT> setProduction(Type type);
        PRODUCT Create();
    }
    /// <summary>
    /// 实现单例模式的中间件。
    /// </summary>
    class ProductionSingle<T> : IProductionMiddleware<T> where T:class {
        public T Create() {
            var constructor = type.GetConstructor(new Type[] { });
            if (this.production == null) return this.production = constructor.Invoke(new object[] { }) as T;
            else return this.production;
        }

        public IProductionMiddleware<T> setProduction(Type type) {
            this.type = type;
            return this;
        }
        private Type type;
        private T production = null;
    }
}
