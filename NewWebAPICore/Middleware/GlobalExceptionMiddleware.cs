using NewWebAPICore.Model;
using WebAPICore.Data;

namespace NewWebAPICore.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var userName = context.User?.Identity?.IsAuthenticated == true
                    ? context.User.Identity.Name
                    : "Anonymous";

                var errorLog = new ErrorLogs
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Path = context.Request.Path,
                    Method = context.Request.Method,
                    UserName = userName,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.ErrorLogs.Add(errorLog);
                await dbContext.SaveChangesAsync();

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    statusCode = 500,
                    message = "Something went wrong. Please try again later."
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

}
