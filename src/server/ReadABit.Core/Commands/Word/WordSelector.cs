using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ReadABit.Core.Commands
{
    public record WordSelector
    {
        [BindRequired]
        public string LanguageCode { get; init; } = "";
        [BindRequired]
        public string Expression { get; init; } = "";
    }

    public class WordSelectorValidator : AbstractValidator<WordSelector>
    {
        public WordSelectorValidator()
        {
            RuleFor(x => x.LanguageCode).MustBeValidLanguageCode();
            RuleFor(x => x.Expression).NotEmpty();
        }
    }
}
