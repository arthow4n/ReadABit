using System;
using System.Collections.Generic;
using FluentValidation;

namespace ReadABit.Core.Commands
{
    public interface IPaginatedRequest
    {
        public PageFilter Page { get; init; }
    }

    /// <summary>
    /// Page filter for incoming requests.
    /// </summary>
    public record PageFilter
    {
        public int Index { get; init; }
        public int? Size { get; init; }

        public PageFilterFilled Fill(int defaultPageSize) => new()
        {
            Index = Index,
            Size = Size ?? defaultPageSize,
        };
    }

    public class PageFilterValidator : AbstractValidator<PageFilter>
    {
        public PageFilterValidator()
        {
            RuleFor(x => x.Index).GreaterThanOrEqualTo(1);
            RuleFor(x => x.Size).GreaterThanOrEqualTo(1).When(x => x.Size is not null);
            RuleFor(x => x.Size).LessThanOrEqualTo(100).When(x => x.Size is not null);
        }
    }

    public record Paginated<T>
    {
        public PageInfo Page { get; init; } = new PageInfo { };
        public List<T> Items { get; init; } = new List<T> { };
    }

    public record PageInfo
    {
        public PageFilterFilled Current { get; init; } = new PageFilterFilled { };
        public PageFilterFilled? Next { get; init; }
        public PageFilterFilled? Previous { get; init; }
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
    }

    /// <summary>
    /// Page filter for retured responses.
    /// </summary>
    public record PageFilterFilled : PageFilter
    {
        public new int Size { get; init; }
    }
}
