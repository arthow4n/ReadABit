
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Web.Contracts;
using Shouldly;

namespace ReadABit.Web.Test.Helpers
{
    public partial class TestBase
    {
        public async Task<ArticleViewModel> SetupArticle(
            string articleCollectionLanguageCode = "sv",
            string articleCollectionName = "collection",
            bool articleCollectionIsPublic = false,
            string articleName = "dummy",
            string articleText = "Hallå!",
            Guid? articleCollectionId = null
        )
        {
            var targetArticleCollectionId =
                articleCollectionId ??
                (await ArticleCollectionsController.Create(new()
                {
                    LanguageCode = articleCollectionLanguageCode,
                    Name = articleCollectionName,
                    Public = articleCollectionIsPublic,
                }))
                    .ShouldBeOfType<CreatedAtActionResult>()
                    .Value.ShouldBeOfType<ArticleCollectionViewModel>().Id;

            var name = articleName;
            var text = articleText;
            var creationResult =
                (await ArticlesController.Create(new()
                {
                    ArticleCollectionId = targetArticleCollectionId,
                    Name = name,
                    Text = text,
                }))
                .ShouldBeOfType<CreatedAtActionResult>();

            return creationResult.Value.ShouldBeOfType<ArticleViewModel>();
        }

        public async Task<WordFamiliarityUpsertBatchResultViewModal> SetupWordFamiliarity(int level, string languageCode, List<string> wordExpressions)
        {
            return (await WordFamiliaritiesController.UpsertBatch(new()
            {
                Level = level,
                Words = wordExpressions.ConvertAll(we => new WordSelector
                {
                    LanguageCode = languageCode,
                    Expression = we,
                })
            }))
                .ShouldBeOfType<OkObjectResult>()
                .Value
                .ShouldBeOfType<WordFamiliarityUpsertBatchResultViewModal>();
        }
    }
}
