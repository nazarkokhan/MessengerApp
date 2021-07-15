using MessengerApp.Core.ResultModel.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.ActionResults
{
    public class ErrorableActionResult : ActionResult
    {
        public ErrorableActionResult(IResult result)
        {
            Result = result;
        }

        public IResult Result { get; }
    }
}