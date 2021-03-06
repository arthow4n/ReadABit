using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public record ArticleUpdate : IRequest<bool>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
    }

    public class ArticleUpdateValidator : AbstractValidator<ArticleUpdate>
    {
        public ArticleUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}
