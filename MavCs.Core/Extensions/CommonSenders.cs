using MavCs.Core.Messages;
using MavCs.Core.Runtime;

namespace MavCs.Core.Extensions;

public static class CommonSenders
{
    public static Task SendHeartbeat(this MavlinkConnection connection,
        byte type = 6,
        byte autopilot = 8,
        byte baseMode = 0,
        uint customMode = 0,
        byte systemStatus = 0,
        CancellationToken ct = default)
    {
        var msg = new HeartbeatMessage
        {
            Type = type,
            Autopilot = autopilot,
            BaseMode = baseMode,
            CustomMode = customMode,
            SystemStatus = systemStatus,
            MavlinkVersion = 3
        };
        
        return connection.SendMessageAsync(msg, ct);
    }
    
    public static Task SendStatusText(this MavlinkConnection connection,
        byte severity,
        string text,
        ushort id = 0,
        byte chunkSeq = 0,
        CancellationToken ct = default)
    {
        var msg = new StatustextMessage
        {
            Severity = severity,
            Text = text,
            Id = id,
            ChunkSeq = chunkSeq
        };
        
        return connection.SendMessageAsync(msg, ct);
    }
}
