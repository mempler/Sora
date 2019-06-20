#region LICENSE

/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

#endregion

namespace Sora.Enums
{
    public enum EventType
    {
        #region Bancho

        BanchoSendUserStatus = 0,
        BanchoSendIrcMessage = 1,
        BanchoExit = 2,
        BanchoRequestStatusUpdate = 3,
        BanchoPong = 4,
        BanchoStartSpectating = 16,
        BanchoStopSpectating = 17,
        BanchoBroadcastFrames = 18,
        BanchoErrorReport = 20,
        BanchoCantSpectate = 21,
        BanchoSendIrcMessagePrivate = 25,
        BanchoLobbyPart = 29,
        BanchoLobbyJoin = 30,
        BanchoMatchCreate = 31,
        BanchoMatchJoin = 32,
        BanchoMatchPart = 33,
        BanchoMatchChangeSlot = 38,
        BanchoMatchReady = 39,
        BanchoMatchLock = 40,
        BanchoMatchChangeSettings = 41,
        BanchoMatchStart = 44,
        BanchoMatchScoreUpdate = 47,
        BanchoMatchComplete = 49,
        BanchoMatchChangeMods = 51,
        BanchoMatchLoadComplete = 52,
        BanchoMatchNoBeatmap = 54,
        BanchoMatchNotReady = 55,
        BanchoMatchFailed = 56,
        BanchoMatchHasBeatmap = 59,
        BanchoMatchSkipRequest = 60,
        BanchoChannelJoin = 63,
        BanchoBeatmapInfoRequest = 68,
        BanchoMatchTransferHost = 70,
        BanchoFriendAdd = 73,
        BanchoFriendRemove = 74,
        BanchoMatchChangeTeam = 77,
        BanchoChannelLeave = 78,
        BanchoReceiveUpdates = 79,
        BanchoSetIrcAwayMessage = 82,
        BanchoUserStatsRequest = 85,
        BanchoInvite = 87,
        BanchoMatchChangePassword = 90,
        BanchoSpecialMatchInfoRequest = 93,
        BanchoUserPresenceRequest = 97,
        BanchoUserPresenceRequestAll = 98,
        BanchoUserToggleBlockNonFriendPm = 99,
        BanchoMatchAbort = 106,
        BanchoSpecialJoinMatchChannel = 108,
        BanchoSpecialLeaveMatchChannel = 109,

        BanchoPacket,
        BanchoLoginRequest,

        #endregion

        #region Other

        AntiCheat

        #endregion
    }
}
