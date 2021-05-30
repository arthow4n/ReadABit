using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Contracts;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controllers
{
    public class DevToolsController : ApiControllerBase
    {
        public DevToolsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoopResponse))]
        /// <summary>
        /// A dummy controller mainly for getting NSwag to output some DTOs.
        /// Should find a better way for doing this.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Noop()
        {
            throw new NotImplementedException();
        }

        public class NoopResponse
        {
            public FolketsLexikonLookUpViewModelWordEntry FolketsLexikonLookUpViewModelWordEntry { get; } = new();
            public FolketsLexikonLookUpViewModelTranslationEntry FolketsLexikonLookUpViewModelTranslationEntry { get; } = new();
        }
    }
}
