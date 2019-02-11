namespace EventManager.Enums
{
    public enum EventType
    {
        #region Bancho

        BanchoPacketHandler,
        BanchoLoginRequest,
        BanchoSendUserStatus,
        BanchoSendIrcMessage,
        BanchoExit,
        BanchoRequestStatusUpdate,
        BanchoPong,
        BanchoStartSpectating,
        BanchoStopSpectating,
        BanchoSpectateFrames,
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