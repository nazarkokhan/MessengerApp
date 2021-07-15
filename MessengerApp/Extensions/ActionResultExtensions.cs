using MessengerApp.Core.ResultModel.Abstraction;
using MessengerApp.ActionResults;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.Extensions
{
    public static class ActionResultExtensions
    {
        public static IActionResult ToActionResult(this IResult result)
            => new ErrorableActionResult(result);
    }
}