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

using JetBrains.Annotations;
using Shared.Database.Models;
using Shared.Enums;
using Shared.Handlers;
using Sora.Objects;
using Sora.Packets.Server;

namespace Sora.Handler
{
    public class MessageHandler
    {
        [UsedImplicitly]
        [Handler(HandlerTypes.ClientSendIrcMessage)]
        [Handler(HandlerTypes.ClientSendIrcMessagePrivate)]
        public void HandleMessage(Presence pr, MessageStruct message)
        {
            Channel chan = null;
            if (message.ChannelTarget.StartsWith("#"))
            {
                chan = Channels.GetChannel(message.ChannelTarget);
                if (chan == null)
                {
                    pr.Write(new ChannelRevoked(message.ChannelTarget));
                    return;
                }
            } else
            {
                // Other persence
                Presence opr = LPresences.GetPresence(Users.GetUserId(message.ChannelTarget));
                if (opr == null) return;
                opr.PrivateChannel.WriteMessage(pr, message.Message);
            }

            chan?.WriteMessage(pr, message.Message);
        }
    }
}