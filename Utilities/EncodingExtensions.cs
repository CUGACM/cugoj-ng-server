using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cugoj_ng_server.Utilities
{
    public static class EncodingExtensions
    {
        public static byte[] Base64Decode(this string str) => Convert.FromBase64String(str);
        public static string Base64Encode(this byte[] data) => Convert.ToBase64String(data);
        public static byte[] HexDecode(this string str) => Convert.FromBase64String(str);
        public static string HexEncode(this byte[] data) => BitConverter.ToString(data).Replace("-", "").ToLower();
        public static byte[] Encode(this string str, Encoding encoding = null) => (encoding ?? Encoding.Default).GetBytes(str);
        public static string Decode(this byte[] data, Encoding encoding = null) => (encoding ?? Encoding.Default).GetString(data);

        public static byte[] Encode<T>(this T value) where T : struct
        {
            var arr = new byte[Marshal.SizeOf(value)];
            MemoryMarshal.Write(arr, ref value);
            return arr;
        }
        public static T Decode<T>(this byte[] arr) where T : struct
        {
            return MemoryMarshal.Read<T>(arr);
        }
    }
}
