using MavCs.Core.Messages;
using MavCs.Core.Runtime;

namespace MavCs.Core.Extensions;

public static class TelemetrySenders
{
    public static Task SendAttitude(this MavlinkConnection connection,
        uint timeBootMs,
        float roll, float pitch, float yaw,
        float rollSpeed = 0, float pitchSpeed = 0, float yawSpeed = 0,
        CancellationToken ct = default)
    {
        var msg = new AttitudeMessage
        {
            TimeBootMs = timeBootMs,
            Roll = roll,
            Pitch = pitch,
            Yaw = yaw,
            RollSpeed = rollSpeed,
            PitchSpeed = pitchSpeed,
            YawSpeed = yawSpeed
        };
        return connection.SendMessageAsync(msg, ct);
    }

    public static Task SendGlobalPositionInt(this MavlinkConnection connection,
        uint timeBootMs,
        int lat, int lon, int alt, int relativeAlt,
        short vx = 0, short vy = 0, short vz = 0, ushort hdg = 0,
        CancellationToken ct = default)
    {
        var msg = new GlobalPositionIntMessage
        {
            TimeBootMs = timeBootMs,
            Lat = lat,
            Lon = lon,
            Alt = alt,
            RelativeAlt = relativeAlt,
            Vx = vx,
            Vy = vy,
            Vz = vz,
            Hdg = hdg
        };
        return connection.SendMessageAsync(msg, ct);
    }
}
