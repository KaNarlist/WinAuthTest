using Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

                Console.Write("Choose:\n1: Basic Auth\n2: Windows Auth\n>");
                var key = Console.ReadKey();
                ITestService testService = null;
                string url = "https://localhost:44304/TestService";

                if (key.KeyChar == '1')
                {
                    var pwdBinding = new WSHttpBinding();
                    pwdBinding.Security.Mode = SecurityMode.TransportWithMessageCredential;
                    pwdBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                    pwdBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

                    var wcfFacotory = new ChannelFactory<ITestService>(pwdBinding, new EndpointAddress(url));
                    wcfFacotory.Credentials.UserName.UserName = "user";
                    wcfFacotory.Credentials.UserName.Password = "pwd";
                    testService = wcfFacotory.CreateChannel();
                }
                else if (key.KeyChar == '2')
                {
                    var ssoBinding = new WSHttpBinding();
                    ssoBinding.Security.Mode = SecurityMode.TransportWithMessageCredential;
                    ssoBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                    ssoBinding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;

                    var wcfFacotory = new ChannelFactory<ITestService>(ssoBinding, new EndpointAddress(url + "/sso"));
                    //wcfFacotory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Identification;
                    testService = wcfFacotory.CreateChannel();
                }
                else
                {
                    Console.WriteLine("Wrong number!");
                    Console.ReadKey();
                    return;
                }

                testService.Test();
                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadKey();
        }
    }
}
