# 🤝 Contributing to MavCs

This document describes how to contribute new messages, tests, and improvements to **MavCs**, following consistent coding and documentation standards.

---

## 1. Repository Overview

The MavCs repository is organized by layers:

```
MavCs.Core/          → Protocol, CRC, Encoder, Decoder, Registry
MavCs.LiveTest/      → Integration tests & UDP live examples
MavCs.Tests/         → Unit tests for CRC, frame parsing, etc.
```

---

## 2. Adding a New MAVLink Message

### Step 1 – Create Message Class

All messages must be defined as classes or structs inside `MavCs.Core.Messages`, decorated with the `[MavMessage]` attribute.

```csharp
[MavMessage(Id = 150, CrcExtra = 0xA4)]
public class GpsRawIntMessage : IMavMessage
{
    public ulong TimeUsec { get; set; }
    public byte FixType { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public double Alt { get; set; }
}
```

> 🧠 **Tip:** Field order and types **must match** the MAVLink XML definition and respect little-endian encoding.

### Step 2 – Register Automatically

You do not need to manually modify any registry. When the program starts, `KnownMessages` uses reflection to discover all `[MavMessage]` classes and populates the internal CRC mapping automatically.

```csharp
var registry = new KnownMessages();
var crc = registry.GetCrcExtra(150); // returns 0xA4
```

### Step 3 – Test Encoding/Decoding

Create a unit test in `MavCs.Tests` to verify correctness:

```csharp
[Fact]
public void EncodeDecode_GpsRawInt_ShouldRoundTrip()
{
    var msg = new GpsRawIntMessage { FixType = 3, Lat = 41.02, Lon = 29.00 };
    var encoder = new MavLinkEncoder(new KnownMessages());
    var decoder = new MavLinkDecoder(new KnownMessages());
    var buf = new ArrayBufferWriter<byte>();

    encoder.WriteV2(msg, 1, 255, 190, buf);
    Assert.True(decoder.TryReadFrame(buf.WrittenSpan, out var frame, out _));
}
```

---

## 3. Code Style Guidelines

- Use **C# 10+ syntax** and consistent naming (`PascalCase` for public members, `camelCase` for locals).
- Avoid allocations: prefer `Span<byte>` and `ArrayBufferWriter<byte>`.
- Follow **SOLID** principles and maintain **protocol symmetry** (encode ≈ decode).
- Each new module should include XML-style summary comments.

---

## 4. Testing

All new code must include at least one unit test inside `MavCs.Tests`.
Run tests locally before submitting a PR:

```bash
dotnet test
```

Integration testing can be done using **MavCs.LiveTest** with a simulated ArduPilot instance:

```bash
sim_vehicle.py -v ArduCopter --out=127.0.0.1:14551
```

---

## 5. Pull Request Process

1. Create a new branch: `feature/<your-feature>` or `fix/<issue-id>`.
2. Follow the existing commit naming style:
   - Example: `fix(crc): correct table-driven lookup error`
   - Example: `feat(core): add sys_status message`
3. Push and open a PR on GitHub.
4. The CI will run `dotnet build` and `dotnet test`.
5. Once approved, merge into `main`.

---

## 6. Issue Labels

| Label | Meaning |
|--------|----------|
| `bug` | Incorrect behavior or crash |
| `enhancement` | Improvement or optimization |
| `message` | New MAVLink message or extension |
| `documentation` | Wiki, docs, or inline comments |
| `testing` | Unit/integration test addition |

---

## 7. Style & Documentation Checklist

- [ ] Class has XML doc summary.
- [ ] Unit test created or updated.
- [ ] CRC-extra verified from upstream MAVLink definition.
- [ ] Build passes without warnings.
- [ ] Public API reviewed for naming consistency.

---

## 8. Community Notes

- Discussions take place via GitHub Issues or Pull Requests.
- Respect the structure — PRs modifying multiple subsystems will be split.
- If unsure about a CRC, message layout, or type size — open an Issue labeled `clarification`.

---

### ✅ Summary
Contributing to **MavCs** means keeping it **protocol-accurate**, **performant**, and **cleanly structured**. Following these conventions ensures interoperability with reference MAVLink implementations and smooth collaboration among developers.
