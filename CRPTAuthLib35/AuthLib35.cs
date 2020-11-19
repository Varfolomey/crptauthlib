using CRPTAuthLib35.Model;
using CRPTAuthLib35.Utils;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

namespace CRPTAuthLib35
{
    public class AuthLib35
    {
        public AuthData Reg(string queryString)
        {
            AuthData authData;

            if (string.IsNullOrEmpty(queryString))
            {
                authData = new AuthData();
                authData.error_message = "Parameter \"queryString\" can't be empty.";
                return authData;
            }

            try
            {
                WebClient webClient = new WebClient();
                webClient.Proxy.Credentials = CredentialCache.DefaultCredentials;
                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add("Content-Type", "application/json");
                webClient.BaseAddress = queryString;

                var response = webClient.DownloadString(queryString);
                authData = JsonConvert.DeserializeObject<AuthData>(response);
            }
            catch (ArgumentException)
            {
                authData = new AuthData();
                authData.error_message = "Parameter \"queryString\" is not a valid URI.";
                return authData;
            }

            return authData;
        }

        public AuthData Auth(AuthData authData, string queryString, string CPCertNumber)
        {
            if (authData == null)
            {
                authData = new AuthData();
                authData.error_message = "Parameter \"authData\" can't be null. Use Reg method to generate correct AuthData.";
                return authData;
            }
            else if (string.IsNullOrEmpty(authData.data))
            {
                authData.error_message = "AuthData parameters are empty. Use Reg method first to generate correct AuthData.";
                return authData;
            }
            else if (string.IsNullOrEmpty(queryString))
            {
                authData.error_message = "Parameter \"queryString\" can't be empty.";
                return authData;
            }
            else if (string.IsNullOrEmpty(CPCertNumber))
            {
                authData.error_message = "Parameter \"CPCertNumber\" can't be empty.";
                return authData;
            }

            try
            {
                authData.data = CryptoUtil.Sign(authData.data, CPCertNumber);
            }
            catch (Exception e)
            {
                authData.error_message = "Unable to sign data. " + e.Message;
                return authData;
            }

            try
            {
                WebClient webClient = new WebClient();
                webClient.Proxy.Credentials = CredentialCache.DefaultCredentials;
                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add("Content-Type", "application/json");
                webClient.BaseAddress = queryString;

                var response = webClient.UploadString(queryString, "POST", JsonConvert.SerializeObject(authData));
                authData = JsonConvert.DeserializeObject<AuthData>(response);
            }
            catch (InvalidOperationException)
            {
                authData.error_message = "Parameter \"queryString\" is not a valid URI.";
                return authData;
            }

            return authData;
        }
    }
}
