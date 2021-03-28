using System.Collections.Generic;
using System.Linq;
using ReadABit.Core.Commands;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Contracts
{
    // TODO: Consider merging this with word definition view model in some way
    // so the client can load and cache all familiarities and definitions on init.
    public record WordFamiliarityListViewModel
    {
        /// <summary>
        /// <see cref="WordFamiliarityListItemViewModel" /> grouped by <see cref="Word.LanguageCode" /> and then keyed by <see cref="Word.Expression" />.
        /// </summary>
        public Dictionary<string, Dictionary<string, WordFamiliarityListItemViewModel>> GroupedWordFamiliarities { get; set; } = new();

        public List<WordFamiliarityListItemViewModel> Flatten()
        {
            return GroupedWordFamiliarities
                .Select(x => x.Value)
                .SelectMany(x => x.Values)
                .ToList();
        }
    }

    public record WordFamiliarityListItemViewModel
    {
        public WordSelector Word { get; init; } = new();
        public int Level { get; init; }
    }
}
