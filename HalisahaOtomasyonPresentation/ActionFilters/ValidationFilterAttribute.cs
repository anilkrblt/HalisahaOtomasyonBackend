using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HalisahaOtomasyon.ActionFilters
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var action = context.RouteData.Values["action"];
            var controller = context.RouteData.Values["controller"];

            foreach (var arg in context.ActionArguments)
            {
                var value = arg.Value;

                if (value == null)
                {
                    context.Result = new BadRequestObjectResult($"Argument '{arg.Key}' is null. Controller: {controller}, Action: {action}");
                    return;
                }

                if (value is IEnumerable<object> list)
                {
                    foreach (var item in list)
                    {
                        if (item == null)
                        {
                            context.Result = new BadRequestObjectResult($"List item in '{arg.Key}' is null. Controller: {controller}, Action: {action}");
                            return;
                        }
                    }
                }
            }
            if (!context.ModelState.IsValid)
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
        }
    }
}