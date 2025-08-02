using Microsoft.AspNetCore.Mvc;

namespace Archean.App.WebApp.Extensions;

public static class ApiEndpointExtensions
{
    public static RouteHandlerBuilder MapSkinApi(this IEndpointRouteBuilder builder)
    {
        return builder.MapGet("skin/{skin}", ([FromRoute] string skin) =>
        {
            // Check for illegal sequences.
            if (skin.Contains("..") || skin.Contains('/') || skin.Contains('\\'))
            {
                return TypedResults.NotFound();
            }

            string skinsDirectory = "skins"; // Todo: Get value from configuration.
            string skinPath = Path.Combine(skinsDirectory, skin);

            // Check for paths outside of skin directory.
            string absoluteSkinPath = Path.GetFullPath(skinPath);
            string absoluteSkinDirectory = Path.GetFullPath(skinsDirectory);
            if (!absoluteSkinPath.StartsWith(absoluteSkinDirectory))
            {
                return TypedResults.NotFound();
            }

            // Check if the requested file exists.
            if (!File.Exists(skinPath))
            {
                return TypedResults.NotFound();
            }

            byte[] bytes = File.ReadAllBytes(skinPath);
            return Results.File(bytes, "image/png");
        });
    }
}
