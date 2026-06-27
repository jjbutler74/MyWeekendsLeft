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

            // Content Security Policy - restrict resource loading.
            // The Swagger UI page (served at "/") has two static inline <script> blocks and one
            // static inline <style> block baked in by Swashbuckle 6.5.0. Their hashes are allow-listed
            // below instead of 'unsafe-inline'. If the Swagger config changes (e.g. adding a v2 doc),
            // the inline bootstrap script's content changes too, and its hash below must be regenerated
            // (fetch "/index.html", hash the new <script> content with SHA-256, base64-encode it).
            context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; " +
                "script-src 'self' 'sha256-Tui7QoFlnLXkJCSl1/JvEZdIXTmBttnWNxzJpXomQjg=' 'sha256-m/T5Wy5/ZwCoQQVeIDdemxZcr9EMBQ7ZoVatNHo7sKo='; " +
                "style-src 'self' 'sha256-wkAU1AW/h8YFx0XlzvpTllAKnFEO2tw8aKErs5a26LY='; " +
                "img-src 'self' data:; font-src 'self'; frame-ancestors 'none'";

            // Allow caching for read-only API endpoints, prevent caching for others
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
            var isCacheableEndpoint = context.Request.Method == "GET" &&
                (path.Contains("/api/getweekends") || path.Contains("/api/version"));

            if (!isCacheableEndpoint)
            {
                // Prevent caching of sensitive data
                context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
            }

            await _next(context);
        }
    }
}
