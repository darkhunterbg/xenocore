using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Editor.Services
{
    public class AppServiceProvider : IServiceProvider
    {
        private Dictionary<Type, Object> services = new Dictionary<Type, object>();

        public object GetService(Type serviceType)
        {
            return services[serviceType];
        }

        public void AddService<T>(T service) where T : class
        {
            this.services.Add(typeof(T), service);
        }
    }
}
