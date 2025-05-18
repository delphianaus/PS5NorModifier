using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS5_NOR_Modifier
{
    public static class Extensions
    {
        public static string GetCalculateChecksum(this string str)
        {
            int sum = 0;
            foreach (char c in str)
            {
                sum += (int)c;
            }
            return str + ":" + (sum & 0xFF).ToString("X2");
        }

        public static IEnumerable<int> GetPatternAt(this byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }

        public static byte[] HexStringToByteArray(this string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

        public static bool IsDiscEdition(this string str)
        {
            return str == "Disc Edition";
        }
        public static bool IsDigitalEdition(this string str)
        {
            return str == "Digital Edition";
        }
        public static string HexStringToString(this string hexString)
        {
            if (hexString == null || (hexString.Length & 1) == 1)
            {
                throw new ArgumentException();
            }
            var sb = new StringBuilder();
            for (var i = 0; i < hexString.Length; i += 2)
            {
                var hexChar = hexString.Substring(i, 2);
                sb.Append((char)Convert.ToByte(hexChar, 16));
            }
            return sb.ToString();
        }

    }
}
