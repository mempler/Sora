using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Shared.Interfaces;

namespace Shared.Helpers
{
    public class MStreamWriter : BinaryWriter
    {
        public MStreamWriter(Stream s) : base(s, Encoding.UTF8) { }

        public static MStreamWriter New()
        {
            var x = new MemoryStream();
            return new MStreamWriter(x);
        }

        public void Write(IPacketSerializer packet)
        {
            base.Write((short)packet.Id);
            base.Write((byte)0);
            /* Packet Data */
            var x = New();
            packet.WriteToStream(x);
            /* END */
            base.Write((int)x.Length);
            Write(x);
            x.Close();
        }

        public void Write(MStreamWriter w)
        {
            w.BaseStream.Position = 0;
            w.BaseStream.CopyTo(BaseStream);
        }
        public void Write(string value, bool nullable)
        {
            if (value == null && nullable)
                base.Write((byte)0);
            else
            {
                base.Write((byte)0x0b);
                base.Write(value + "");
            }
        }
        public override void Write(string value)
        {
            Write((byte)0x0b);
            base.Write(value);
        }
        public override void Write(byte[] buff)
        {
            int length = buff.Length;
            Write(length);
            if (length > 0)
                base.Write(buff);
        }
        public void Write(List<int> list)
        {
            var count = list.Count;
            Write(count);
            for (var i = 0; i < count; i++)
                Write(list[i]);
        }
        public void WriteRawBuffer(byte[] buff) => base.Write(buff);
        public void WriteObject(object obj)
        {
            if (obj == null)
                Write((byte)0x00);
            else
            {
                switch (obj.GetType().Name)
                {
                    case "Boolean":
                        Write((bool)obj);
                        break;
                    case "Byte":
                        Write((byte)obj);
                        break;
                    case "UInt16":
                        Write((ushort)obj);
                        break;
                    case "UInt32":
                        Write((uint)obj);
                        break;
                    case "UInt64":
                        Write((ulong)obj);
                        break;
                    case "SByte":
                        Write((sbyte)obj);
                        break;
                    case "Int16":
                        Write((short)obj);
                        break;
                    case "Int32":
                        Write((int)obj);
                        break;
                    case "Int64":
                        Write((long)obj);
                        break;
                    case "Char":
                        Write((char)obj);
                        break;
                    case "String":
                        Write((string)obj);
                        break;
                    case "Single":
                        Write((float)obj);
                        break;
                    case "Double":
                        Write((double)obj);
                        break;
                    case "Decimal":
                        Write((decimal)obj);
                        break;
                    default:
                        var b = new BinaryFormatter()
                        {
                            AssemblyFormat = FormatterAssemblyStyle.Simple,
                            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
                        };
                        b.Serialize(BaseStream, obj);
                        break;
                }
            }
        }

        public long Length => BaseStream.Length;
        public byte[] ToArray() => ((MemoryStream) BaseStream).ToArray();
    }
    public class MStreamReader : BinaryReader
    {
        public MStreamReader(Stream s) : base(s, Encoding.UTF8) { }

        public override string ReadString()
        {
            return (ReadByte() == 0x00 ? "" : base.ReadString()) ?? throw new InvalidOperationException();
        }

        public byte[] ReadBytes()
        {
            var len = ReadInt32();
            return len > 0 ? base.ReadBytes(len) : len < 0 ? null : (new byte[0]);
        }
        public List<int> ReadInt32List()
        {
            var count = ReadInt32();
            if (count < 0)
                return null;
            var outList = new List<int>(count);
            for (var i = 0; i < count; i++)
            {
                outList.Add(ReadInt32());
            }
            return outList;
        }
    }
}
