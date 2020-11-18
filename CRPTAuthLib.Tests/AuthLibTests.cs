using CRPTAuthLib.Model;
using CRPTAuthLib.Utils;
using NUnit.Framework;
using System;

namespace CRPTAuthLib.AuthLibTests
{
    [TestFixture]
    public class AuthLibTests
    {
        AuthData data;
        [SetUp]
        public void Setup()
        {
            data = new AuthData();
        }

        [TestCase("")]
        [TestCase("invalid host input")]
        public void AuthLib_Reg_ReturnsError(string inputValue)
        {
            AuthLib authLib = new AuthLib();
            
            AuthData result = authLib.Reg(inputValue);

            Assert.IsFalse(string.IsNullOrEmpty(result.error_message));
        }

        [TestCase(null, "", "")]
        public void AuthLib_Auth_ReturnsError(AuthData authData, string queryString, string CPCertNumber)
        {
            AuthLib authLib = new AuthLib();
            
            AuthData result = authLib.Auth(authData, queryString, CPCertNumber);

            Assert.IsFalse(string.IsNullOrEmpty(result.error_message));
        }

        [TestCase("", "")]
        [TestCase("https://ya.ru", "")]
        [TestCase("random string", "")]
        [TestCase("random string", "random string")]
        public void AuthLib_Auth_ReturnsError(string queryString, string CPCertNumber)
        {
            AuthLib authLib = new AuthLib();
            AuthData data = new AuthData();
            data.data = "some data";

            AuthData result = authLib.Auth(data, queryString, CPCertNumber);

            Assert.IsFalse(string.IsNullOrEmpty(result.error_message));
        }

        [Test]
        public void AuthLib_Reg()
        {
            AuthLib authLib = new AuthLib();

            AuthData result = authLib.Reg("https://int01.gismt.crpt.tech/api/v3/true-api/auth/key");

            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.error_message));
        }

        [TestCase("01bab2c2000fac61b142d058308882ea99")]
        public void AuthLib_Auth(string sertNum)
        {
            AuthLib authLib = new AuthLib();

            AuthData result = authLib.Reg("https://int01.gismt.crpt.tech/api/v3/true-api/auth/key");
            result = authLib.Auth(result, "https://int01.gismt.crpt.tech/api/v3/true-api/auth/simpleSignIn", sertNum);

            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.error_message));
            Console.WriteLine(result.token);
        }
    }
}