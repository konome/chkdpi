using System.Text;

namespace Konome
{
    public static class Base64
    {
        public static string EncodeToBase64(string s)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(s));

        public static string DecodeStringFromBase64(string s)
            => Encoding.UTF8.GetString(Convert.FromBase64String(s));
    }
}
