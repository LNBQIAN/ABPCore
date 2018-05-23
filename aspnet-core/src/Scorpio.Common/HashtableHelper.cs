using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class HashtableHelper
    {
        public static int GetStringHash(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException("s");
            }

            var bytes = Encoding.ASCII.GetBytes(s);
            int checksum = GetBytesHash(bytes, 0, bytes.Length);
            return checksum;
        }

        public static int GetBytesHash(byte[] array, int ibStart, int cbSize)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentNullException("array");
            }

            int checksum = 0;
            for (int i = ibStart; i < (ibStart + cbSize); i++)
            {
                checksum = (checksum * 131) + array[i];
            }
            return checksum;
        }

        public static int GetBytesHash(char[] array, int ibStart, int cbSize)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentNullException("array");
            }

            int checksum = 0;
            for (int i = ibStart; i < (ibStart + cbSize); i++)
            {
                checksum = (checksum * 131) + array[i];
            }
            return checksum;
        }
    }
}
