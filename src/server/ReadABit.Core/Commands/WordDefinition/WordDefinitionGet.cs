﻿using System;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record WordDefinitionGet : IRequest<WordDefinition?>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid Id { get; init; }
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
    }
}
