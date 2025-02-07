using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace SharedLibrary.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.UnprocessableContent, ex.Message.Split("\n"));
            }
            catch (InvalidDataException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (DataNotFoundException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (DataConflictException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        private Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, dynamic message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
        }
    }
}
