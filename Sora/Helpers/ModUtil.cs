using Sora.Enums;

namespace Sora.Helpers
{
    public static class ModUtil
    {
        public static string ToString(Mod mods)
        {
            var ret = string.Empty;

            if (mods == Mod.None)
                ret = "NoMod";
            if ((mods & Mod.NoFail) > 0)
                ret += "NF";
            if ((mods & Mod.Easy) > 0)
                ret += "EZ";
            if ((mods & Mod.TouchDevice) > 0)
                ret += "TD";
            if ((mods & Mod.Hidden) > 0)
                ret += "HD";
            if ((mods & Mod.HardRock) > 0)
                ret += "HR";
            if ((mods & Mod.SuddenDeath) > 0)
                ret += "SD";
            if ((mods & Mod.DoubleTime) > 0)
                ret += "DT";
            if ((mods & Mod.Relax) > 0)
                ret += "RX";
            if ((mods & Mod.HalfTime) > 0)
                ret += "HT";
            if ((mods & Mod.Nightcore) > 0)
            {
                ret += "NC";
                ret = ret.Replace("DT", "");
            }

            if ((mods & Mod.Flashlight) > 0)
                ret += "FL";
            if ((mods & Mod.Autoplay) > 0)
                ret += "Auto";
            if ((mods & Mod.SpunOut) > 0)
                ret += "SO";
            if ((mods & Mod.Relax2) > 0)
                ret += "AP";
            if ((mods & Mod.Perfect) > 0)
                ret += "PF";
            if ((mods & Mod.Key4) > 0)
                ret += "K4";
            if ((mods & Mod.Key5) > 0)
                ret += "K5";
            if ((mods & Mod.Key6) > 0)
                ret += "K6";
            if ((mods & Mod.Key7) > 0)
                ret += "K7";
            if ((mods & Mod.Key8) > 0)
                ret += "K8";
            if ((mods & Mod.FadeIn) > 0)
                ret += "FI";
            if ((mods & Mod.Random) > 0)
                ret += "RD";
            if ((mods & Mod.TargetPractice) > 0)
                ret += "TP";
            if ((mods & Mod.Key9) > 0)
                ret += "K9";
            if ((mods & Mod.Coop) > 0)
                ret += "COOP";
            if ((mods & Mod.Key1) > 0)
                ret += "K1";
            if ((mods & Mod.Key2) > 0)
                ret += "K2";
            if ((mods & Mod.Key3) > 0)
                ret += "K3";

            return ret;
        }
    }
}
