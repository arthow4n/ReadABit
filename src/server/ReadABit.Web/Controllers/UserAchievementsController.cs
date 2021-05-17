using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadABit.Core.Commands;
using ReadABit.Core.Commands.UserAchievements;
using ReadABit.Core.Contracts;
using ReadABit.Infrastructure.Models;
using ReadABit.Web.Controller.Utils;
using ReadABit.Web.Controllers.Helpers;

namespace ReadABit.Web.Controllers
{
    public class UserAchievementsController : ApiControllerBase
    {

        public UserAchievementsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("DailyGoalStreak")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserAchievementsDailyGoalStreakStateViewModel))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetDailyGoalStreakState([FromQuery] UserAchievementsDailyGoalStreakGet request)
        {
            var dailyGoalCheckViewModel = await new DailyGoalHelper(this).PerformDailyGoalCheck();

            UserAchievementsDailyGoalStreakStateViewModel vm = await Mediator.Send(request with
            {
                UserId = RequestUserId,
                DailyGoalCheckViewModel = dailyGoalCheckViewModel,
            });

            return Ok(vm);
        }
    }
}
