using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewWebAPICore.DTO_s;

namespace NewWebAPICore.Filters
{
    public class ExceptionFilter :IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            //var exception = context.Exception;

            //var response = new APIResponseDTO_s<string>(
            //    false,
            //    exception.Message,
            //    null
            //);

            //context.Result = exception switch
            //{
            //    UnauthorizedAccessException => new ObjectResult(response)
            //    {
            //        StatusCode = StatusCodes.Status403Forbidden
            //    },

            //    ArgumentException => new ObjectResult(response)
            //    {
            //        StatusCode = StatusCodes.Status400BadRequest
            //    },

            //    _ => new ObjectResult(new APIResponseDTO_s<string>(
            //            false,
            //            "An unexpected error occurred",
            //            null))
            //    {
            //        StatusCode = StatusCodes.Status500InternalServerError
            //    }
            //};

            //context.ExceptionHandled = true; 

            if (context.Exception is ProductionController)
            {
                context.Result = new ObjectResult(new
                {
                    message = context.Exception.Message
                })
                {
                    StatusCode = 400
                };

                context.ExceptionHandled = true; 
            }
        }
    }
}
