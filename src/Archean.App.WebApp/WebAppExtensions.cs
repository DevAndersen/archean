using Archean.App.WebApp.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Archean.App.WebApp;

public static class WebAppExtensions
{
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

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, "admin")],
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
        return builder.AddEndpointFilter(async (invocationContext, next) =>
        {
            if (invocationContext.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                return await next(invocationContext);
            }
            return TypedResults.Unauthorized();
        });
    }
}
