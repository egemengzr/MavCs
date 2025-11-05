namespace MavCs.Core.Messages;

/*
  <message id="30" name="ATTITUDE">
     <description>The attitude in the aeronautical frame (right-handed, Z-down, Y-right, X-front, ZYX, intrinsic).</description>
     <field type="uint32_t" name="time_boot_ms" units="ms">Timestamp (time since system boot).</field>
     <field type="float" name="roll" units="rad">Roll angle (-pi..+pi)</field>
     <field type="float" name="pitch" units="rad">Pitch angle (-pi..+pi)</field>
     <field type="float" name="yaw" units="rad">Yaw angle (-pi..+pi)</field>
     <field type="float" name="rollspeed" units="rad/s">Roll angular speed</field>
     <field type="float" name="pitchspeed" units="rad/s">Pitch angular speed</field>
     <field type="float" name="yawspeed" units="rad/s">Yaw angular speed</field>
   </message>
 */

[MavMessage(Id = 30u, CrcExtra = 39, Name = "ATTITUDE")]
public sealed class AttitudeMessage
{
    public uint TimeBootMs { get; set; }
    public float Roll { get; set; }
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public float RollSpeed { get; set; }
    public float PitchSpeed { get; set; }
    public float YawSpeed { get; set; }
}
