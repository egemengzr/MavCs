namespace MavCs.Core.Protocol;

public static class Constants
{
    // Magic bytes
    public const byte MagicV1 = 0xFE;
    public const byte MagicV2 = 0xFD;
    
    // Header sizes (excluding magic)
    public const int HeaderV1Size = 5; /* len, seq, sys, comp, msgid(1) */
    public const int HeaderV2Size = 9; /* len, incompat, compat, seq, sys, comp, msgid(3) */
    
    // CRC Size
    public const int CrcSize = 2;
    
    // Signature Size for v2
    public const int V2SignatureSize = 13;
}
