using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Core.Extensions;
using Business.Constants;

namespace Business.BusinessAspects.Autofac
{
    public class SecuredOperation : MethodInterception
    {
        private string[] _roles;
        private IHttpContextAccessor _httpContextAccessor;

        public SecuredOperation(string roles)
        {
            _roles = roles.Split(',');
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            // Aspect de injection olmadığı için Core.Utilities.IoC altında
            // ServiceTool oluşturuldu. buradaki GetService sayesinde generic olarak
            // gönderilen servis Autofac ile yapılan injectiondaki değerleri alır
            // bu sayede servisler winform yada wpf'de de kullanılabiir.
            // GetService gelmesi için Microsoft.Extensions.DependencyInjection paket
            // olarak nugetten ekleniyor aşağıdaki iki using buraya ekleniyor
            // using Castle.DynamicProxy;
            // using Microsoft.Extensions.DependencyInjection;
        }

        protected override void OnBefore(IInvocation invocation)
        {
            var roleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles();
            foreach (var role in _roles)
            {
                if (roleClaims.Contains(role))
                {
                    return;
                }
            }
            throw new Exception(Messages.AuthorizationDenied);
        }
    }
}
