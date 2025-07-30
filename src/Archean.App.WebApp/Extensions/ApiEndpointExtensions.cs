using Microsoft.AspNetCore.Mvc;

namespace Archean.App.WebApp.Extensions;

public static class ApiEndpointExtensions
{
    public static RouteHandlerBuilder MapSkinApi(this IEndpointRouteBuilder builder)
    {
        return builder.MapGet("skin/{skin}", ([FromRoute] string skin) =>
        {
            string skinsDirectory = "skins"; // Todo: Get value from configuration.
            string skinPath = Path.Combine(skinsDirectory, skin);

            if (!File.Exists(skinPath))
            {
                return TypedResults.NotFound();
            }

            byte[] bytes = File.ReadAllBytes(skinPath);
            return Results.File(bytes, "image/png");
        });
    }
}
