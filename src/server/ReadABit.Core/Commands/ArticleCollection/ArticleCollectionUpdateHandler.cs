﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Utils;
using EnsureThat;
using FluentValidation;

namespace ReadABit.Core.Commands
{
    public class ArticleCollectionUpdateHandler : IRequestHandler<ArticleCollectionUpdate, bool>
    {
        private readonly DB _db;

        public ArticleCollectionUpdateHandler(DB db)
        {
            _db = db;
        }

        public async Task<bool> Handle(ArticleCollectionUpdate request, CancellationToken cancellationToken)
        {
            new ArticleCollectionUpdateValidator().ValidateAndThrow(request);

            var articleCollection = await _db
                .ArticleCollectionsOfUser(request.UserId)
                .Where(ac => ac.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (articleCollection is null)
            {
                return false;
            }

            articleCollection.Name = request.Name is not null ? request.Name.Trim() : articleCollection.Name;
            articleCollection.LanguageCode = request.LanguageCode ?? articleCollection.LanguageCode;
            articleCollection.Public = request.Public ?? articleCollection.Public;

            return true;
        }
    }
}
