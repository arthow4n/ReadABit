using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Core.Utils;
using ReadABit.Web.Contracts;
using ReadABit.Web.Controller.Utils;
using ReadABit.Web.Controllers.Helpers;

namespace ReadABit.Web.Controllers
{
    public class WordFamiliaritiesController : ApiControllerBase
    {

        public WordFamiliaritiesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WordFamiliarityListViewModel))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> List([FromQuery] WordFamiliarityList request)
        {
            WordFamiliarityListViewModel list = await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });
            return Ok(list);
        }

        [Route("UpsertBatch")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WordFamiliarityUpsertBatchResultViewModal))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpsertBatch(WordFamiliarityUpsertBatch request)
        {
            using var ts = DB.TransactionScope();

            await Mediator.Send(request with
            {
                UserId = RequestUserId,
            });
            await SaveChangesAsync();

            var dailyGoalStatus = await new DailyGoalHelper(this).PerformDailyGoalCheck();
            await SaveChangesAsync();

            ts.Complete();

            return Ok(new WordFamiliarityUpsertBatchResultViewModal
            {
                DailyGoalStatus = dailyGoalStatus,
            });
        }

        [Route("/DailyGoalCheck")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WordFamiliarityDailyGoalCheckViewModel))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DailyGoalCheck([FromQuery] WordFamiliarityDailyGoalCheck request)
        {
            var result = await new DailyGoalHelper(this).PerformDailyGoalCheck();
            await SaveChangesAsync();
            return Ok(result);
        }
    }
}
