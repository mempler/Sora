#region copyright
/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using System;

namespace Sora.Enums
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
