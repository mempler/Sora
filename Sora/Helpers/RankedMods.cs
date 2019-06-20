using Sora.Enums;

namespace Sora.Helpers
{
    public static class RankedMods
    {
        public static bool IsRanked(Mod mods)
        {
            var r = (mods & Mod.Relax2) == 0;
            if ((mods & Mod.Cinema) != 0)
                r = false;
            if ((mods & Mod.Autoplay) != 0)
                r = false;
            if ((mods & Mod.TargetPractice) != 0)
                r = false;
            if ((mods & Mod.HardRock) != 0 && (mods & Mod.Easy) != 0)
                r = false;
            return r;
        }
    }
}
