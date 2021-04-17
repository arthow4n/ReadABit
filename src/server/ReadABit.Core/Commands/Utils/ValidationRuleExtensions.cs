using FluentValidation;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public static class ValidationRuleExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValidLanguageCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(lc => LanguageCode.IsValid(lc))
                              .WithMessage("Language code must be a valid IETF BCP 47 language tag, where the shortest form should be preferred when available.");
        }

        public static IRuleBuilderOptions<T, WordSelector> MustBeValidWordSelector<T>(this IRuleBuilder<T, WordSelector> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new WordSelectorValidator());
        }

        public static IRuleBuilderOptions<T, PageFilter> MustBeValidPageFilter<T>(this IRuleBuilder<T, PageFilter> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PageFilterValidator());
        }
    }
}
