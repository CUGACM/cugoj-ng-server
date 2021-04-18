using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cugoj_ng_server.Utilities
{
    public static class Crypto
    {
        public static byte[] SHA1(byte[] data)
        {
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            return sha1.ComputeHash(data);
        }
        public static byte[] MD5(byte[] data)
        {
            using var sha1 = System.Security.Cryptography.MD5.Create();
            return sha1.ComputeHash(data);
        }
    }
}
