using System.Threading.Tasks;
using ReadABit.Core.Commands;
using ReadABit.Core.Contracts;
using ReadABit.Web.Controller.Utils;

namespace ReadABit.Web.Controllers.Helpers
{
    public class DailyGoalHelper
    {
        private readonly ApiControllerBase _controllerBase;

        public DailyGoalHelper(ApiControllerBase controllerBase)
        {
            _controllerBase = controllerBase;
        }

        public async Task<WordFamiliarityDailyGoalCheckViewModel> PerformDailyGoalCheck()
        {
            var userPreferenceData = await _controllerBase.Mediator.Send(new UserPreferenceGet
            {
                UserId = _controllerBase.RequestUserId,
            });

            return await _controllerBase.Mediator.Send(new WordFamiliarityDailyGoalCheck
            {
                UserId = _controllerBase.RequestUserId,
                DailyGoalResetTimeTimeZone = userPreferenceData.DailyGoalResetTimeTimeZone,
                DailyGoalResetTimePartial = userPreferenceData.DailyGoalResetTimePartial,
                DailyGoalNewlyCreatedWordFamiliarityCount = userPreferenceData.DailyGoalNewlyCreatedWordFamiliarityCount,
            });
        }
    }
}
