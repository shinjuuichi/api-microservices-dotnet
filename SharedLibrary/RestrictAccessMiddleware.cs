using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SharedLibrary
{
    public class RestrictAccessMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var referrer = context.Request.Headers["Referer"].FirstOrDefault();
            if(string.IsNullOrEmpty(referrer))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Cant reach.");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
