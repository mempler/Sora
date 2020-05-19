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
