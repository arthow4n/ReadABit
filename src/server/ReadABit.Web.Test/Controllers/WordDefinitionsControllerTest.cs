using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controllers;
using ReadABit.Web.Test.Helpers;
using Shouldly;
using Xunit;

namespace ReadABit.Web.Test.Controllers
{
    public class WordDefinitionsControllerTest : TestBase
    {
        public WordDefinitionsControllerTest(IServiceProvider serviceProvider, IRequestContext requestContext) : base(serviceProvider, requestContext)
        {
        }

        [Fact]
        public async Task CRUD_Succeeds()
        {
            var creationRequest = new WordDefinitionCreate
            {
                Word = Word,
                LanguageCode = "en",
                Meaning = "Hello",
                Public = false,
            };

            var creationResult =
                (await WordDefinitionsController.Create(creationRequest))
                .ShouldBeOfType<CreatedAtActionResult>();
            var createdId = creationResult.Value.ShouldBeOfType<WordDefinition>().Id;

            (await List()).Items.Count.ShouldBe(1);

            using (User(2))
            {
                (await List()).Items.Count.ShouldBe(0);
                (await WordDefinitionsController.Get(createdId, new WordDefinitionGet { })).ShouldBeOfType<NotFoundResult>();
            }

            await WordDefinitionsController.Update(createdId, new WordDefinitionUpdate { Public = true });

            using (User(2))
            {
                var created = await Get(createdId);
                created.Id.ShouldBe(createdId);
                created.LanguageCode.ShouldBe(creationRequest.LanguageCode);
                created.Meaning.ShouldBe(creationRequest.Meaning);

                (await WordDefinitionsController.Update(createdId, new WordDefinitionUpdate
                {
                    LanguageCode = "en",
                    Meaning = "Another way to say hello",
                })).ShouldBeOfType<NotFoundResult>();
            }

            var updateRequest = new WordDefinitionUpdate
            {
                LanguageCode = "sv",
                Meaning = "ett hälsningsord",
            };
            await WordDefinitionsController.Update(createdId, updateRequest);

            var updated = await Get(createdId);
            updated.LanguageCode.ShouldBe(updateRequest.LanguageCode);
            updated.Meaning.ShouldBe(updateRequest.Meaning);

            using (User(2))
            {
                (await WordDefinitionsController.Delete(createdId, new WordDefinitionDelete { })).ShouldBeOfType<NotFoundResult>();
            }
            (await List()).Items.Count.ShouldBe(1);

            (await WordDefinitionsController.Delete(createdId, new WordDefinitionDelete { })).ShouldBeOfType<NoContentResult>();
            (await List()).Items.Count.ShouldBe(0);
            (await WordDefinitionsController.Get(createdId, new WordDefinitionGet { })).ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ListWordDefinitionPublicSuggestions_CountsCorrectly()
        {
            WordDefinitionCreate creationRequest = new()
            {
                Word = Word,
                LanguageCode = "en",
                Meaning = "Hello",
                Public = true,
            };

            var expectedVm = new WordDefinitionListPublicSuggestionViewModel
            {
                LanguageCode = creationRequest.LanguageCode,
                Meaning = creationRequest.Meaning,
                Count = 1,
            };

            await WordDefinitionsController.Create(creationRequest with { Public = false });
            // The user should always be able to see their own definition.
            (await ListPublicSuggestions()).Items.ShouldHaveSingleItem().ShouldBe(expectedVm with { Count = 1 });

            // User 2 shouldn't see the word definition created by user 1 because it's private.
            using (User(2))
            {
                (await ListPublicSuggestions()).Items.ShouldBeEmpty();

                await WordDefinitionsController.Create(creationRequest with { Public = true });
            }

            // Switching user for the following queries so we don't have to take private word definitions into account when making assertions.

            using (User(3))
            {
                (await ListPublicSuggestions()).Items.ShouldHaveSingleItem().ShouldBe(expectedVm with { Count = 1 });

                await WordDefinitionsController.Create(creationRequest with { Public = true });
            }

            // User 4 should be able to see 2 becasue they were created public by user 2 and 3.
            using (User(4))
            {
                (await ListPublicSuggestions()).Items.ShouldHaveSingleItem().ShouldBe(expectedVm with { Count = 2 });

                await WordDefinitionsController.Create(creationRequest with
                {
                    LanguageCode = "sv",
                    Meaning = "något annat",
                    Public = true,
                });
            }

            using (User(5))
            {
                (await ListPublicSuggestions()).Items.ShouldSatisfyAllConditions(
                    x => x.Count.ShouldBe(2),
                    x => x.First().ShouldBe(expectedVm with { Count = 2 }),
                    x => x.ElementAt(1).ShouldBe(expectedVm with { Count = 1, LanguageCode = "sv", Meaning = "något annat" })
                );
            }

            using (User(6))
            {
                // Word definitions of preferred language should always show up first.
                (await ListPublicSuggestions(preferredLanguageCode: "sv"))
                    .Items
                    .ShouldSatisfyAllConditions(
                        x => x.Count.ShouldBe(2),
                        x => x.First().ShouldBe(expectedVm with { Count = 1, LanguageCode = "sv", Meaning = "något annat" }),
                        x => x.ElementAt(1).ShouldBe(expectedVm with { Count = 2 }
                    )
                );
            }
        }

        private async Task<Paginated<WordDefinition>> List()
        {
            return (await WordDefinitionsController.List(
                new WordDefinitionList
                {
                    Filter = new WordDefinitionListFilter
                    {
                        Word = Word,
                    },
                    Page = new PageFilter
                    {
                        Index = 1,
                    },
                })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<Paginated<WordDefinition>>();
        }

        private async Task<Paginated<WordDefinitionListPublicSuggestionViewModel>> ListPublicSuggestions(string preferredLanguageCode = "en")
        {
            return (await WordDefinitionsController.ListPublicSuggestions(new WordDefinitionListPublicSuggestions
            {
                Filter = new WordDefinitionListPublicSuggestionsFilter
                {
                    Word = Word,
                    PreferredLanguageCode = preferredLanguageCode
                },
                Page = new PageFilter
                {
                    Index = 1,
                },
            })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<Paginated<WordDefinitionListPublicSuggestionViewModel>>();
        }

        private async Task<WordDefinition> Get(Guid id)
        {
            return (await WordDefinitionsController.Get(id, new WordDefinitionGet { })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<WordDefinition>();
        }

        private static WordSelector Word => new()
        {
            LanguageCode = "en",
            Expression = "Hallå",
        };
    }
}
