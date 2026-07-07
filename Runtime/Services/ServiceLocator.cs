using System;
using System.Collections.Generic;

namespace CatAnnaDev.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>(32);

        public static int Count
        {
            get { return services.Count; }
        }

        public static void Register<T>(T service) where T : class
        {
            if (service == null)
            {
                CatLog.Error("ServiceLocator.Register called with a null service for type " + typeof(T).Name + ".");
                return;
            }

            Type key = typeof(T);
            if (services.ContainsKey(key))
            {
                CatLog.Warn("ServiceLocator already contains a service of type " + key.Name + ". Overwriting.");

                object previous = services[key];
                IService previousLifecycle = previous as IService;
                if (previousLifecycle != null)
                {
                    previousLifecycle.OnUnregistered();
                }
            }

            services[key] = service;

            IService lifecycle = service as IService;
            if (lifecycle != null)
            {
                lifecycle.OnRegistered();
            }
        }

        public static T Get<T>() where T : class
        {
            Type key = typeof(T);
            object found;
            if (services.TryGetValue(key, out found))
            {
                return (T)found;
            }

            CatLog.Error("ServiceLocator has no registered service of type " + key.Name + ".");
            return null;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            Type key = typeof(T);
            object found;
            if (services.TryGetValue(key, out found))
            {
                service = (T)found;
                return true;
            }

            service = null;
            return false;
        }

        public static bool IsRegistered<T>() where T : class
        {
            return services.ContainsKey(typeof(T));
        }

        public static bool Unregister<T>() where T : class
        {
            Type key = typeof(T);
            object existing;
            if (!services.TryGetValue(key, out existing))
            {
                return false;
            }

            services.Remove(key);

            IService lifecycle = existing as IService;
            if (lifecycle != null)
            {
                lifecycle.OnUnregistered();
            }

            return true;
        }

        public static void Clear()
        {
            foreach (KeyValuePair<Type, object> pair in services)
            {
                IService lifecycle = pair.Value as IService;
                if (lifecycle != null)
                {
                    lifecycle.OnUnregistered();
                }
            }

            services.Clear();
        }
    }
}
