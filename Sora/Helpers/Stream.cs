using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Sora.Enums;

namespace Sora.Helpers
{
    public class MStreamWriter : BinaryWriter
    {
        public MStreamWriter(Stream s) : base(s, Encoding.UTF8) { }

        public static MStreamWriter New()
        {
            var x = new MemoryStream(2048);
            return new MStreamWriter(x);
        }

        public void Write(IPacketSerializer packet)
        {
            Write((short)packet.Id);
            Write(false);
            var x = New();
            packet.Write_to_stream(x);
            Write((int)x.Length);
            Write(x);
        }

        public void Write(MStreamWriter w) => WriteRawBuffer(w.ToArray());
        public void Write(string value, bool nullable = false)
        {
            if (value == null && nullable)
                Write((byte)0x00);
            else
            {
                Write((byte)0x0b);
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
        public void Write<T>(List<T> list)
            where T : IPacketSerializer
        {
            var count = list.Count;
            Write(count);
            for (var i = 0; i < count; i++)
                list[i].Write_to_stream(this);
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
                        BinaryFormatter b = new BinaryFormatter()
                        {
                            AssemblyFormat = FormatterAssemblyStyle.Simple,
                            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
                        };
                        b.Serialize(BaseStream, obj);
                        break;
                }
            }
        }

        public long Length => OutStream.Length;
        public byte[] ToArray() => ((MemoryStream) OutStream).ToArray();
    }
    public class MStreamReader : BinaryReader
    {
        public MStreamReader(Stream s) : base(s, Encoding.UTF8) { }

        public override string ReadString()
        {
            return (ReadByte() == 0x00 ? null : base.ReadString()) ?? throw new InvalidOperationException();
        }

        public byte[] ReadBytes()
        {
            var len = ReadInt32();
            return len > 0 ? ReadBytes(len) : len < 0 ? null : (new byte[0]);
        }
        public List<T> ReadList<T>()
            where T : IPacketSerializer, new()
        {
            var count = ReadInt32();
            if (count < 0)
                return null;
            var outList = new List<T>(count);

            var sr = new MStreamReader(BaseStream);

            for (var i = 0; i < count; i++)
            {
                var obj = new T();
                obj.Read_from_stream(sr);
                outList.Add(obj);
            }

            return outList;
        }
    }
}
