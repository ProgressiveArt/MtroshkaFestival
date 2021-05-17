using System.Linq;
using System.Reflection;
using Autofac;
using Interfaces.Core;

namespace MetroshkaFestival.Core
{
    public class CoreModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var domainTypes = new[]
            {
                typeof (IService),
                typeof (IFactory)
            };

            builder.RegisterAssemblyTypes(GetType().GetTypeInfo().Assembly)
                .Where(t => t.GetTypeInfo().ImplementedInterfaces.Intersect(domainTypes).Any())
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}