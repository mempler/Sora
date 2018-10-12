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

using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Objects;

namespace Sora.Packets.Server
{
    public class ChannelAvailableAutojoin : IPacket
    {
        public Channel Channel;

        public ChannelAvailableAutojoin(Channel channel) { Channel = channel; }

        public PacketId Id => PacketId.ServerChannelAvailableAutojoin;

        public void ReadFromStream(MStreamReader sr)
        {
            Channel = new Channel(sr.ReadString(), sr.ReadString())
                { UserCount = sr.ReadInt32() };
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Channel.ChannelName, false);
            sw.Write(Channel.ChannelTopic, true);
            sw.Write((short) Channel.UserCount);
        }
    }
}