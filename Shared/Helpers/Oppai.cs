using System;
using System.Runtime.InteropServices;

namespace Shared.Helpers
{
    public class Oppai
    {
        [DllImport(@"oppai")]
        public static extern IntPtr ezpp_new();
        [DllImport(@"oppai")]
        public static extern IntPtr ezpp(IntPtr ez, char[] map);
        [DllImport(@"oppai")]
        public static extern float ezpp_pp(IntPtr ez);
        [DllImport(@"oppai")]
        public static extern float ezpp_free(IntPtr ez);

        private IntPtr _ez;

        public Oppai(string bmData) => ezpp(_ez = ezpp_new(), bmData.ToCharArray());
        ~Oppai() => ezpp_free(_ez);
        public float PP => ezpp_pp(_ez);
    }
}