using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class StringUtilities
    {

        public static string FormatWith(this string format, IFormatProvider provider, object org0)
        {
            return format.FormatWith(provider, new object[] { org0 });
        }

        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            //DataValidate.ArgumentNotNull(format, "format");
            return string.Format(provider, format, args);
        }

        
    }
}
