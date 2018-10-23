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

using Shared.Database.Models;
using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;
using Sora.Objects;
using Sora.Packets.Client;
using SendIrcMessage = Sora.Packets.Client.SendIrcMessage;

namespace Sora.Handler
{
    internal class PacketHandler
    {
        [Handler(HandlerTypes.PacketHandler)]
        public void HandlePackets(Presence pr, PacketId packetId, MStreamReader packetDataReader)
        {
            switch (packetId)
            {
                case PacketId.ClientSendUserStatus:
                    SendUserStatus sendUserStatus = new SendUserStatus();
                    sendUserStatus.ReadFromStream(packetDataReader);
                    Handlers.ExecuteHandler(HandlerTypes.ClientSendUserStatus, pr, sendUserStatus.Status);
                    break;
                case PacketId.ClientPong:
                    Handlers.ExecuteHandler(HandlerTypes.ClientPong, pr);
                    break;
                case PacketId.ClientRequestStatusUpdate:
                    Handlers.ExecuteHandler(HandlerTypes.ClientRequestStatusUpdate, pr);
                    break;
                case PacketId.ClientUserStatsRequest:
                    UserStatsRequest userStatsRequest = new UserStatsRequest();
                    userStatsRequest.ReadFromStream(packetDataReader);
                    Handlers.ExecuteHandler(HandlerTypes.ClientUserStatsRequest, pr, userStatsRequest.Userids);
                    break;
                case PacketId.ClientChannelJoin:
                    ChannelJoin channelJoin = new ChannelJoin();
                    channelJoin.ReadFromStream(packetDataReader);
                    Handlers.ExecuteHandler(HandlerTypes.ClientChannelJoin, pr, channelJoin.ChannelName);
                    break;
                case PacketId.ClientChannelLeave:
                    ChannelLeave channelLeave = new ChannelLeave();
                    channelLeave.ReadFromStream(packetDataReader);
                    Handlers.ExecuteHandler(HandlerTypes.ClientChannelLeave, pr, channelLeave.ChannelName);
                    break;
                case PacketId.ClientSendIrcMessagePrivate:
                case PacketId.ClientSendIrcMessage:
                    SendIrcMessage msg = new SendIrcMessage();
                    msg.ReadFromStream(packetDataReader);
                    if (msg.Msg.ChannelTarget.StartsWith("#"))
                        Handlers.ExecuteHandler(HandlerTypes.ClientSendIrcMessage, pr, msg.Msg);
                    else
                        Handlers.ExecuteHandler(HandlerTypes.ClientSendIrcMessagePrivate, pr, msg.Msg);
                    break;
                case PacketId.ClientExit:
                    Exit exit = new Exit();
                    exit.ReadFromStream(packetDataReader);
                    Handlers.ExecuteHandler(HandlerTypes.ClientExit, pr, exit.ErrorState);
                    break;
                case PacketId.ClientFriendAdd:
                    FriendAdd friendAdd = new FriendAdd();
                    friendAdd.ReadFromStream(packetDataReader);
                    Handlers.ExecuteHandler(HandlerTypes.ClientFriendAdd, pr, friendAdd.FriendId);
                    break;
                case PacketId.ClientFriendRemove:
                    FriendRemove friendRemove = new FriendRemove();
                    friendRemove.ReadFromStream(packetDataReader);
                    Handlers.ExecuteHandler(HandlerTypes.ClientFriendRemove, pr, friendRemove.FriendId);
                    break;
                case PacketId.ClientStartSpectating:
                    StartSpectating startSpectating = new StartSpectating();
                    startSpectating.ReadFromStream(packetDataReader);
                    Handlers.ExecuteHandler(HandlerTypes.ClientStartSpectating, pr, startSpectating.ToSpectateId);
                    break;
                case PacketId.ClientStopSpectating:
                    Handlers.ExecuteHandler(HandlerTypes.ClientStopSpectating, pr);
                    break;
                case PacketId.ClientSpectateFrames:
                    SpectatorFrames spectateFrames = new SpectatorFrames();
                    spectateFrames.ReadFromStream(packetDataReader);
                    Handlers.ExecuteHandler(HandlerTypes.ClientSpectateFrames, pr, spectateFrames.Frames);
                    break;
                default:
                    Logger.L.Debug($"PacketId: {packetId}");
                    break;
            }
        }
    }
}