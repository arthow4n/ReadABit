using System;
using System.Collections.Generic;
using FluentValidation;

namespace ReadABit.Core.Commands
{
    public interface IPaginatedRequest
    {
        public PageFilter Page { get; set; }
    }

    /// <summary>
    /// Page filter for incoming requests.
    /// </summary>
    public record PageFilter
    {
        public int Index { get; set; }
        public int? Size { get; set; }

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

    public class Paginated<T>
    {
        public PageInfo Page { get; set; } = new PageInfo { };
        public List<T> Items { get; set; } = new List<T> { };
    }

    public record PageInfo
    {
        public PageFilterFilled Current { get; set; } = new PageFilterFilled { };
        public PageFilterFilled? Next { get; set; }
        public PageFilterFilled? Previous { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Page filter for retured responses.
    /// </summary>
    public record PageFilterFilled : PageFilter
    {
        public new int Size { get; set; }
    }
}
