using System;
using System.Text.Json.Serialization;
using MediatR;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record UserPreferenceGet : IRequest<UserPreferenceData>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
    }
}
