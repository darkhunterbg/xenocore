using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine
{
    public static class ServiceProvider
    {
        private static Dictionary<Type, Object> services = new Dictionary<Type, object>();
        private static Stack<IDisposable> disposableServices = new Stack<IDisposable>();

        public static void Add<T>(T service) where T : class
        {
            services.Add(typeof(T), service);
            if (service is IDisposable)
                disposableServices.Push(service as IDisposable);
        }
        public static T Get<T>() where T : class
        {
            Object service;
            Debug.Assert(services.TryGetValue(typeof(T), out service), $"Service {typeof(T)} was not found!");

            return service as T;
        }

        public static void Dispose()
        {
            while (disposableServices.Count > 0)
                disposableServices.Pop().Dispose();

            services.Clear();
        }
    }
}
