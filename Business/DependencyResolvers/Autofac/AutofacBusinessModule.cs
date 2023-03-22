using Autofac;
using Autofac.Extras.DynamicProxy;
using Business.Abstract;
using Business.CCS;
using Business.Concrete;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Core.Utilities.Sequrity.JWT;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //base.Load(builder);

            builder.RegisterType<ProductManager>().As<IProductService>();
            // webapi de startup da yazdığımız services.AddSingleton<IProductService, ProductManager>(); 
            // ile aynı işi yapıyor: IProductService istendiğinde ProductManager new()'liyor
            builder.RegisterType<EfProductDal>().As<IProductDal>();
            // burada alternatif servisler için if blokları vs. kullanılabilir
            // buradaki yapılandırmalır etkin olması için ilgili projelerdeki yapılandırmalara
            // dahil edilmesi gerekir: örneğin webapi de program.cs dosyası içinde
            // CreateDefaultBuilder den sonra aşağıdaki extension methodlar sırasıyla eklenir
            // .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            // .ConfigureContainer<ContainerBuilder>(builder => {
            //  builder.RegisterModule(new AutofacBusinessModule());
            //  })

            builder.RegisterType<CategoryManager>().As<ICategoryService>();
            builder.RegisterType<EfCategoryDal>().As<ICategoryDal>();

            builder.RegisterType<FileLogger>().As<ILogger>().SingleInstance();


            builder.RegisterType<UserManager>().As<IUserService>();
            builder.RegisterType<EfUserDal>().As<IUserDal>();

            builder.RegisterType<AuthManager>().As<IAuthService>();
            builder.RegisterType<JwtHelper>().As<ITokenHelper>();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions()
                {
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance();
        }
    }
}
