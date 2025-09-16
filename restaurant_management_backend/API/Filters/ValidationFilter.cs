using Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                    .ToList();

                string errorMessage = string.Join(" | ", errors);

                var response = ApiResponse<string>.Fail(errorMessage);

                context.Result = new JsonResult(response)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };

                return;
            }

            await next();
        }
    }
}
