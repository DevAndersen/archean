using Archean.Core;
using Archean.Core.Models.Commands;
using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Application.Models.Commands;

[Command("Teleport", Aliases = ["tp"])]
public class TeleportCommand : Command
{
    private readonly ILogger<TestCommand> _logger;

    [CommandParameter(0, Required = true)]
    public int PosX { get; set; }

    [CommandParameter(1, Required = true)]
    public int PosY { get; set; }

    [CommandParameter(2, Required = true)]
    public int PosZ { get; set; }

    public TeleportCommand(ILogger<TestCommand> logger)
    {
        _logger = logger;
    }

    public override async Task InvokeAsync()
    {
        _logger.LogInformation("Teleport command invoked.");
        if (InvokingPlayer != null)
        {
            await InvokingPlayer.Connection.SendAsync(new ServerAbsolutePositionAndOrientationPacket
            {
                X = PosX,
                Y = PosY,
                Z = PosZ,
                PlayerId = Constants.Networking.PlayerSelfId,
                Pitch = 0, // Todo
                Yaw = 0 // Todo
            });
        }
    }
}
