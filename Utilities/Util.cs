using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QCUtilities
{
    public static class Util
    {
        public static string Capitalize(this string input)
        {
            // eww
            return input[0].ToString().ToUpper() + input.Substring(1);
        }
    }
}
