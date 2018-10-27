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
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Objects;

namespace Sora.Packets.Server
{
    public class UserPresence : IPacket
    {
        public PacketId Id => PacketId.ServerUserPresence;

        public readonly Presence Presence;

        public UserPresence(Presence pr) => Presence = pr;

        public void ReadFromStream(MStreamReader sr) => throw new NotImplementedException();

        public void WriteToStream(MStreamWriter sw)
        {
            if (Presence == null)
                throw new ArgumentNullException();

            sw.Write(Presence.User.Id);
            sw.Write(Presence.User.Username, false);
            sw.Write(Presence.Timezone);
            sw.Write((byte) Presence.CountryId);
            sw.Write(Presence.ClientPermissions);
            sw.Write(Presence.Lat);
            sw.Write(Presence.Lon);
            sw.Write(Presence.Rank);
        }
    }
}
