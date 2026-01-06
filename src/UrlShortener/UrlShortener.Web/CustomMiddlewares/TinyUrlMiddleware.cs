using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using UrlShortener.Web.Constants;
using UrlShortener.Web.Data;

namespace UrlShortener.Web.CustomMiddlewares
{
    public class TinyUrlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public TinyUrlMiddleware(RequestDelegate next,
            IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.Trim('/');

            if (!string.IsNullOrEmpty(path) && IsTinyUrl(path))
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var url = await db.UrlShorteners
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Code == path);

                if (url != null)
                {
                    context.Request.Path = "/UrlShortener/RedirectUrl";
                    context.Request.QueryString = new QueryString($"?url={url.LongUrl}");
                }
            }

            await _next(context);
        }

        private bool IsTinyUrl(string path)
        {
            return Regex.IsMatch(path, $"^[a-zA-Z0-9]{{{GlobalVariables.codeLength}}}$");
        }
    }
}
