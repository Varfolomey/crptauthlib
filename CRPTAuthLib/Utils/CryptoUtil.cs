using CAdESCOM;
using CAPICOM;
using CRPTAuthLib.Utils;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CRPTAuthLib.Utils
{
    public class CryptoUtil
    {
        /// <summary>
        /// Signs data and returns obfuscated base64 string.
        /// </summary>
        /// <param name="dataToSign"></param>
        /// <param name="CPCertNumber">Personal certificate serial number.</param>
        /// <param name="detached">Set to true to sign documents.</param>
        /// <returns>Base64 signed result</returns>
        public static string Sign(string dataToSign, string CPCertNumber, bool detached = false)
        {
            if(string.IsNullOrEmpty(dataToSign))
            {
                throw new ArgumentNullException("Parameter \"dataToSign\" can't be empty!");
            }
            else if(string.IsNullOrEmpty(CPCertNumber))
            {
                throw new ArgumentNullException("Parameter \"CPCertNumber\" can't be empty!");
            }

            Store store;
            ICertificate cert = null;

            try
            {
                store = new Store();
                store.Open(CAPICOM_STORE_LOCATION.CAPICOM_CURRENT_USER_STORE, StoreName.My.ToString(), CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);


                foreach (ICertificate c in store.Certificates)
                {
                    if (c.SerialNumber.Equals(CPCertNumber.ToUpper()))
                    {
                        cert = c;
                        break;
                    }
                }

                if (cert == null)
                    throw new Exception("Can't find certificate with number " + CPCertNumber);
            }
            catch(COMException ex)
            {
                throw new Exception(ex.Message);
            }

            CPSigner signer = new CPSigner();
            signer.Certificate = cert;
            signer.TSAAddress = "http://qs.cryptopro.ru/tsp/tsp.srf";
            
            var signedData = new CadesSignedData();

            signedData.ContentEncoding = CADESCOM_CONTENT_ENCODING_TYPE.CADESCOM_STRING_TO_UCS2LE;
            signedData.Content = UTF8Encoding.UTF8.GetBytes(dataToSign);

            string signedResult;

            try
            {
                var sign = signedData.SignCades(signer, CADESCOM_CADES_TYPE.CADESCOM_CADES_BES, detached, CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BINARY);
                signedResult = Convert.ToBase64String(sign, Base64FormattingOptions.None);
            }
            catch (Exception e)
            {
                return e.Message + " " + e.StackTrace + " " + dataToSign;
            }

            return StringUtil.Obfuscate(signedResult);
        }
    }
}
