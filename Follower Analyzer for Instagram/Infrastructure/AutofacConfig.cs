using Autofac;
using Autofac.Integration.Mvc;
using Follower_Analyzer_for_Instagram.Models.DBInfrastructure;
using Follower_Analyzer_for_Instagram.Services.InstagramAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Follower_Analyzer_for_Instagram.Infrastructure
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            // получаем экземпляр контейнера
            ContainerBuilder builder = new ContainerBuilder();

            // регистрируем контроллер в текущей сборке
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // регистрируем споставление типов
            builder.RegisterType<FollowerAnalyzerRepository>().As<IRepository>();
            builder.RegisterType<InstagramAPI>().As<IInstagramAPI>();

            // создаем новый контейнер с теми зависимостями, которые определены выше
            IContainer container = builder.Build();

            // установка сопоставителя зависимостей
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}