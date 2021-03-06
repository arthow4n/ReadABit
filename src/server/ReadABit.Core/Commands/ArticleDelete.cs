﻿using System;
using MediatR;
using Newtonsoft.Json;

namespace ReadABit.Core.Commands
{
    public record ArticleDelete : IRequest<bool>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
