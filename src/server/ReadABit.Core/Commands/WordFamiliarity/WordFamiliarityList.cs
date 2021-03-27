using System;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Core.Contracts;

namespace ReadABit.Core.Commands
{
    public record WordFamiliarityList : IRequest<WordFamiliarityListViewModel>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
    }
}
