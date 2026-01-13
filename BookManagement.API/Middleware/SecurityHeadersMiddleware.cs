using Microsoft.AspNetCore.Http;

namespace BookManagement.Common.Middleware
{
    /// <summary>
    /// Middleware that adds security-related HTTP headers to all responses.
    /// </summary>
    /// <remarks>
    /// This middleware enhances security by setting the following headers:
    /// <list type="bullet">
    ///   <item><description><c>X-Frame-Options: DENY</c> — Prevents clickjacking.</description></item>
    ///   <item><description><c>X-Content-Type-Options: nosniff</c> — Prevents MIME type sniffing.</description></item>
    ///   <item><description><c>X-XSS-Protection: 1; mode=block</c> — Enables XSS filtering in older browsers.</description></item>
    ///   <item><description><c>Referrer-Policy: strict-origin-when-cross-origin</c> — Controls referrer information sent.</description></item>
    ///   <item><description><c>Permissions-Policy</c> — Restricts access to APIs like geolocation, camera, and microphone.</description></item>
    ///   <item><description><c>Content-Security-Policy</c> — Restricts sources for scripts, styles, images, fonts, and connections.</description></item>
    /// </list>
    /// </remarks>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the middleware to add security headers to the HTTP response.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Zabránit klikjackingu
            context.Response.Headers["X-Frame-Options"] = "DENY";

            // Zabránit MIME sniffingu
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            // Zapnout XSS ochranu v prohlížečích
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

            // Referrer Policy
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Permissions Policy (nahrazuje Feature-Policy)
            context.Response.Headers["Permissions-Policy"] =
                "geolocation=(), microphone=(), camera=()";

            // Content Security Policy
            context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; connect-src 'self' https://localhost:5001;";

            await _next(context);
        }
    }
}
