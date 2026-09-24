using System.Security.Claims;
using GoldShopAPI.Helpers;

namespace GoldShopAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IJwtHelper jwtHelper)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var principal = jwtHelper.ValidateToken(token);
                    if (principal != null)
                    {
                        context.User = principal;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating token");
                }
            }

            await _next(context);
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}