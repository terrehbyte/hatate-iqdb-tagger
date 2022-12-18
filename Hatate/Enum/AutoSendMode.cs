namespace Hatate.Enum
{
    public enum AutoSendMode : byte
    {
        Undefined = 0,
        Update = 1,     // updates the existing file record
        New = 2,        // sends as URL only
        Both = 3,       // updates the existing file record and sends it as a new URL
    }
}