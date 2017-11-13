using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore
{
    public static class Extensions
    {
        public static T GetService<T>(this IServiceProvider services) where T : class
        {
            return services.GetService(typeof(T)) as T;
        }
    }
}
