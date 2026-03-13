using System;
using System.Collections.Generic;
using System.Text;

namespace KtaneCustomKeys
{
    public static class KtaneCustomKeysHolster
    {
        private static readonly Dictionary<string, Hold> s_storage = [];

        private static readonly Random s_random = new();

        private static readonly char[] s_tokenChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();

        private static string CreateCode()
        {
            lock (s_storage)
            {
                var builder = new StringBuilder();
                for (var i = 0; i < 5; i++)
                    builder.Append(s_random.Next(10));

                var code = builder.ToString();
                if (s_storage.ContainsKey(code))
                    return CreateCode();

                return code;
            }
        }

        private static string CreateToken()
        {
            lock (s_storage)
            {
                var builder = new StringBuilder();
                for (var i = 0; i < 15; i++)
                    builder.Append(s_tokenChars[s_random.Next(s_tokenChars.Length)]);

                return builder.ToString();
            }
        }

        public static string Push(string data)
        {
            lock (s_storage)
            {
                var hold = new Hold(data);
                s_storage.Add(hold.Code, hold);
                return $"{hold.Code}|{hold.Token}";
            }
        }

        public static string Pull(string code)
        {
            lock (s_storage)
                return s_storage.TryGetValue(code, out var result) ? result.Data : null;
        }

        public static bool? Remove(string code, string token)
        {
            lock (s_storage)
            {
                if (!s_storage.TryGetValue(code, out var hold))
                    return false;
                if (hold.Token != token)
                    return null;
                s_storage.Remove(code);
                return true;
            }
        }

        private class Hold(string data)
        {
            public string Code { get; } = CreateCode();
            public string Data { get; } = data;
            public string Token { get; } = CreateToken();
        }
    }
}