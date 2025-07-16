using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
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
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Strict,
        });

        app.MapPost(LoginApiUrl, async ([FromForm] string password, HttpContext httpContext) =>
        {
            if (!password.Equals("password")) // Todo: Make this configurable. 
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
}
