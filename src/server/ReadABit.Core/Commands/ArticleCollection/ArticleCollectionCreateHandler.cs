using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using NodaTime;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionCreateHandler : IRequestHandler<ArticleCollectionCreate, ArticleCollectionViewModel>
    {
        private readonly DB _db;
        private readonly IClock _clock;
        private readonly IMapper _mapper;

        public ArticleCollectionCreateHandler(DB db, IClock clock, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
            _clock = clock;
        }

        public async Task<ArticleCollectionViewModel> Handle(ArticleCollectionCreate request, CancellationToken cancellationToken)
        {
            new ArticleCollectionCreateValidator().ValidateAndThrow(request);

            var articleCollection = new ArticleCollection
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Name.Trim(),
                LanguageCode = request.LanguageCode,
                Public = request.Public,
                CreatedAt = _clock.GetCurrentInstant(),
                UpdatedAt = _clock.GetCurrentInstant(),
            };

            await _db.Unsafe.AddAsync(articleCollection, cancellationToken);

            return _mapper.Map<ArticleCollectionViewModel>(articleCollection);
        }
    }
}
