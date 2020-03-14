﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace WinDynamicDesktop
{
    class ProxyServer
    {
        private static string proxyAddress;
        private static string proxyCredentials;

        private static WebProxy GetProxy()
        {
            if (string.IsNullOrEmpty(JsonConfig.settings.webProxy))
            {
                return null;
            }

            if (proxyAddress == null)
            {
                proxyAddress = JsonConfig.settings.webProxy;

                if (proxyAddress.Contains('@'))
                {
                    string[] credentialsAndAddress = proxyAddress.Split(new char[] { '@' }, 2);
                    proxyCredentials = credentialsAndAddress[0];
                    proxyAddress = credentialsAndAddress[1];
                }
            }

            WebProxy proxy = new WebProxy(proxyAddress);
            
            if (proxyCredentials != null)
            {
                proxy.Credentials = GetCredentials();
            }

            return proxy;
        }

        private static ICredentials GetCredentials()
        {
            string[] userAndPass;
            string username;
            string password;

            if (!proxyCredentials.Contains(':'))
            {
                proxyCredentials = Encoding.UTF8.GetString(
                    Convert.FromBase64String(proxyCredentials));
            }

            userAndPass = proxyCredentials.Split(new char[] { ':' }, 2);
            username = userAndPass[0];
            password = userAndPass[1];

            return new NetworkCredential(username, password);
        }

        public static void ApplyProxyToClient(RestSharp.RestClient client)
        {
            WebProxy proxy = GetProxy();
            
            if (proxy != null)
            {
                client.Proxy = proxy;
            }
        }

        public static void ApplyProxyToClient(WebClient client)
        {
            WebProxy proxy = GetProxy();

            if (proxy != null)
            {
                client.Proxy = proxy;
            }
        }
    }
}
