using System.IO;

namespace Kaoiji.helpers
{
    class Binary
    {
        public static byte[] WriteOsuString(string s)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            
            if (s == null || s == "")
            {
                bw.Write((byte)0x00);
            } else {
                bw.Write((byte)0x0b);
                bw.Write(s);
            }
            bw.Flush();
            bw.Close();
            return ms.ToArray();
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
