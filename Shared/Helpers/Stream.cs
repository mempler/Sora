#region copyright
/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

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

        public static MStreamWriter New() => new MStreamWriter(new MemoryStream());

        public void Write(IPacket packet)
        {
            base.Write((short)packet.Id);
            base.Write((byte)0);
            /* Packet Data */
            MStreamWriter x = New();
            packet.WriteToStream(x);
            /* END */
            base.Write((int)x.Length);
            this.Write(x);
            x.Close();
        }

        public void Write(BinaryWriter w)
        {
            w.BaseStream.Position = 0;
            w.BaseStream.CopyTo(this.BaseStream);
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
            this.Write((byte)0x0b);
            base.Write(value);
        }
        public override void Write(byte[] buff)
        {
            int length = buff.Length;
            this.Write(length);
            if (length > 0)
                base.Write(buff);
        }
        public void Write(List<int> list)
        {
            int count = list.Count;
            this.Write(count);
            for (int i = 0; i < count; i++) this.Write(list[i]);
        }
        public void WriteRawBuffer(byte[] buff) => base.Write(buff);
        public void WriteRawString(string value) => this.WriteRawBuffer(Encoding.UTF8.GetBytes(value));
        public void WriteObject(object obj)
        {
            if (obj == null)
                this.Write((byte)0x00);
            else
            {
                switch (obj.GetType().Name)
                {
                    case "Boolean":
                        this.Write((bool)obj);
                        break;
                    case "Byte":
                        this.Write((byte)obj);
                        break;
                    case "UInt16":
                        this.Write((ushort)obj);
                        break;
                    case "UInt32":
                        this.Write((uint)obj);
                        break;
                    case "UInt64":
                        this.Write((ulong)obj);
                        break;
                    case "SByte":
                        this.Write((sbyte)obj);
                        break;
                    case "Int16":
                        this.Write((short)obj);
                        break;
                    case "Int32":
                        this.Write((int)obj);
                        break;
                    case "Int64":
                        this.Write((long)obj);
                        break;
                    case "Char":
                        this.Write((char)obj);
                        break;
                    case "String":
                        this.Write((string)obj);
                        break;
                    case "Single":
                        this.Write((float)obj);
                        break;
                    case "Double":
                        this.Write((double)obj);
                        break;
                    case "Decimal":
                        this.Write((decimal)obj);
                        break;
                    default:
                        BinaryFormatter b = new BinaryFormatter()
                        {
                            AssemblyFormat = FormatterAssemblyStyle.Simple,
                            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
                        };
                        b.Serialize(this.BaseStream, obj);
                        break;
                }
            }
        }

        public long Length => this.BaseStream.Length;
        public byte[] ToArray() => ((MemoryStream) this.BaseStream).ToArray();
    }
    public class MStreamReader : BinaryReader
    {
        public MStreamReader(Stream s) : base(s, Encoding.UTF8) { }

        public override string ReadString() => (this.ReadByte() == 0x00 ? "" : base.ReadString()) ?? throw new InvalidOperationException();

        public byte[] ReadBytes()
        {
            int len = this.ReadInt32();
            return len > 0 ? base.ReadBytes(len) : len < 0 ? null : (new byte[0]);
        }
        public List<int> ReadInt32List()
        {
            short count = this.ReadInt16();
            if (count < 0)
                return null;
            var outList = new List<int>(count);
            for (int i = 0; i < count; i++)
                outList.Add(this.ReadInt32());
            return outList;
        }
    }
}
