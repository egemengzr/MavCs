namespace MavCs.Core.Messages;

[MavMessage(Id = 36u, CrcExtra = 222, Name = "SERVO_OUTPUT_RAW")]
public sealed class ServoOutputRawMessage
{
    // Wire Order: time_usec, servo1_raw .. servo16_raw, port
    public uint TimeUsec { get; set; }
    public ushort Servo1Raw { get; set; }
    public ushort Servo2Raw { get; set; }
    public ushort Servo3Raw { get; set; }
    public ushort Servo4Raw { get; set; }
    public ushort Servo5Raw { get; set; }
    public ushort Servo6Raw { get; set; }
    public ushort Servo7Raw { get; set; }
    public ushort Servo8Raw { get; set; }
    
    public byte Port { get; set; }
}
