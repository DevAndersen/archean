using Archean.App.WebApp.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Archean.App.WebApp.Extensions;

public static class WebAppAuthenticationExtensions
{
    private const string CookieNameClaimValue = "admin";
    private const string CookiePassClaimType = "passhash";

    public const string LoginApiUrl = "/api/login";
    public const string LogoutApiUrl = "/api/logout";

    public static WebApplicationBuilder AddCookieAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Todo: Make this configurable.
            options.SlidingExpiration = true;
            options.LoginPath = LoginApiUrl;
            options.LogoutPath = LogoutApiUrl;
            options.Events.OnValidatePrincipal = async principalContext =>
            {
                IOptions<WebAppSettings> webAppSettings = principalContext.HttpContext.RequestServices.GetRequiredService<IOptions<WebAppSettings>>();
                string serverPasswordHash = HashPassword(webAppSettings.Value.SitePassword);

                Claim? passHashClaim = principalContext.Principal?.Claims.FirstOrDefault(x => x.Type == CookiePassClaimType && x.Value == serverPasswordHash);
                if (passHashClaim == null)
                {
                    principalContext.RejectPrincipal();
                    await principalContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            };
        });

        return builder;
    }

    public static WebApplication UseCookieAuthentication(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Strict,
        });

        app.MapPost(LoginApiUrl, async (
            [FromForm] string password,
            [FromServices] IOptions<WebAppSettings> webAppSettings,
            HttpContext httpContext) =>
        {
            string serverPassword = webAppSettings.Value.SitePassword;
            if (!string.IsNullOrWhiteSpace(serverPassword) && !password.Equals(serverPassword))
            {
                return TypedResults.Redirect("/?loginfail");
            }

            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            string serverPasswordHash = HashPassword(serverPassword);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, CookieNameClaimValue),
                    new Claim(CookiePassClaimType, serverPasswordHash)
                ],
                CookieAuthenticationDefaults.AuthenticationScheme);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return TypedResults.Redirect("/");
        }).AllowAnonymous();

        app.MapPost(LogoutApiUrl, () => TypedResults.SignOut(new AuthenticationProperties
        {
            RedirectUri = "/"
        }));

        return app;
    }

    /// <summary>
    /// Creates a group of endpoints, declaring endpoints using <paramref name="endpointBuildingAction"/>.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <param name="prefix"></param>
    /// <param name="endpointBuildingAction"></param>
    /// <returns></returns>
    public static RouteGroupBuilder MapGroup(this IEndpointRouteBuilder endpoints, [StringSyntax("Route")] string prefix, Action<RouteGroupBuilder> endpointBuildingAction)
    {
        RouteGroupBuilder group = endpoints.MapGroup(prefix);
        endpointBuildingAction(group);
        return group;
    }

    /// <summary>
    /// Adds simple request authentication filtering.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static T RequireAuthentication<T>(this T builder) where T : IEndpointConventionBuilder
    {
        return builder.AddEndpointFilter(async (invocationContext, next)
            => invocationContext.HttpContext.User.Identity?.IsAuthenticated == true
                ? await next(invocationContext)
                : TypedResults.Unauthorized());
    }

    /// <summary>
    /// Returns the base-64 encoded hash of <paramref name="password"/>.
    /// </summary>
    /// <remarks>
    /// This is not industry standard security, but it should be sufficient for the use case.
    /// </remarks>
    /// <param name="password"></param>
    /// <returns></returns>
    private static string HashPassword(string password)
    {
        Span<byte> buffer = stackalloc byte[SHA256.HashSizeInBytes];
        SHA256.HashData(Encoding.UTF8.GetBytes(password), buffer);

        for (int i = 0; i < 100; i++)
        {
            SHA256.HashData([.. buffer, .. "static salt"u8, (byte)i], buffer);
        }

        return Convert.ToBase64String(buffer);
    }
}
