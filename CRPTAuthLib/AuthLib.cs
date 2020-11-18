using CRPTAuthLib.Model;
using CRPTAuthLib.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CRPTAuthLib
{
    public class AuthLib
    {
        /// <summary>
        /// Requests authorization data.
        /// </summary>
        /// <param name="queryString">[host]/auth/key</param>
        /// <returns></returns>
        public AuthData Reg(string queryString)
        {
            return RegAsync(queryString).Result;
        }

        /// <summary>
        /// Requests authorization data async.
        /// </summary>
        /// <param name="queryString">[host]/auth/key</param>
        /// <returns></returns>
        public async Task<AuthData> RegAsync(string queryString)
        {
            AuthData authData = null;

            if (string.IsNullOrEmpty(queryString))
            {
                authData = new AuthData();
                authData.error_message = "Parameter \"queryString\" can't be empty.";
                return authData;
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, queryString);

                request.Headers.Add("ContentType", "application/json;charset=UTF-8");

                var response = await new HttpClient().SendAsync(request);

                if (response != null)
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        TextReader reader = new StreamReader(responseStream);
                        authData = JsonConvert.DeserializeObject<AuthData>(reader.ReadToEnd());
                    }
                }
            }
            catch(InvalidOperationException)
            {
                authData = new AuthData();
                authData.error_message = "Parameter \"queryString\" is not a valid URI.";
                return authData;
            }
           
            return authData;
        }

        /// <summary>
        /// Receive session key.
        /// </summary>
        /// <param name="authData"></param>
        /// <param name="queryString">[host]/auth/simpleSignIn</param>
        /// <param name="CPCertNumber">Personal certificate serial number.</param>
        /// <returns></returns>
        public AuthData Auth(AuthData authData, string queryString, string CPCertNumber)
        {
            return AuthAsync(authData, queryString, CPCertNumber).Result;
        }

        /// <summary>
        /// Receive session key async.
        /// </summary>
        /// <param name="authData"></param>
        /// <param name="queryString">[host]/auth/simpleSignIn</param>
        /// <param name="CPCertNumber">Personal certificate serial number.</param>
        /// <returns></returns>
        public async Task<AuthData> AuthAsync(AuthData authData, string queryString, string CPCertNumber)
        {
            if(authData == null)
            {
                authData = new AuthData();
                authData.error_message = "Parameter \"authData\" can't be null. Use Reg method to generate correct AuthData.";
                return authData;
            }
            else if(string.IsNullOrEmpty(authData.data))
            {
                authData.error_message = "AuthData parameters are empty. Use Reg method first to generate correct AuthData.";
                return authData;
            }
            else if(string.IsNullOrEmpty(queryString))
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
            catch(Exception e)
            {
                authData.error_message = "Unable to sign data. " + e.Message;
                return authData;
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, queryString);

                request.Headers.Add("ContentType", "application/json;charset=UTF-8");
                request.Content = new StringContent(JsonConvert.SerializeObject(authData),
                    Encoding.UTF8, "application/json");

                var response = await new HttpClient().SendAsync(request);

                if (response != null)
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        TextReader reader = new StreamReader(responseStream);
                        authData = JsonConvert.DeserializeObject<AuthData>(reader.ReadToEnd());
                    }
                }
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
