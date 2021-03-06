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
                // This can actually fallback to 3 letters even if it's named as "two letter"
                // https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.twoletterisolanguagename
                .Select(c => c.TwoLetterISOLanguageName)
                .ToList()
        );

        public static bool IsValid(string code)
        {
            return s_validCodes.Contains(code);
        }
    }
}
