using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using NUnit.Framework;

namespace Test.Ksu.Encryption
{
    [TestFixture]
    public class MachineKeyTests
    {
        [Test]
        public void BackAndForth()
        {
            var bytes = Encoding.UTF8.GetBytes("aldksfjaskdljf89023u54jrf");

            var cipherText = MachineKey.Protect(bytes);

            var plainText = MachineKey.Unprotect(cipherText);

            Assert.AreEqual(bytes, plainText);
        }

        [Test]
        public void SameHash()
        {
            var bytes = Encoding.UTF8.GetBytes("aldksfjaskdljf89023u54jrf");

            var cipherText = MachineKey.Protect(bytes);

            var plainText1 = MachineKey.Unprotect(cipherText);
            var plainText2 = MachineKey.Unprotect(cipherText);

            Assert.AreEqual(plainText1, plainText2);
        }

        [Test]
        public void Salt()
        {
            const string salt = "saltyhashbrowns";
            
            var bytes = Encoding.UTF8.GetBytes("aldksfjaskdljf89023u54jrf");

            var cipherText = MachineKey.Protect(bytes, "saltyhashbrowns");
            
            Assert.Throws<CryptographicException> (() => MachineKey.Unprotect(cipherText));
            Assert.AreEqual(bytes, MachineKey.Unprotect(cipherText, salt));
        }

        [Test]
        public void PersistentDecrypting()
        {
            var bytes = Encoding.UTF8.GetBytes("aldksfjaskdljf89023u54jrf");

            var cipherText = MachineKey.Protect(bytes);

            var decrypted1 = MachineKey.Unprotect(cipherText);
            var decrypted2 = MachineKey.Unprotect(cipherText);

            Assert.AreEqual(decrypted2, decrypted1);
        }
    }
}
