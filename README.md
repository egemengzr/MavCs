# 🛸 MavCs

**MavCs** is a modern, lightweight MAVLink implementation written in **C# (.NET 9)** — designed with **modularity**, and **auto-discovery** in mind.  
It aims to be a community-driven, easily extensible MAVLink library for modern .NET environments (Unity, MAUI, server backends, etc).

---

## ✨ Features

✅ Full MAVLink v1/v2 frame support  
✅ Automatic CRC handling  
✅ Auto-discovery of messages and serializers via attributes  
✅ Type-safe `IMessageSerializer<T>` interfaces  
✅ Simple test-based examples using `xUnit`  
✅ Built for .NET 8 / 9 (works cross-platform)  

---

## 🧩 Project Structure

```
MavCs/
├── MavCs.Core/            # Core MAVLink logic
│   ├── Protocol/           # FrameV1/V2, CRC, constants
│   ├── Messages/           # MAVLink message definitions
│   ├── Serialization/      # Auto-generated serializers
│   ├── Runtime/            # Encoder, Decoder, Factory
│   └── Registry/           # Known message registry
└── MavCs.Tests/            # Unit and integration tests
```

---

## 🚀 Quick Start

```csharp
using System.Buffers;
using MavCs.Core.Messages;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;

// Create message
var heartbeat = new HeartbeatMessage
{
    Type = 6,
    Autopilot = 8,
    BaseMode = 0x81,
    CustomMode = 0x11223344u,
    SystemStatus = 4,
    MavlinkVersion = 3
};

// Encode
var encoder = new MavLinkEncoder(new KnownMessages());
var buf = new ArrayBufferWriter<byte>();
encoder.WriteV1(heartbeat, sequence: 1, systemId: 1, componentId: 1, output: buf);

// Decode
var decoder = new MavLinkDecoder(new KnownMessages());
decoder.TryReadFrame(buf.WrittenSpan, out var frame, out _);

// Deserialize into typed message
var factory = new MavMessageFactory();
factory.TryDeserializeFrame(frame!, out var obj);
Console.WriteLine(obj is HeartbeatMessage ? "✅ Heartbeat parsed!" : "❌ Failed");
```

---

## 🧠 How It Works

- Each MAVLink message (e.g., `HeartbeatMessage`) is decorated with a `[MavMessage(Id, CrcExtra)]` attribute.
- Serializers are discovered automatically via naming convention (e.g., `HeartbeatSerializer`).
- The encoder auto-detects serializer & metadata — you just call `WriteV1` or `WriteV2`.
- The decoder extracts frame metadata, and `MavMessageFactory` reconstructs the typed object.

---

## 🧪 Running Tests

```bash
dotnet test
```

Tests cover roundtrip encode/decode for Heartbeat and SysStatus messages.

---

## 🛠️ Roadmap

- [ ] Add all common MAVLink 2.0 messages  
- [ ] Add UDP & Serial transport layer  
- [ ] Add message auto-generator (from XML definitions)  
- [ ] Improve unit coverage and docs  
- [ ] Release on NuGet  

---

## 🪪 License

MIT License © 2025 [Egemen Gezer](https://github.com/egemengzr)

---

## 🤝 Contributing

PRs are welcome!  
Open a PR or issue — please follow the message/serializer naming convention (`<MessageName>Message` and `<MessageName>Serializer`).

---

### 🛸 Example Messages Implemented
| Message | ID | CRC | Serializer |
|----------|----|-----|-------------|
| `HEARTBEAT` | 0 | 50 | ✅ |
| `SYS_STATUS` | 1 | 124 | ✅ |

---

> Built with ❤️ for developers who love clean and modular.

