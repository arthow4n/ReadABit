using System.Linq;
using System.Globalization;
using System.Collections.Generic;
namespace ReadABit.Core.Utils
{
    public static class LanguageCode
    {
        private static readonly HashSet<string> s_validCodes = new(
            CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Select(c => c.TwoLetterISOLanguageName)
                .ToList()
        );

        public static bool IsValid(string code)
        {
            return s_validCodes.Contains(code);
        }
    }
}
