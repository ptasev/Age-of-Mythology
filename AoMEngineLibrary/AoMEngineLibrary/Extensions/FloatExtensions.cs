using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Extensions
{
    public static class FloatExtensions
    {
        public static string ToRoundTripString(this float f)
        {
            return f.ToString("F9", CultureInfo.InvariantCulture);
        }
    }
}
