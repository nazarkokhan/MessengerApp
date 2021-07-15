using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MessengerApp.Core.ResultModel.Abstraction;
using MessengerApp.Core.ResultModel.Abstraction.Generics;
using MessengerApp.ActionResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessengerApp.Filters
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "ASP NET framework will instantiate")]
    public class ErrorableResultFilterAttribute : ResultFilterAttribute
    {
        private const string Errors = nameof(Errors);

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ErrorableActionResult actionResult)
            {
                if (!actionResult.Result.Success)
                {
                    var error = new SerializableError
                    {
                        { Errors, actionResult.Result.Messages }
                    };

                    context.Result = new BadRequestObjectResult(error);
                    LogFailureResult(actionResult.Result, context
                        .HttpContext
                        .RequestServices
                        .GetRequiredService<ILogger<ErrorableResultFilterAttribute>>()
                    );

                    return;
                }

                if (context.HttpContext.Request.Method == "DELETE")
                {
                    context.Result = new NoContentResult();

                    return;
                }

                if (actionResult.Result is IResult<object> objectResult)
                    context.Result = new OkObjectResult(objectResult.Data);
                else
                    context.Result = new OkResult();
            }
        }

        private void LogFailureResult(IResult result, ILogger<ErrorableResultFilterAttribute> logger)
        {
            if(result.Exception is null)
                logger.LogWarning($"Failure result. Error message: {result.Messages.FirstOrDefault()}");
            else
                logger.LogWarning(result.Exception, $"Failure result. Error message: {result.Messages.FirstOrDefault()}");
        }
    }
}