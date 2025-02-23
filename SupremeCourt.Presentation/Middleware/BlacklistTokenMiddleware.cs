using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SupremeCourt.Application.Services;

namespace SupremeCourt.Presentation.Middleware
{
    public class BlacklistTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenBlacklistService _tokenBlacklistService;

        public BlacklistTokenMiddleware(RequestDelegate next, TokenBlacklistService tokenBlacklistService)
        {
            _next = next;
            _tokenBlacklistService = tokenBlacklistService;
        }

        public async Task Invoke(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Replace("Bearer ", "");

                if (_tokenBlacklistService.IsTokenBlacklisted(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token has been revoked.");
                    return;
                }
            }

            await _next(context);
        }
    }
}