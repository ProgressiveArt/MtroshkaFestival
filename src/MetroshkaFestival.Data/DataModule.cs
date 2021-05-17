using System.Linq;
using System.Reflection;
using Autofac;
using Interfaces.Core;
using Interfaces.Data;
using MetroshkaFestival.Data.Authentication;
using Module = Autofac.Module;

namespace MetroshkaFestival.Data
{
    public class DataModule : Module
    {
        public DataModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly string _connectionString;

        protected override void Load(ContainerBuilder builder)
        {
            var domainTypes = new[]
            {
                typeof (IService),
                typeof (IFactory),
            };

            builder.RegisterAssemblyTypes(GetType().GetTypeInfo().Assembly)
                .Where(t => t.GetTypeInfo().ImplementedInterfaces.Intersect(domainTypes).Any())
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(DataContext))
                .As<DataContext>()
                .InstancePerLifetimeScope();

            builder.Register(x => new ApplicationContextManager(_connectionString))
                .As<IEfContextManager<DataContext>>()
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(ApplicationUserStore))
                .As<ApplicationUserStore>()
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(ApplicationRoleStore))
                .As<ApplicationRoleStore>()
                .InstancePerLifetimeScope();
        }
    }
}