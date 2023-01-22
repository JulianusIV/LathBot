using DSharpPlus;
using LathBotBack.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LathBotBack.Base
{
    public class BaseService
    {
        public static void InitAll(DiscordClient client)
        {
            if (StartupService.Instance.InitCompleted)
                return;

            AppDomain domain = AppDomain.CurrentDomain;
            Assembly assembly = domain.GetAssemblies().Single(x => x.FullName.Contains("LathBotBack"));
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> services = types.Where(x => x.IsSubclassOf(typeof(BaseService)));
            foreach (Type service in services)
            {
                PropertyInfo property = service.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                BaseService serviceInstance = (BaseService)property.GetValue(null, null);
                serviceInstance.Init(client);
            }

            StartupService.Instance.InitCompleted = true;
        }

        public virtual void Init(DiscordClient client) { }
    }
}
