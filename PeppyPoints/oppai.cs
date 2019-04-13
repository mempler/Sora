using System;
using System.Runtime.InteropServices;
using Shared.Enums;

namespace PeppyPoints
{
    public class oppai
    {
        [DllImport(@"oppai")]
        private static extern IntPtr ezpp_new();

        [DllImport(@"oppai")]
        private static extern int ezpp(IntPtr ez, string map);

        [DllImport(@"oppai")]
        private static extern void ezpp_set_autocalc(IntPtr ez, int should);
        
        [DllImport(@"oppai")]
        private static extern float ezpp_pp(IntPtr ez);

        [DllImport(@"oppai")]
        private static extern void ezpp_free(IntPtr ez);

        [DllImport(@"oppai")]
        private static extern void ezpp_set_mods(IntPtr ez, int mods);

        [DllImport(@"oppai")]
        private static extern void ezpp_set_combo(IntPtr ez, int combo);

        [DllImport(@"oppai")]
        private static extern void ezpp_set_nmiss(IntPtr ez, int nmiss);

        [DllImport(@"oppai")]
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
                Console.WriteLine(err);
        }

        public float GetPP() => ezpp_pp(_ez);
    }
}