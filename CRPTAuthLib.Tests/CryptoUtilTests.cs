using CRPTAuthLib.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRPTAuthLib.Tests
{
    [TestFixture]
    class CryptoUtilTests
    {
        [TestCase("", "")]
        [TestCase("random string", "")]
        [TestCase("", "random string")]
        public void CryptoUtil_Sign_ThrowsArgumentNullException(string dataToSign, string CPCertNumber)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                CryptoUtil.Sign(dataToSign, CPCertNumber);
            });
        }

        [TestCase("random string", "random string")]
        public void CryptoUtil_Sign_ThrowsException(string dataToSign, string CPCertNumber)
        {
            Assert.Throws<Exception>(() =>
            {
                CryptoUtil.Sign(dataToSign, CPCertNumber);
            });
        }
    }
}
