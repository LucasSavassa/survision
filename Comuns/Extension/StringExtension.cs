using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Extension
{
    public static class StringExtension
    {
        public static DateTime GetStartFromSurgeryName(this string surgeryName)
        {
            return DateTime.ParseExact(surgeryName, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }
    }
}
