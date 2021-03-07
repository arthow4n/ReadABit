using System.Data;
using FluentValidation;

namespace ReadABit.Core.Commands
{
    public record WordSelector
    {
        public string LanguageCode { get; set; } = "";

        public string Expression { get; set; } = "";
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
