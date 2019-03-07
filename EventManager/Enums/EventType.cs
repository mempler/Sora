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

namespace EventManager.Enums
{
    public enum EventType
    {
        #region Bancho

        BanchoPacket,
        BanchoLoginRequest,
        BanchoSendUserStatus,
        BanchoSendIrcMessage,
        BanchoExit,
        BanchoRequestStatusUpdate,
        BanchoPong,
        BanchoStartSpectating,
        BanchoStopSpectating,
        BanchoBroadcastFrames,
        BanchoErrorReport,
        BanchoCantSpectate,
        BanchoSendIrcMessagePrivate,
        BanchoLobbyPart,
        BanchoLobbyJoin,
        BanchoMatchCreate,
        BanchoMatchJoin,
        BanchoMatchPart,
        BanchoMatchChangeSlot,
        BanchoMatchReady,
        BanchoMatchLock,
        BanchoMatchChangeSettings,
        BanchoMatchStart,
        BanchoMatchScoreUpdate,
        BanchoMatchComplete,
        BanchoMatchChangeMods,
        BanchoMatchLoadComplete,
        BanchoMatchNoBeatmap,
        BanchoMatchNotReady,
        BanchoMatchFailed,
        BanchoMatchHasBeatmap,
        BanchoMatchSkipRequest,
        BanchoChannelJoin,
        BanchoBeatmapInfoRequest,
        BanchoMatchTransferHost,
        BanchoFriendAdd,
        BanchoFriendRemove,
        BanchoMatchChangeTeam,
        BanchoChannelLeave,
        BanchoReceiveUpdates,
        BanchoSetIrcAwayMessage,
        BanchoUserStatsRequest,
        BanchoInvite,
        BanchoMatchChangePassword,
        BanchoSpecialMatchInfoRequest,
        BanchoUserPresenceRequest,
        BanchoUserPresenceRequestAll,
        BanchoUserToggleBlockNonFriendPm,
        BanchoMatchAbort,
        BanchoSpecialJoinMatchChannel,
        BanchoSpecialLeaveMatchChannel,

        #endregion

        #region PrivateAPI

        SharedAvatars,
        SharedScoreboardRequest,
        SharedScoreSubmittion,
        SharedGetReplay,

        #endregion
    }
}