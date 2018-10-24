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

using System.Collections.Generic;
using Shared.Enums;
using Shared.Handlers;
using Sora.Objects;
using Sora.Packets.Server;

namespace Sora.Handler
{
    public class MultiplayerHandler
    {
        #region Lobby
        [Handler(HandlerTypes.ClientLobbyJoin)]
        public void OnLobbyJoin(Presence pr)
        {
            PacketStream lobbyStream = LPacketStreams.GetStream("lobby");
            
            IEnumerable<MultiplayerRoom> rooms = MultiplayerLobby.GetRooms();
            foreach (MultiplayerRoom room in rooms)
                pr.Write(new MatchNew(room));
            
            lobbyStream.Join(pr);
        }
        [Handler(HandlerTypes.ClientLobbyPart)]
        public void OnLobbyLeft(Presence pr)
        {
            PacketStream lobbyStream = LPacketStreams.GetStream("lobby");
            
            IEnumerable<MultiplayerRoom> rooms = MultiplayerLobby.GetRooms();
            foreach (MultiplayerRoom room in rooms)
                pr.Write(new MatchDisband(room));

            lobbyStream.Left(pr);
        }
        #endregion

        #region Match
        [Handler(HandlerTypes.ClientMatchCreate)]
        public void OnMatchCreate(Presence pr, MultiplayerRoom room)
        {
            PacketStream lobbyStream = LPacketStreams.GetStream("lobby");
            
            if (room.Join(pr))
                pr.Write(new MatchJoinSuccess(room));
            else
                pr.Write(new MatchJoinFail());

            MultiplayerLobby.Add(room);
            lobbyStream.Broadcast(new MatchNew(room));
        }

        [Handler(HandlerTypes.ClientMatchPart)]
        public void OnMatchLeave(Presence pr)
        {
            if (pr.JoinedRoom == null) return;
            
            MultiplayerRoom room = pr.JoinedRoom;
            PacketStream lobbyStream = LPacketStreams.GetStream("lobby");
            pr.JoinedRoom.Leave(pr);
            
            lobbyStream.Broadcast(new MatchNew(room));
        }
        #endregion
    }
}