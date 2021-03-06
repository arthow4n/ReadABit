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

namespace ReadABit.Core.Commands
{
    public class ArticleUpdateHandler : IRequestHandler<ArticleUpdate, bool>
    {
        private readonly DB _db;

        public ArticleUpdateHandler(DB db)
        {
            _db = db;
        }

        public async Task<bool> Handle(ArticleUpdate request, CancellationToken cancellationToken)
        {
            new ArticleUpdateValidator().ValidateAndThrow(request);

            var article = await _db.ArticlesOfUser(request.UserId)
                                   .Where(a => a.Id == request.Id)
                                   .Select(a => new
                                   {
                                       Article = a,
                                       a.ArticleCollection.LanguageCode,
                                   })
                                   .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            if (article is null)
            {
                return false;
            }


            article.Article.Name = request.Name;
            article.Article.Text = request.Text;
            article.Article.Conllu = UDPipeV1Service.ToConllu(article.LanguageCode, request.Text);

            return true;
        }
    }
}
