using System.Reflection;
using Autofac;

namespace MetroshkaFestival.Application
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(GetType().GetTypeInfo().Assembly)
                .Where(t => t.GetTypeInfo().IsPublic)
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}