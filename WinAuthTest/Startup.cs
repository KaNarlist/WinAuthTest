using Contracts;
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.IdentityModel.Policy;
using CoreWCF.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace WinAuthTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            var userPasswordBinding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
            userPasswordBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

            var ssoBinding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
            ssoBinding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            ssoBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            var srvCredentials = new ServiceCredentials();
            srvCredentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            srvCredentials.UserNameAuthentication.CustomUserNamePasswordValidator = new ServiceAuthenticator();



            app.UseHttpsRedirection();

            ServiceAuthorizationBehavior authBehavior = app.ApplicationServices.GetRequiredService<ServiceAuthorizationBehavior>();
            var authPolicies = new List<IAuthorizationPolicy>
            {
                new ServiceAuthorizationPolicy()
            };
            var externalAuthPolicies = new ReadOnlyCollection<IAuthorizationPolicy>(authPolicies);
            authBehavior.ExternalAuthorizationPolicies = externalAuthPolicies;
            //authBehavior.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

            app.UseServiceModel(builder =>
            {
                builder.ConfigureServiceHostBase<TestService>(host =>
                {
                    host.Description.Behaviors.Remove<ServiceCredentials>();
                    host.Description.Behaviors.Add(srvCredentials);
                });
                builder.AddService<TestService>().AddServiceEndpoint<TestService, ITestService>(userPasswordBinding, string.Format("/{0}", typeof(TestService).Name))
                                                 .AddServiceEndpoint<TestService, ITestService>(ssoBinding, string.Format("/{0}/sso", typeof(TestService).Name));
            });
        }
    }
}
