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
                { "basic-safety", new ProductionSingle<BasePage>().setProduction(typeof(Basic.safety))},
                { "basic-self-document", new ProductionSingle<BasePage>().setProduction(typeof(Basic.self_document))},
                { "basic-other-document", new ProductionSingle<BasePage>().setProduction(typeof(Basic.other_document))},
                { "basic-manager-document", new ProductionSingle<BasePage>().setProduction(typeof(Basic.manager_document))},

                { "student-self-document", new ProductionSingle<BasePage>().setProduction(typeof(Student.self_document))},
                { "student-other-document", new ProductionSingle<BasePage>().setProduction(typeof(Student.other_document))},
                { "student-manager-document", new ProductionSingle<BasePage>().setProduction(typeof(Student.manager_document))},
                { "student-self-attendances", new ProductionSingle<BasePage>().setProduction(typeof(Student.self_attendances))},

                { "teacher-self-document", new ProductionSingle<BasePage>().setProduction(typeof(Teacher.self_document))},
                { "teacher-other-document", new ProductionSingle<BasePage>().setProduction(typeof(Teacher.other_document))},
                { "teacher-manager-document", new ProductionSingle<BasePage>().setProduction(typeof(Teacher.manager_document))},

                { "instructor-self-document", new ProductionSingle<BasePage>().setProduction(typeof(Instructor.self_document))},
                { "instructor-other-document", new ProductionSingle<BasePage>().setProduction(typeof(Instructor.other_document))},
                { "instructor-manager-document", new ProductionSingle<BasePage>().setProduction(typeof(Instructor.manager_document))},

                { "office-self-document", new ProductionSingle<BasePage>().setProduction(typeof(Office.self_document))},
                { "office-self-students", new ProductionSingle<BasePage>().setProduction(typeof(Office.self_student))},
                { "office-self-teachers", new ProductionSingle<BasePage>().setProduction(typeof(Office.self_teacher))},
                { "office-self-instructors", new ProductionSingle<BasePage>().setProduction(typeof(Office.self_instructor))},

                { "generalmanager-classes", new ProductionSingle<BasePage>().setProduction(typeof(Generalmanager.class_list))},
                { "generalmanager-courses", new ProductionSingle<BasePage>().setProduction(typeof(Generalmanager.course_list))},

                { "admin-self-document", new ProductionSingle<BasePage>().setProduction(typeof(Admin.self_document))},
                { "admin-root-auth", new ProductionSingle<BasePage>().setProduction(typeof(Admin.root_auth))},
                { "admin-password", new ProductionSingle<BasePage>().setProduction(typeof(Admin.password))},
                { "admin-user-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.user_list))},
                { "admin-student-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.student_list))},
                { "admin-teacher-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.teacher_list))},
                { "admin-instructor-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.instructor_list))},
                { "admin-course-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.course_list))},
                { "admin-class-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.class_list))},
                { "admin-classroom-list",new ProductionSingle<BasePage>().setProduction(typeof(Admin.classroom_list))},
                { "admin-admin-schedule",new ProductionSingle<BasePage>().setProduction(typeof(Admin.admin_schedule))},
                { "admin-admin-schedule-instance",new ProductionSingle<BasePage>().setProduction(typeof(Admin.admin_schedule_instance))},

                { "course-instance",new ProductionSingle<BasePage>().setProduction(typeof(Course.instance))},
                { "course-modify",new ProductionSingle<BasePage>().setProduction(typeof(Course.modify))},
                { "course-detail",new ProductionSingle<BasePage>().setProduction(typeof(Course.detail))},

                { "class-instance",new ProductionSingle<BasePage>().setProduction(typeof(Class.instance))},
                { "class-modify",new ProductionSingle<BasePage>().setProduction(typeof(Class.modify))},
                { "class-detail",new ProductionSingle<BasePage>().setProduction(typeof(Class.detail))},

                { "classroom-instance",new ProductionSingle<BasePage>().setProduction(typeof(Classroom.instance))},
                { "classroom-modify",new ProductionSingle<BasePage>().setProduction(typeof(Classroom.modify))},
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
