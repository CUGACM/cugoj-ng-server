using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cugoj_ng_server.Utilities
{
    public static class Sequence
    {
        public static byte[] ByteArrayConcat(params byte[][] arrays)
        {
            var dstArr = new byte[arrays.Sum(x => x.Length)];
            var offset = 0;
            Array.ForEach(arrays, arr =>
            {
                Buffer.BlockCopy(arr, 0, dstArr, offset, arr.Length);
                offset += arr.Length;
            });
            return dstArr;
        }
    }
}
