namespace SistemaOrdenesReparacion.SI

{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf"; 

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/")
            {
                context.Response.Redirect("/swagger");
                return;
            }

            if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey) || extractedApiKey != API_KEY)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key no válida.");
                return;
            }

            await _next(context);
        }
    }
}
