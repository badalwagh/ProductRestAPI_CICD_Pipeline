using Microsoft.AspNetCore.Mvc.Filters;

namespace NewWebAPICore.Filters
{
    public class CustomResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add("X-App-Name", "WebAPICore");
            context.HttpContext.Response.Headers.Add("Created", "WebAPICore");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Runs AFTER response is sent (rarely used)
        }
    }
}
