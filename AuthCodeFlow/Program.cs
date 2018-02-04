using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Firefox;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.EventArguments;

namespace AuthCodeFlow
{
    class Program
    {
        //private static string appBaseAddress = "https://si-tridentapi-test.azurewebsites.net/api";
        //private static string redirectUri = "https://login.live.com/oauth20_desktop.srf";
        //private static string userName = "digitalmouse@somarkinnovations.com";

        private static string authUri;

        static void Main(string[] args)
        {
            var authTest = new AuthTest();
            authUri = authTest.GetAuthCodeURL().GetAwaiter().GetResult().ToString();
            authTest.OpenBrowser(authUri);  
        }

       
    }

    public class AuthTest
    {
        private static string tenant = "somarkinnovations.com";
        private static string resource = "https://somarkinnovations.com/c5213d40-0e4e-4284-be31-02e9c5bdf538";
        private static string clientId = "61ddf9a5-bbb4-42d5-82ba-13ef2112d3b6";
        const string authority = "https://login.microsoftonline.com/somarkinnovations.com";
        private readonly AuthenticationContext authContext;

        //proxy server
        private ProxyServer _proxyserver;


        public void InitializeProxy()
        {
            _proxyserver = new ProxyServer
            {
                TrustRootCertificate = true
            };

            var explicitEndPoint = new ExplicitProxyEndPoint(System.Net.IPAddress.Any, 54777, true);
            _proxyserver.AddEndPoint(explicitEndPoint);
            _proxyserver.Start();
            _proxyserver.SetAsSystemHttpProxy(explicitEndPoint);
            _proxyserver.SetAsSystemHttpsProxy(explicitEndPoint);
            _proxyserver.BeforeResponse += OnResponseCaptureTrafficEventHandler;

        }

        //Event handler
        private async Task OnResponseCaptureTrafficEventHandler(object arg1, SessionEventArgs arg2)
        {
            
        }

        
      

        public AuthTest()
        {
            authContext = new AuthenticationContext(authority);
        }
        public async Task<Uri> GetAuthCodeURL()
        {
            Uri redirectUri = new Uri("https://login.live.com/oauth20_desktop.srf");

            //Uri redirectUri = new Uri("urn:ietf:wg:oauth:2.0:oob");


            UserIdentifier userId = new UserIdentifier("digitalmouse@somarkinnovations.com", UserIdentifierType.RequiredDisplayableId);

            var authCodeURL = await authContext.GetAuthorizationRequestUrlAsync(resource, clientId, redirectUri, userId, null);
            return authCodeURL;
        }

        public void OpenBrowser(string url)
        {
            var proxy = new OpenQA.Selenium.Proxy
            {
                HttpProxy = "http://127.0.0.1:54777",
                SslProxy = "http://127.0.0.1:54777"
            };

            var options = new FirefoxOptions
            {
                Proxy = proxy
            };

            IWebDriver driver = new FirefoxDriver(options);
            driver.Navigate().GoToUrl(url);

            var passwdField = driver.FindElement(By.Name("passwd"));
            passwdField.SendKeys("sensaLab2");

            var submitBtn = driver.FindElement(By.Id("idSIButton9"));
            submitBtn.Click();

            


        }

    }
}
