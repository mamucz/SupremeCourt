using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Infrastructure.Services
{
    public class SimplePasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
