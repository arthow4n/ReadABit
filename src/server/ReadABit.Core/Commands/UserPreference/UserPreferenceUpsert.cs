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
        public UserPreferenceData Data { get; init; } = new();
    }

    public class UserPreferenceUpdateValidator : AbstractValidator<UserPreferenceUpsert>
    {
        public UserPreferenceUpdateValidator()
        {
            RuleFor(x => x.Data.WordDefinitionLanguageCode).MustBeValidLanguageCode();
        }
    }
}
