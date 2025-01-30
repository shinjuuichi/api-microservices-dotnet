namespace Gateway.Middlewares
{
    public class InterceptionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.Headers["Referer"]= "Api-Gateway";
            await next(context);
        }
    }
}
    