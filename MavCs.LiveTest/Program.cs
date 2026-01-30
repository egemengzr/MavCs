using System.Buffers;
using MavCs.Core.Messages;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;
using MavCs.Core.Transport;

namespace MavCs.LiveTest;

class Program
{
    private static readonly KnownMessages Registry = new();
    
    private static readonly MessageDispatcher Dispatcher = new();

    static async Task Main()
    {
        Console.WriteLine("🚀 MavCs Live Listener (Polymorphic Dispatch)");
        Console.WriteLine("→ Listening on 127.0.0.1:14550...\n");

        RegisterHandlers();

        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        await using var udp = new MavLinkUdpTransport(
            host: "127.0.0.1", 
            localPort: 14550, 
            remotePort: 14551
        );

        await udp.StartAsync(ct);

        var decoder = new MavLinkDecoder(Registry);
        var factory = new MavMessageFactory();

        await foreach (var frameBuffer in udp.ReceiveAsync(ct))
        {
            if (decoder.TryReadFrame(frameBuffer.Span, out var header, out _))
            {
                if (header is not null && factory.TryDeserializeFrame(header, out var msg) && msg is not null)
                {
                    Dispatcher.Dispatch(msg);
                }
            }
        }
    }
    private static void RegisterHandlers()
    {
        Dispatcher.Subscribe<HeartbeatMessage>(OnHeartbeat);
        Dispatcher.Subscribe<SysStatusMessage>(OnSysStatus);
        Dispatcher.Subscribe<BatteryStatusMessage>(OnBatteryStatus);
        Dispatcher.Subscribe<GpsRawIntMessage>(OnGpsRawInt);
        Dispatcher.Subscribe<AttitudeMessage>(OnAttitude);
        Dispatcher.Subscribe<StatustextMessage>(OnStatusText);
        
        Dispatcher.Subscribe<VfrHudMessage>(msg => 
            Console.WriteLine($"✈️  HUD: Speed {msg.Airspeed:F1} m/s, Alt {msg.Alt:F1} m"));
        
        Dispatcher.Subscribe<AutopilotVersionMessage>(msg => 
            Console.WriteLine($"🧠 AUTOPILOT  | Board: {msg.BoardVersion} | Vendor: {msg.VendorId} | Product: {msg.ProductId}"));
        
        Dispatcher.Subscribe<ParamValueMessage>(OnParamValue);

        Dispatcher.Subscribe<MissionCountMessage>(msg => 
            Console.WriteLine($"🗺️  MISSION    | Waypoint Count: {msg.Count} | Type: {msg.MissionType}"));
            
        Dispatcher.Subscribe<MissionItemIntMessage>(OnMissionItem);
        
    }

    
    private static void OnParamValue(ParamValueMessage msg)
    {
        Console.WriteLine($"⚙️  PARAM      | {msg.ParamIndex + 1}/{msg.ParamCount} | {msg.ParamId} : {msg.ParamValue}");
    }

    private static void OnMissionItem(MissionItemIntMessage msg)
    {
        Console.WriteLine($"📍 WAYPOINT   | Seq: {msg.Seq} | Cmd: {msg.Command} | X: {msg.X} | Y: {msg.Y} | Z: {msg.Z}");
    }
    
    private static void OnHeartbeat(HeartbeatMessage msg)
    {
        Console.WriteLine($"💓 HEARTBEAT | Mode: {msg.BaseMode} | Status: {msg.SystemStatus}");
    }

    private static void OnSysStatus(SysStatusMessage msg)
    {
        Console.WriteLine($"⚡ SYS_STATUS | Load: {msg.Load/10.0}% | Batt: {msg.VoltageBattery}mV");
    }

    private static void OnBatteryStatus(BatteryStatusMessage msg)
    {
        var cell1 = (msg.Voltages != null && msg.Voltages.Length > 0) ? msg.Voltages[0] : 0;
        Console.WriteLine($"🔋 BATTERY    | Rem: {msg.BatteryRemaining}% | Cell1: {cell1}mV | Cur: {msg.CurrentBattery}cA");
    }

    private static void OnGpsRawInt(GpsRawIntMessage msg)
    {
        double lat = msg.Lat / 10_000_000.0;
        double lon = msg.Lon / 10_000_000.0;
        string fix = msg.FixType switch { 0 => "No Fix", 3 => "3D Fix", _ => msg.FixType.ToString() };

        Console.WriteLine($"📍 GPS        | {fix} | Lat: {lat:F6} | Lon: {lon:F6} | Sats: {msg.SatellitesVisible}");
    }

    private static void OnAttitude(AttitudeMessage msg)
    {
        double roll = msg.Roll * (180.0 / Math.PI);
        double pitch = msg.Pitch * (180.0 / Math.PI);
        double yaw = msg.Yaw * (180.0 / Math.PI);

        Console.WriteLine($"🔄 ATTITUDE   | R: {roll,6:F1}° | P: {pitch,6:F1}° | Y: {yaw,6:F1}°");
    }

    private static void OnStatusText(StatustextMessage msg)
    {
        var color = msg.Severity <= 3 ? ConsoleColor.Red : ConsoleColor.Cyan;
        Console.ForegroundColor = color;
        Console.WriteLine($"💬 MSG        | {msg.Text}");
        Console.ResetColor();
    }
}
