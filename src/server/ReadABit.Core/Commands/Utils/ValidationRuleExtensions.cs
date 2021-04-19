using FluentValidation;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public static class ValidationRuleExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValidLanguageCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(lc => LanguageCode.IsValid(lc))
                              .WithMessage("Must be a valid IETF BCP 47 language tag, where the shortest form should be preferred when available. For example: en.");
        }

        public static IRuleBuilder<T, string> MustBeSupportedTimeZone<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => x.TryParseToDateTimeZone() is not null)
                .WithMessage("Must be a valid IANA time zone name. For example: Asia/Taipei.");
        }

        public static IRuleBuilder<T, string> MustBeIsoHHmmss<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => x.TryParseIsoHhmmssToLocalTime().Success)
                .WithMessage("Must be a valid ISO 8601 hh:mm:ss time. For example: 23:59:59.");
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
