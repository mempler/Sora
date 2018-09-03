using System.IO;

namespace Kaoiji.helpers
{
    class Binary
    {
        public static void WriteOsuString(string s, BinaryWriter w)
        {
            if (s == null || s == "")
                    w.Write((byte)0x00);
            else
            {
                w.Write((byte)0x0b);
                w.Write(s);
            }
        }

        public static string ReadOsuString(BinaryReader r)
        {
            if (r.ReadByte() != 0x0b)
                return null;
            else
                return r.ReadString();
        }
    }
}
