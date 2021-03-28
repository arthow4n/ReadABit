using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class WordFamiliarityListHandler : CommandHandlerBase, IRequestHandler<WordFamiliarityList, WordFamiliarityListViewModel>
    {
        public WordFamiliarityListHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<WordFamiliarityListViewModel> Handle(WordFamiliarityList request, CancellationToken cancellationToken)
        {
            var grouped = (await DB.WordFamiliaritiesOfUser(request.UserId)
                        .ProjectTo<WordFamiliarityListItemViewModel>(Mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken))
                        .GroupBy(wf => wf.Word.LanguageCode)
                        .ToDictionary(
                            wfg => wfg.Key,
                            wfg => wfg.ToDictionary(wf => wf.Word.Expression)
                        );

            return new WordFamiliarityListViewModel
            {
                GroupedWordFamiliarities = grouped,
            };
        }
    }
}
