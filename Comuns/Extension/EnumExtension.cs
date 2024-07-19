using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Extension
{
    public static class EnumExtension
    {
        private static class EnumCache<T> where T : struct, Enum
        {
            public static readonly HashSet<T> Values = new((T[])Enum.GetValues(typeof(T)));
        }

        public static bool IsDefined<T>(this T value) where T : struct, Enum
        {
            return EnumCache<T>.Values.Contains(value);
        }
    }
}
