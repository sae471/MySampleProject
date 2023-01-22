using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SAE471
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    error = "Validation Exception!!",
                    message = exception.Message
                })
            );
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    error = "Internal Server Error!!",
                    message = exception.Message,
                    innerExceptipn = exception.InnerException != null ? exception.InnerException.Message : string.Empty
                })
            );
        }
    }
}