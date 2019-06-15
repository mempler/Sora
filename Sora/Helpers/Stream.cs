#region LICENSE
/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Sora.Interfaces;

namespace Sora.Helpers
{
    public class MStreamWriter : BinaryWriter
    {
        public MStreamWriter(Stream s) : base(s, Encoding.UTF8)
        {
        }

        public long Length => BaseStream.Length;
        
        public static MStreamWriter New()
        {
            return new MStreamWriter(new MemoryStream());
        }

        public void Write(ISerializer seri)
        {
            seri.WriteToStream(this);
        }

        public void Write(IPacket packet)
        {
            using MStreamWriter x = New();
            
            base.Write((short) packet.Id);
            base.Write((byte) 0);
            /* Packet Data */
            x.Write((ISerializer) packet);
            /* END */
            base.Write((int) x.Length);
            Write(x);
            
            x.Close();
        }

        public void Write(BinaryWriter w)
        {
            w.BaseStream.Position = 0;
            w.BaseStream.CopyTo(BaseStream);
        }

        public void Write(string value, bool nullable)
        {
            if (value == null && nullable)
            {
                base.Write((byte) 0);
            }
            else
            {
                base.Write((byte) 0x0b);
                base.Write(value + "");
            }
        }

        public override void Write(string value)
        {
            Write((byte) 0x0b);
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
            short count = (short) list.Count;
            Write(count);
            for (int i = 0; i < count; i++) Write(list[i]);
        }

        public void WriteRawBuffer(byte[] buff)
        {
            base.Write(buff);
        }

        public void WriteRawString(string value)
        {
            WriteRawBuffer(Encoding.UTF8.GetBytes(value));
        }

        public void WriteObject(object obj)
        {
            if (obj == null)
                Write((byte) 0x00);
            else
                switch (obj.GetType().Name)
                {
                    case "Boolean":
                        Write((bool) obj);
                        break;
                    case "Byte":
                        Write((byte) obj);
                        break;
                    case "UInt16":
                        Write((ushort) obj);
                        break;
                    case "UInt32":
                        Write((uint) obj);
                        break;
                    case "UInt64":
                        Write((ulong) obj);
                        break;
                    case "SByte":
                        Write((sbyte) obj);
                        break;
                    case "Int16":
                        Write((short) obj);
                        break;
                    case "Int32":
                        Write((int) obj);
                        break;
                    case "Int64":
                        Write((long) obj);
                        break;
                    case "Char":
                        Write((char) obj);
                        break;
                    case "String":
                        Write((string) obj);
                        break;
                    case "Single":
                        Write((float) obj);
                        break;
                    case "Double":
                        Write((double) obj);
                        break;
                    case "Decimal":
                        Write((decimal) obj);
                        break;
                    default:
                        BinaryFormatter b = new BinaryFormatter
                        {
                            AssemblyFormat = FormatterAssemblyStyle.Simple,
                            TypeFormat     = FormatterTypeStyle.TypesWhenNeeded
                        };
                        b.Serialize(BaseStream, obj);
                        break;
                }
        }

        public byte[] ToArray()
        {
            return ((MemoryStream) BaseStream).ToArray();
        }
    }

    public class MStreamReader : BinaryReader
    {
        public MStreamReader(Stream s) : base(s, Encoding.UTF8)
        {
        }

        public override string ReadString()
        {
            return (ReadByte() == 0x00 ? "" : base.ReadString()) ?? throw new InvalidOperationException();
        }

        public byte[] ReadBytes()
        {
            int len = ReadInt32();
            return len > 0 ? base.ReadBytes(len) : len < 0 ? null : new byte[0];
        }

        public List<int> ReadInt32List()
        {
            short count = ReadInt16();
            if (count < 0)
                return new List<int>();
            List<int> outList = new List<int>(count);
            for (int i = 0; i < count; i++)
                outList.Add(ReadInt32());
            return outList;
        }

        public T ReadPacket<T>() where T : IPacket, new()
        {
            T packet = new T();
            ReadInt16();
            ReadByte();
            byte[] rawPacketData = ReadBytes();
            using (MStreamWriter x = MStreamWriter.New())
            {
                x.WriteRawBuffer(rawPacketData);
                x.BaseStream.Position = 0;
                packet.ReadFromStream(new MStreamReader(x.BaseStream));
            }

            return packet;
        }

        public T ReadData<T>() where T : ISerializer, new()
        {
            T data = new T();
            data.ReadFromStream(this);
            return data;
        }

        public byte[] ReadToEnd()
        {
            List<byte> x = new List<byte>();
            while (BaseStream.Position != BaseStream.Length) x.Add(ReadByte());

            return x.ToArray();
        }
    }
}