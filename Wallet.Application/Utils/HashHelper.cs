using System.Security.Cryptography;
using System.Text;

namespace Wallet.Utils
{
    public static class HashHelper
    {
        public static byte[] ComputeSha256Hash(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            }
        }
    }
}
