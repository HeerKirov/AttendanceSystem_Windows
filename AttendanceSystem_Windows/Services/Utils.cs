using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem_Windows.Services {
    public class Utils {
    }
    public static class StaticUtils {
        public static int indexOf<T>(this T[]th,T goal) {
            if(th != null) for (int i = 0; i < th.Length; ++i)
                if (th[i].Equals(goal)) return i;
            return -1;
        }
        /// <summary>
        /// 基于给定的条件，返回第一个序列中配对的内容的索引。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="th"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static int indexOf<T>(this T[]th,Func<T,bool> condition) {
            if (th != null && condition != null)
                for (int i = 0; i < th.Length; ++i)
                    if (condition(th[i])) return i;
            return -1;
        }
        /// <summary>
        /// 将指定枚举器的内容进行连接生成字符串。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="th"></param>
        /// <param name="middle"></param>
        /// <returns></returns>
        public static string join<T>(this IEnumerable<T> th,string middle = " ") {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach(var i in th) {
                if (first) first = false;
                else sb.Append(middle);
                sb.Append(i.ToString());
            }
            return sb.ToString();
        }

        public static IEnumerable<V> map<T,V>(this IEnumerable<T> th,Func<T,V> func) {
            var ans = new List<V>();
            foreach(var i in th) {
                ans.Add(func(i));
            }
            return ans.ToArray();
        }
        public static int firstAt<T>(this T[] th, Func<T,bool> func) {
            for (int i = 0; i < th.Length; ++i)
                if (func(th[i])) return i;
            return -1;
        }

        public static T let<T>(this T th,Action<T> action) {
            action(th);
            return th;
        }

        public static void forEach<T,V>(this IDictionary<T,V> th,Action<T,V> action) {
            foreach(var k in th) {
                action(k.Key, k.Value);
            }
        }

        public static void forEach<T>(this IEnumerable<T> th, Action<T> action) {
            foreach (var k in th) action(k);
        }

        public static T[] append<T>(this T[] th,T n) {
            var ans = new T[th.Length + 1];
            for (int i = 0; i < th.Length; ++i) ans[i] = th[i];
            ans[ans.Length - 1] = n;
            return ans;
        }
    }
}
