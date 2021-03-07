﻿using System;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;

namespace ReadABit.Core.Commands
{
    public record WordDefinitionDelete : IRequest<bool>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid Id { get; set; }
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; set; }
    }
}