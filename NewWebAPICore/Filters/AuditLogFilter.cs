using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
namespace NewWebAPICore.Filters
{
    public class AuditLogFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            var username = user?.Identity?.IsAuthenticated == true
                ? user.Identity.Name
                : "Anonymous";

            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];

            Console.WriteLine(
                $"User: {username}, Controller: {controller}, Action: {action}, Time: {DateTime.UtcNow}"
            );
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // After action executes (optional)
        }

    }

}
