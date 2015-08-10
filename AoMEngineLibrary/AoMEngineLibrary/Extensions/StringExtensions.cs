namespace AoMEngineLibrary.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comparer)
        {
            return source.IndexOf(toCheck, comparer) >= 0;
        }
    }
}
