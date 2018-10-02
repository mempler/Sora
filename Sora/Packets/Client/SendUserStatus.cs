﻿#region copyright
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
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Enums;

namespace Sora.Packets.Client
{
    public class SendUserStatus : IPacketSerializer
    {
        public PacketId Id => PacketId.ClientSendUserStatus;

        public UserStatus Status;

        public void ReadFromStream(MStreamReader sr) =>
            Status = new UserStatus
            {
                Status = (Status) sr.ReadByte(),
                StatusText = sr.ReadString(),
                BeatmapChecksum = sr.ReadString(),
                CurrentMods = sr.ReadUInt32(),
                Playmode = (PlayModes) sr.ReadByte(),
                BeatmapId = sr.ReadUInt32()
            };

        public void WriteToStream(MStreamWriter sw) =>
            throw new NotImplementedException();
    }

    public struct UserStatus
    {
        public Status Status;
        public string StatusText;
        public string BeatmapChecksum;
        public uint CurrentMods;
        public PlayModes Playmode;
        public uint BeatmapId;
    }
}