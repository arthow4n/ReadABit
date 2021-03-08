using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record UserPreferenceUpsert : IRequest<bool>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        public UserPreferenceType Type { get; init; }
        public string Value { get; init; } = "";
    }

    public class UserPreferenceUpdateValidator : AbstractValidator<UserPreferenceUpsert>
    {
        public UserPreferenceUpdateValidator()
        {
            // TODO: Expand this when we add more preference types.
            RuleFor(x => x.Type).Must(t => t == UserPreferenceType.LanguageCode);
            RuleFor(x => x.Value).MustBeValidLanguageCode().When(x => x.Type == UserPreferenceType.LanguageCode);
        }
    }
}
