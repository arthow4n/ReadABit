using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReadABit.Core.Commands.Utils;
using ReadABit.Core.Integrations.Services;
using ReadABit.Core.Utils;

namespace ReadABit.Core.Commands
{
    public class ArticleUpdateHandler : CommandHandlerBase, IRequestHandler<ArticleUpdate, bool>
    {
        public ArticleUpdateHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<bool> Handle(ArticleUpdate request, CancellationToken cancellationToken)
        {
            new ArticleUpdateValidator().ValidateAndThrow(request);

            var article = await DB.ArticlesOfUser(request.UserId)
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

            article.ArticleCollection.UpdatedAt = Clock.GetCurrentInstant();

            return true;
        }
    }
}
