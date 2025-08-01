﻿namespace Archean.Core.Settings;

public class ServerSettings
{
    /// <summary>
    /// The port that the socket server runs on.
    /// </summary>
    public required ushort Port { get; init; } = 25565;

    /// <summary>
    /// The size of the pending connection backlog.
    /// </summary>
    public required int Backlog { get; init; } = 3;

    /// <summary>
    /// The name of the server.
    /// </summary>
    public required string Name { get; init; } = "Unnamed server";

    /// <summary>
    /// The MOTD (message of the day) of the server.
    /// </summary>
    public required string Motd { get; init; } = "Server MOTD";

    /// <summary>
    /// The MOTD displayed when a player joins the world.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><c>{0}</c> will be replaced by the world loading percentage.</item>
    /// </list>
    /// </remarks>
    public required string WorldLoadingMotd { get; init; } = "Loading world, {0}% done";

    /// <summary>
    /// The disconnect message sent to the client when, when attempting to join the server while it is full.
    /// </summary>
    public required string ServerFullMessage { get; init; } = "The server is full";

    /// <summary>
    /// The maximum number of players that can be connected to the server.
    /// </summary>
    /// <remarks>
    /// Values higher than <c>127</c> will be ignored.
    /// </remarks>
    public virtual required byte MaxPlayers { get; init; } = (byte)Constants.Players.HighestPlayerId;
}
