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
        public Guid UserId { get; set; }
        [OpenApiIgnore, JsonIgnore]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Text { get; set; }
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
