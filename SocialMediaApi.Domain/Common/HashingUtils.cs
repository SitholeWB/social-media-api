using System.Security.Cryptography;
using System.Text;

namespace SocialMediaApi.Domain.Common
{
    public struct HashingUtils
    {
        public static string HashUserPassword(Guid userId, string inputString)
        {
            var combined = new StringBuilder();
            combined.Append(userId.ToString().ToLowerInvariant());
            combined.Append('_');
            combined.Append(inputString);
            using var sha512 = SHA512.Create();
            using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(combined.ToString()));
            return BitConverter.ToString(sha512.ComputeHash(fileStream)).Replace("-", "");
        }
    }
}