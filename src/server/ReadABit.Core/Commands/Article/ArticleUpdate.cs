using System;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NSwag.Annotations;

namespace ReadABit.Core.Commands
{
    public record ArticleUpdate : IRequest<bool>
    {
        [OpenApiIgnore, JsonIgnore]
        public Guid UserId { get; init; }
        [OpenApiIgnore, JsonIgnore]
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Text { get; init; }
    }

    public class ArticleUpdateValidator : AbstractValidator<ArticleUpdate>
    {
        public ArticleUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().When(x => x.Name is not null);
            RuleFor(x => x.Text).NotEmpty().When(x => x.Text is not null);
        }
    }
}
