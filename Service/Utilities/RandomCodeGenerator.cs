using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Service.Utilities
{
    public class RandomCodeGenerator : ICodeGenerator
    {
        // Kodda kullanılacak karakter kümesi:
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string Generate(int length)
        {
            // Cryptographically secure rastgele byte üretir
            var data = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);

            var result = new StringBuilder(length);
            foreach (var b in data)
            {
                // Her byte’ı, karakter kümesinin bir indeksine dönüştür
                result.Append(_chars[b % _chars.Length]);
            }

            return result.ToString();
        }
    }
}
