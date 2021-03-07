using System;
using System.Collections.Generic;
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
    public class WordDefinitionsControllerTest : TestBase<WordDefinitionsController>
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
                (await T1.CreateWordDefinition(creationRequest))
                .ShouldBeOfType<CreatedAtActionResult>();
            var createdId = creationResult.Value.ShouldBeOfType<WordDefinition>().Id;

            (await List(searchPublic: false)).Count.ShouldBe(1);

            using (AnotherUser)
            {
                (await List(searchPublic: true)).Count.ShouldBe(0);
                (await T1.GetWordDefinition(createdId, new WordDefinitionGet { })).ShouldBeOfType<NotFoundResult>();
            }

            await T1.UpdateWordDefinition(createdId, new WordDefinitionUpdate { Public = true });

            using (AnotherUser)
            {
                var created = await Get(createdId);
                created.Id.ShouldBe(createdId);
                created.LanguageCode.ShouldBe(creationRequest.LanguageCode);
                created.Meaning.ShouldBe(creationRequest.Meaning);

                (await T1.UpdateWordDefinition(createdId, new WordDefinitionUpdate
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
            await T1.UpdateWordDefinition(createdId, updateRequest);

            var updated = await Get(createdId);
            updated.LanguageCode.ShouldBe(updateRequest.LanguageCode);
            updated.Meaning.ShouldBe(updateRequest.Meaning);

            using (AnotherUser)
            {
                (await T1.DeleteWordDefinition(createdId, new WordDefinitionDelete { })).ShouldBeOfType<NotFoundResult>();
            }
            (await List(searchPublic: true)).Count.ShouldBe(1);

            (await T1.DeleteWordDefinition(createdId, new WordDefinitionDelete { })).ShouldBeOfType<NoContentResult>();
            (await List(searchPublic: true)).Count.ShouldBe(0);
            (await T1.GetWordDefinition(createdId, new WordDefinitionGet { })).ShouldBeOfType<NotFoundResult>();
        }

        private async Task<List<WordDefinition>> List(bool searchPublic)
        {
            return (await T1.ListWordDefinitions(
                new WordDefinitionList
                {
                    Filter = new WordDefinitionListFilter
                    {
                        Public = searchPublic,
                        Word = Word,
                    },
                })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<List<WordDefinition>>();
        }

        private async Task<WordDefinition> Get(Guid id)
        {
            return (await T1.GetWordDefinition(id, new WordDefinitionGet { })).ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<WordDefinition>();
        }

        private static WordSelector Word => new()
        {
            LanguageCode = "en",
            Expression = "Hallå",
        };
    }
}
