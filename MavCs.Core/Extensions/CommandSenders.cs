using MavCs.Core.Messages;
using MavCs.Core.Runtime;

namespace MavCs.Core.Extensions;

public static class CommandSenders
{
    public static Task SendCommandLong(this MavlinkConnection connection,
        byte targetSystem,
        byte targetComponent,
        ushort command,
        float param1 = 0, float param2 = 0, float param3 = 0, float param4 = 0,
        float param5 = 0, float param6 = 0, float param7 = 0,
        byte confirmation = 0,
        CancellationToken ct = default)
    {
        var msg = new CommandLongMessage
        {
            TargetSystem = targetSystem,
            TargetComponent = targetComponent,
            Command = command,
            Confirmation = confirmation,
            Param1 = param1,
            Param2 = param2,
            Param3 = param3,
            Param4 = param4,
            Param5 = param5,
            Param6 = param6,
            Param7 = param7
        };
        return connection.SendMessageAsync(msg, ct);
    }
}
