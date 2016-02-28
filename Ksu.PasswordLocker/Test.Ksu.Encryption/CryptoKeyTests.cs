using Ksu.Encryption;
using NUnit.Framework;

namespace Test.Ksu.Encryption
{
    [TestFixture]
    public class CryptoKeyTests
    {
        private const string PassPhrase = "P@$$Ph4A$3";
        private const string PlainText = "JordanWoodman%$312349IIIn";

        [Test]
        public void SymmetricEncryption()
        {
            var cipherText = CryptoKey.Encrypt(PlainText, PassPhrase);

            var plainText = CryptoKey.Decrypt(cipherText, PassPhrase);

            Assert.AreEqual(plainText, PlainText);
        }
    }
}
