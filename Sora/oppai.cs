using System;
using System.Runtime.InteropServices;
using Sora.Enums;
using Sora.Helpers;

namespace Sora
{
    public class oppai
    {
        [DllImport(@"lib/oppai")]
        private static extern IntPtr ezpp_new();

        [DllImport(@"lib/oppai")]
        private static extern int ezpp(IntPtr ez, string map);

        [DllImport(@"lib/oppai")]
        private static extern void ezpp_set_autocalc(IntPtr ez, int should);
        
        [DllImport(@"lib/oppai")]
        private static extern float ezpp_pp(IntPtr ez);

        [DllImport(@"lib/oppai")]
        private static extern void ezpp_free(IntPtr ez);

        [DllImport(@"lib/oppai")]
        private static extern void ezpp_set_mods(IntPtr ez, int mods);

        [DllImport(@"lib/oppai")]
        private static extern void ezpp_set_combo(IntPtr ez, int combo);

        [DllImport(@"lib/oppai")]
        private static extern void ezpp_set_nmiss(IntPtr ez, int nmiss);

        [DllImport(@"lib/oppai")]
        private static extern void ezpp_set_accuracy(IntPtr ez, int n100, int n50);

        private IntPtr _ez;

        private string _bm_path;

        public oppai(string bmPath)
        {
            _ez = ezpp_new();
            _bm_path = bmPath;
        }
        ~oppai() => ezpp_free(_ez);

        public void SetMods(Mod mods) => ezpp_set_mods(_ez, (int) mods);
        public void SetCombo(int combo) => ezpp_set_combo(_ez, combo);

        public void SetAcc(int c100, int c50, int cmiss)
        {
            ezpp_set_nmiss(_ez, cmiss);
            ezpp_set_accuracy(_ez, c100, c50);
        }

        public void Calculate()
        {
            int err = ezpp(_ez, _bm_path);
            if (err != 0)
                Logger.Err("Oppai returned an Error! Id", err);
        }

        public float GetPP() => ezpp_pp(_ez);
    }
}