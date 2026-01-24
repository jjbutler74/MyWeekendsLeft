using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MWL.API.Middleware
{
    /// <summary>
    /// Middleware that adds security headers to all HTTP responses.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Prevent clickjacking attacks
            context.Response.Headers["X-Frame-Options"] = "DENY";

            // Prevent MIME type sniffing
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            // Enable XSS filter in browsers
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

            // Control referrer information
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Content Security Policy - restrict resource loading
            context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; frame-ancestors 'none'";

            // Prevent caching of sensitive data
            context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";

            await _next(context);
        }
    }
}
