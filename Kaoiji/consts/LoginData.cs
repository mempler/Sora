namespace Kaoiji.consts
{
    public class LoginData
    {
        public string Username;
        public string Password;
        public string OsuVersion;
        public int TimeOff;
        public SecurityHash SecurityHash;
        public bool BlockNonFriendsDM;

        public override string ToString() =>
             $"Username: {Username} Password: {Password} Osu Version: {OsuVersion} TimeOffset: {TimeOff * 12} SecurityHash: {SecurityHash} Block Nonfriend DMs {BlockNonFriendsDM}";
    }
    public class SecurityHash
    {
        public string OsuHash;
        public string DiskMD5;
        public string UniqueMD5;
        public override string ToString() =>
            $"OsuHash: {OsuHash} DiskMD5: {DiskMD5} UniqueMD5: {UniqueMD5}";
    }
}
