using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Exceptions;
using System.Threading.Tasks;

namespace RestaurantAPI.Middleware
{
    //Klasa odpowiadająca za procesowanie zapytań
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            try
            {
                await next.Invoke(context);
            }
            catch(NotFoundException notFoundException)
            {
                context.Response.StatusCode = 404;
                context.Response.WriteAsync(notFoundException.Message);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = 500;//Zwracany kod 
                await context.Response.WriteAsync("Something went wrong");//Wypisanie do klienta wartości
            }
        }
    }
}
