using FluentValidation;

namespace ReadABit.Core.Commands
{
    public record WordSelector
    {
        public string LanguageCode { get; init; } = "";

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
