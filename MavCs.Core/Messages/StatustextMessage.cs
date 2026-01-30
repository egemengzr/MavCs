namespace MavCs.Core.Messages;

/*
  <message id="253" name="STATUSTEXT">
     <description>Status text message. These messages are printed in yellow in the COMM console of QGroundControl. WARNING: They consume quite some bandwidth, so use only for important status and error messages. If implemented wisely, these messages are buffered on the MCU and sent only at a limited rate (e.g. 10 Hz).</description>
     <field type="uint8_t" name="severity" enum="MAV_SEVERITY">Severity of status. Relies on the definitions within RFC-5424.</field>
     <field type="char[50]" name="text">Status text message, without null termination character</field>
     <extensions/>
     <field type="uint16_t" name="id">Unique (opaque) identifier for this statustext message.  May be used to reassemble a logical long-statustext message from a sequence of chunks.  A value of zero indicates this is the only chunk in the sequence and the message can be emitted immediately.</field>
     <field type="uint8_t" name="chunk_seq">This chunk's sequence number; indexing is from zero.  Any null character in the text field is taken to mean this was the last chunk.</field>
   </message>
 */

[MavMessage(Id = 253u, CrcExtra = 83, Name = "STATUSTEXT")]
public sealed class StatustextMessage
{
    // Wire Order
    public byte Severity { get; set; }
    public string? Text { get; set; }
    public ushort Id { get; set; }
    public byte ChunkSeq { get; set; }
    
}
