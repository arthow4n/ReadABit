
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NodaTime.Text;
using ReadABit.Core.Contracts;
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
            string articleText = "Hallå!"
        )
        {
            var articleCollectionId =
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
                    ArticleCollectionId = articleCollectionId,
                    Name = name,
                    Text = text,
                }))
                .ShouldBeOfType<CreatedAtActionResult>();

            return creationResult.Value.ShouldBeOfType<ArticleViewModel>();
        }

        public void SetFakeClockTo(string offsetDateTimePatternIso)
        {
            FakeClock.Reset(
                OffsetDateTimePattern
                    .GeneralIso
                    .Parse(offsetDateTimePatternIso)
                    .Value
                    .ToInstant()
            );
        }
    }
}
