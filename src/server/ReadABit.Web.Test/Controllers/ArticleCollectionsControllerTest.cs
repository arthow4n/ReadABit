using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Web.Controllers;
using Shouldly;
using Xunit;

namespace ReadABit.Web.Test.Controllers
{
    public class ArticleCollectionsControllerTest
    {
        private readonly ArticleCollectionsController controller;

        public ArticleCollectionsControllerTest(IMediator mediator)
        {
            this.controller = new ArticleCollectionsController(mediator);
        }

        [Fact]
        public async Task Create_Succeeds()
        {
            var name = "dummy";

            var id = await controller.Create(name);
            var created = await controller.Get(id);

            created.Id.ShouldBe(id);
            created.Name.ShouldBe(name);
        }
    }
}