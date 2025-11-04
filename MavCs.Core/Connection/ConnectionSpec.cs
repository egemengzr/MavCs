using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MavCs.Core.Connection;

internal enum ConnectionKind { Udp, Serial }

internal sealed class ConnectionSpec
{
    public ConnectionKind Kind { get; private set; }
    
    // UDP
    public string? Host { get; private set; }
    public int? RemotePort { get; private set; }
    public int? LocalPort { get; private set; }
    
    // Serial
    public string? PortName { get; private set; }
    public int? Baud { get; private set; }
    
    public int SysId { get; private set; }
    public int CompId { get; private set; }

    public static ConnectionSpec Parse(string input, int? sysId, int? compId)
    {
        var spec = new ConnectionSpec
        {
            SysId = sysId ?? 255,
            CompId = compId ?? 190
        };

        input = input.Trim();
        var (path, query) = SplitQuery(input);
        var qp = ParseQuery(query);

        if (path.StartsWith("udp:", StringComparison.OrdinalIgnoreCase))
        {
            spec.Kind = ConnectionKind.Udp;
            ParseUdp(path.Substring(4), qp, spec);
        }
        else if (path.StartsWith("serial:", StringComparison.OrdinalIgnoreCase))
        {
            spec.Kind = ConnectionKind.Serial;
            ParseSerial(path.Substring(7), qp, spec);
        }
        else
        {
            // Heuristic: "host:port" => UDP
            if (Regex.IsMatch(path, @"^[^:\s]+:\d+$"))
            {
                spec.Kind = ConnectionKind.Udp;
                ParseUdp(path, qp, spec);
            }
            else
            {
                spec.Kind = ConnectionKind.Serial;
                ParseSerial(path, qp, spec);
            }
        }

        if (qp.TryGetValue("sysid", out var sid) && int.TryParse(sid, out var sId)) spec.SysId = sId;
        if (qp.TryGetValue("compid", out var cid) && int.TryParse(cid, out var cId)) spec.CompId = cId;

        return spec;
    }

    private static void ParseUdp(string body, Dictionary<string,string> qp, ConnectionSpec spec)
    {
        var parts = body.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) throw new FormatException("udp:<host>:<port> expected");
        spec.Host = parts[0];
        if (!int.TryParse(parts[1], out var p)) throw new FormatException("Invalid UDP port");

        // If there is one port: local=p, remote=p+1; ?local= or ?remote= override
        int local = qp.TryGetValue("local", out var locStr) && int.TryParse(locStr, out var loc) ? loc : p;
        int remote = qp.TryGetValue("remote", out var remStr) && int.TryParse(remStr, out var rem) ? rem : (p + 1);

        spec.LocalPort = local;
        spec.RemotePort = remote;
    }
    
    private static void ParseSerial(string body, Dictionary<string,string> qp, ConnectionSpec spec)
    {
        if (string.IsNullOrWhiteSpace(body)) throw new FormatException("serial:<port> expected");
        spec.PortName = body;
        spec.Baud = qp.TryGetValue("baud", out var b) && int.TryParse(b, out var v) ? v : 115200;
    }

    private static (string path, string query) SplitQuery(string input)
    {
        var i = input.IndexOf('?', StringComparison.Ordinal);
        return i < 0 ? (input, "") : (input[..i], input[(i+1)..]);
    }

    private static Dictionary<string,string> ParseQuery(string q)
    {
        var d = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(q)) return d;
        foreach (var kv in q.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var p = kv.Split('=', 2);
            var k = Uri.UnescapeDataString(p[0]);
            var v = p.Length > 1 ? Uri.UnescapeDataString(p[1]) : "";
            d[k] = v;
        }
        return d;
    }
}
