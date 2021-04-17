using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ReadABit.Core.Utils
{
    public static class LanguageCode
    {
        private static readonly HashSet<string> s_validCodes = new(
            CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                // RFC 4646 IETF language tag
                .Select(c => c.Name)
                .ToList()
        );

        public static bool IsValid(string code)
        {
            return s_validCodes.Contains(code);
        }
    }
}
