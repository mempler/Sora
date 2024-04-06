using System;

namespace Sora.Framework.Enums
{
    // Source https://github.com/HoLLy-HaCKeR/HOPEless/blob/master/HOPEless/osu/Enums_Multiplayer.cs under MIT License.
    [Flags]
    public enum MultiSlotStatus : byte
    {
        Open = 1 << 0,
        Locked = 1 << 1,
        NotReady = 1 << 2,
        Ready = 1 << 3,
        NoMap = 1 << 4,
        Playing = 1 << 5,
        Complete = 1 << 6,
        Quit = 1 << 7,
        Joinable = Open | Quit,
        NotJoinable = Locked | NotReady | Ready | NoMap | Playing | Complete
    }
}
