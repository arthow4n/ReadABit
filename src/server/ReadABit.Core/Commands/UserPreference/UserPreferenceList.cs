using System;
using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record UserPreferenceList : IRequest<List<UserPreference>>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
    }
}
