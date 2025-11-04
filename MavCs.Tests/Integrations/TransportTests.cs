using System.Buffers;
using MavCs.Core.Messages;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;
using MavCs.Core.Transport;

namespace MavCs.Tests.Integrations;

public class TransportTests
{
    private static async Task<T> WaitOrThrow<T>(Task<T> task, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        var t = await Task.WhenAny(task, Task.Delay(Timeout.InfiniteTimeSpan, cts.Token));
        if (t != task)
            throw new TimeoutException("Timed out waiting for UDP frame");
        return await task;
    }

    [Fact]
    public async Task Udp_Roundtrip_RawBytes()
    {
        const int A = 14560;
        const int B = 14561;

        await using var recv = new MavLinkUdpTransport(host: "127.0.0.1", remotePort: B, localPort: A);
        await using var send = new MavLinkUdpTransport(host: "127.0.0.1", remotePort: A, localPort: B);

        await recv.StartAsync(CancellationToken.None);
        await send.StartAsync(CancellationToken.None);

        var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Staring async listening on background
        _ = Task.Run(async () =>
        {
            await foreach (var data in recv.ReceiveAsync(CancellationToken.None))
            {
                tcs.TrySetResult(data.ToArray());
                break;
            }
        });

        byte[] payload = { 1, 2, 3, 4, 5 };
        await send.SendAsync(payload, CancellationToken.None);

        var received = await WaitOrThrow(tcs.Task, TimeSpan.FromSeconds(1));
        Assert.Equal(payload, received);
    }

    [Fact]
    public async Task Udp_Roundtrip_Heartbeat_EndToEnd()
    {
        const int A = 14570;
        const int B = 14571;

        await using var recv = new MavLinkUdpTransport(host: "127.0.0.1", remotePort: B, localPort: A);
        await using var send = new MavLinkUdpTransport(host: "127.0.0.1", remotePort: A, localPort: B);

        await recv.StartAsync(CancellationToken.None);
        await send.StartAsync(CancellationToken.None);

        // Prepare a Heartbeat message
        var msg = new HeartbeatMessage
        {
            Type = 6,
            Autopilot = 8,
            BaseMode = 0x81,
            CustomMode = 0x11223344u,
            SystemStatus = 4,
            MavlinkVersion = 3
        };

        var encoder = new MavLinkEncoder(new KnownMessages());
        var buf = new ArrayBufferWriter<byte>();
        encoder.WriteV1(msg, sequence: 1, systemId: 1, componentId: 1, output: buf);

        var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Async listening
        _ = Task.Run(async () =>
        {
            await foreach (var data in recv.ReceiveAsync(CancellationToken.None))
            {
                tcs.TrySetResult(data.ToArray());
                break;
            }
        });

        await send.SendAsync(buf.WrittenMemory, CancellationToken.None);

        var received = await WaitOrThrow(tcs.Task, TimeSpan.FromSeconds(1));

        var decoder = new MavLinkDecoder(new KnownMessages());
        Assert.True(decoder.TryReadFrame(received, out var frame, out _));

        var factory = new MavMessageFactory();
        Assert.True(factory.TryDeserializeFrame(frame!, out var obj));
        Assert.IsType<HeartbeatMessage>(obj);
    }
}
