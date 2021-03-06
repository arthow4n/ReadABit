using FluentValidation;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public static class ValidationRuleExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValidLanguageCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(lc => LanguageCode.IsValid(lc))
                              .WithMessage("Language code must be a valid ISO 639 code, where ISO 639-1 should be preferred.");
        }
    }
}
