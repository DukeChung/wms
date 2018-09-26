using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection;
using Castle.Core.Logging;
using FortuneLab.ECService.Securities;
using FortuneLab.Repositories.Dapper;
using System.Reflection;

namespace FortuneLab.Security.AuthCenter
{
    public class AuthCenterModule : AbpModule
    {
        public ILogger Logger { get; set; }

        private readonly ITypeFinder _typeFinder;

        public AuthCenterModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
            Logger = NullLogger.Instance;

        }

        public override void Initialize()
        {
            //IocManager.Register<SimpleDbContextProvider<>>();
            //IocManager.IocContainer.Register(Component.For(typeof(SimpleDbContextProvider<>)).ImplementedBy(typeof(SimpleDbContextProvider<>)).LifestyleTransient());
            //IocManager.Register<AuthCenterRepository>();
            //IocManager.Register<SystemUserRepository>();
            //IocManager.Register<UserDeviceRepository>();
            IocManager.Register<IUserAppService, UserAppService>();
            RegisterGenericRepositories();
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        private void RegisterGenericRepositories()
        {
            var repositories =
                _typeFinder.Find(type =>
                    type.Assembly.Equals(Assembly.GetExecutingAssembly()) &&
                    type.IsPublic &&
                    !type.IsAbstract &&
                    type.IsClass &&
                    typeof(IDapperRepository).IsAssignableFrom(type)
                    );

            if (repositories.IsNullOrEmpty())
            {
                Logger.Warn("No class found derived from IDapperRepository.");
                return;
            }

            foreach (var repository in repositories)
            {
                if (!IocManager.IsRegistered(repository.GetType()))
                    IocManager.Register(repository);
            }
        }
    }
}
