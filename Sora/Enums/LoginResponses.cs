namespace Sora.Enums
{
    public enum LoginResponses : int
    {
        Failed = -1,
        Outdated = -2,
        Banned = -3,
        Multiacc = -4,
        Exception = -5,
        Supporteronly = -6,
        Twofactorauth = -8
    }
}
