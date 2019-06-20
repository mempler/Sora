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
    public enum PacketId : short
    {
        ClientSendUserStatus = 0,
        ClientSendIrcMessage = 1,
        ClientExit = 2,
        ClientRequestStatusUpdate = 3,
        ClientPong = 4,
        ServerLoginResponse = 5,
        ServerCommandError = 6,
        ServerSendMessage = 7,
        ServerPing = 8,
        ServerHandleIrcChangeUsername = 9,
        ServerHandleIrcQuit = 10,
        ServerHandleOsuUpdate = 11,
        ServerHandleUserQuit = 12,
        ServerSpectatorJoined = 13,
        ServerSpectatorLeft = 14,
        ServerSpectateFrames = 15,
        ClientStartSpectating = 16,
        ClientStopSpectating = 17,
        ClientSpectateFrames = 18,
        ServerVersionUpdate = 19,
        ClientErrorReport = 20,
        ClientCantSpectate = 21,
        ServerSpectatorCantSpectate = 22,
        ServerGetAttention = 23,
        ServerAnnounce = 24,
        ClientSendIrcMessagePrivate = 25,
        ServerMatchUpdate = 26,
        ServerMatchNew = 27,
        ServerMatchDisband = 28,
        ClientLobbyPart = 29,
        ClientLobbyJoin = 30,
        ClientMatchCreate = 31,
        ClientMatchJoin = 32,
        ClientMatchPart = 33,
        ServerMatchJoinSuccess = 36,
        ServerMatchJoinFail = 37,
        ClientMatchChangeSlot = 38,
        ClientMatchReady = 39,
        ClientMatchLock = 40,
        ClientMatchChangeSettings = 41,
        ServerFellowSpectatorJoined = 42,
        ServerFellowSpectatorLeft = 43,
        ClientMatchStart = 44,
        ServerMatchStart = 46,
        ClientMatchScoreUpdate = 47,
        ServerMatchScoreUpdate = 48,
        ClientMatchComplete = 49,
        ServerMatchTransferHost = 50,
        ClientMatchChangeMods = 51,
        ClientMatchLoadComplete = 52,
        ServerMatchAllPlayersLoaded = 53,
        ClientMatchNoBeatmap = 54,
        ClientMatchNotReady = 55,
        ClientMatchFailed = 56,
        ServerMatchPlayerFailed = 57,
        ServerMatchComplete = 58,
        ClientMatchHasBeatmap = 59,
        ClientMatchSkipRequest = 60,
        ServerMatchSkip = 61,
        ServerUnauthorised = 62,
        ClientChannelJoin = 63,
        ServerChannelJoinSuccess = 64,
        ServerChannelAvailable = 65,
        ServerChannelRevoked = 66,
        ServerChannelAvailableAutojoin = 67,
        ClientBeatmapInfoRequest = 68,
        ServerBeatmapInfoReply = 69,
        ClientMatchTransferHost = 70,
        ServerLoginPermissions = 71,
        ServerFriendsList = 72,
        ClientFriendAdd = 73,
        ClientFriendRemove = 74,
        ServerProtocolNegotiation = 75,
        ServerTitleUpdate = 76,
        ClientMatchChangeTeam = 77,
        ClientChannelLeave = 78,
        ClientReceiveUpdates = 79,
        ServerMonitor = 80,
        ServerMatchPlayerSkipped = 81,
        ClientSetIrcAwayMessage = 82,
        ServerUserPresence = 83,
        ClientUserStatsRequest = 85,
        ServerRestart = 86,
        ClientInvite = 87,
        ServerInvite = 88,
        ServerChannelListingComplete = 89,
        ClientMatchChangePassword = 90,
        ServerMatchChangePassword = 91,
        ServerBanInfo = 92,
        ClientSpecialMatchInfoRequest = 93,
        ServerUserSilenced = 94,
        ServerUserPresenceSingle = 95,
        ServerUserPresenceBundle = 96,
        ClientUserPresenceRequest = 97,
        ClientUserPresenceRequestAll = 98,
        ClientUserToggleBlockNonFriendPm = 99,
        ServerUserPmBlocked = 100,
        ServerTargetIsSilenced = 101,
        ServerVersionUpdateForced = 102,
        ServerSwitchServer = 103,
        ServerAccountRestricted = 104,
        ServerRtx = 105,
        ClientMatchAbort = 106,
        ServerSwitchTourneyServer = 107,
        ClientSpecialJoinMatchChannel = 108,
        ClientSpecialLeaveMatchChannel = 109
    }
}
