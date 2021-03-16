using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReadABit.Infrastructure.Models;
using ReadABit.Core.Utils;
using ReadABit.Core.Integrations.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using NodaTime;

namespace ReadABit.Core.Commands
{
    public class ArticleUpdateHandler : IRequestHandler<ArticleUpdate, bool>
    {
        private readonly DB _db;
        private readonly IClock _clock;

        public ArticleUpdateHandler(DB db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public async Task<bool> Handle(ArticleUpdate request, CancellationToken cancellationToken)
        {
            new ArticleUpdateValidator().ValidateAndThrow(request);

            var article = await _db.ArticlesOfUser(request.UserId)
                                   .Where(a => a.Id == request.Id)
                                   .Select(a => new
                                   {
                                       Article = a,
                                       a.ArticleCollection,
                                   })
                                   .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (article is null)
            {
                return false;
            }

            article.Article.Name = request.Name is not null ? request.Name.Trim() : article.Article.Name;
            article.Article.Text = request.Text ?? article.Article.Text;
            article.Article.ConlluDocument =
                request.Text is null ?
                    article.Article.ConlluDocument :
                    UDPipeV1Service.ToConlluDocument(article.ArticleCollection.LanguageCode, request.Text);

            article.ArticleCollection.UpdatedAt = _clock.GetCurrentInstant();

            return true;
        }
    }
}
