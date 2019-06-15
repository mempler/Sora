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
using System.Text;
using IPacket = Sora.Interfaces.IPacket;
using ISerializer = Sora.Interfaces.ISerializer;
using MStreamReader = Sora.Helpers.MStreamReader;
using MStreamWriter = Sora.Helpers.MStreamWriter;
using PacketId = Sora.Enums.PacketId;

namespace Sora.Packets.Client
{
    public class SpectatorFrames : IPacket
    {
        public SpectatorFrame Frames;
        public PacketId Id => PacketId.ClientSpectateFrames;

        public void ReadFromStream(MStreamReader sr)
        {
            Frames = sr.ReadData<SpectatorFrame>();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new NotImplementedException();
        }
    }

    public class ScoreFrame : ISerializer
    {
        public double BonusPortion;
        public double ComboPortion;
        public ushort Count100;

        public ushort Count300;
        public ushort Count50;
        public ushort CountGeki;
        public ushort CountKatu;
        public ushort CountMiss;
        public ushort CurrentCombo;

        public byte CurrentHp;
        public byte Id;
        public ushort MaxCombo;
        public bool Perfect;

        public bool ScoreV2;
        public byte TagByte;
        public int Time;

        public int TotalScore;

        public void ReadFromStream(MStreamReader sr)
        {
            Time         = sr.ReadInt32();
            Id           = sr.ReadByte();
            Count300     = sr.ReadUInt16();
            Count100     = sr.ReadUInt16();
            Count50      = sr.ReadUInt16();
            CountGeki    = sr.ReadUInt16();
            CountKatu    = sr.ReadUInt16();
            CountMiss    = sr.ReadUInt16();
            TotalScore   = sr.ReadInt32();
            MaxCombo     = sr.ReadUInt16();
            CurrentCombo = sr.ReadUInt16();
            Perfect      = sr.ReadBoolean();
            CurrentHp    = sr.ReadByte();
            TagByte      = sr.ReadByte();
            ScoreV2      = sr.ReadBoolean();
            ComboPortion = ScoreV2 ? sr.ReadDouble() : 0;
            BonusPortion = ScoreV2 ? sr.ReadDouble() : 0;
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Time);
            sw.Write(Id);
            sw.Write(Count300);
            sw.Write(Count100);
            sw.Write(Count50);
            sw.Write(CountGeki);
            sw.Write(CountKatu);
            sw.Write(CountMiss);
            sw.Write(TotalScore);
            sw.Write(MaxCombo);
            sw.Write(CurrentCombo);
            sw.Write(Perfect);
            sw.Write(CurrentHp);
            sw.Write(TagByte);
            sw.Write(ScoreV2);
            if (!ScoreV2) return;
            sw.Write(ComboPortion);
            sw.Write(BonusPortion);
        }

        public override string ToString()
        {
            return $"Time: {Time} Id: {Id}\n" +
                   $"Count300: {Count300} Count100: {Count100} Count50: {Count50} " +
                   $"CountGeki: {CountGeki} CountKatu: {CountKatu} CountMiss: {CountMiss}\n" +
                   $"TotalScore: {TotalScore} MaxCombo: {MaxCombo} CurrentCombo: {CurrentCombo} Perfect: {Perfect}\n" +
                   $"CurrentHp: {CurrentHp} TagByte: {TagByte}\n" +
                   $"ScoreV2: {ScoreV2} ComboPortion: {ComboPortion} BonusPortion: {BonusPortion}";
        }
    }

    public class SpectatorFrame : ISerializer
    {
        public byte Action;
        public int Extra;
        public List<ReplayFrame> ReplayFrames;
        public ScoreFrame ScoreFrame;

        public void ReadFromStream(MStreamReader sr)
        {
            Extra = sr.ReadInt32();

            int count = sr.ReadInt16();
            ReplayFrames = new List<ReplayFrame>(count);
            for (int i = 0; i < count; i++)
            {
                ReplayFrame rframes = sr.ReadData<ReplayFrame>();
                ReplayFrames.Add(rframes);
            }

            Action = sr.ReadByte();

            ScoreFrame sframe = new ScoreFrame();
            sframe.ReadFromStream(sr);
            ScoreFrame = sframe;
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Extra);
            sw.Write((short) ReplayFrames.Count);
            foreach (ReplayFrame rframe in ReplayFrames)
                sw.Write(rframe);
            sw.Write(Action);
            sw.Write(ScoreFrame);
        }

        public override string ToString()
        {
            StringBuilder repframes = new StringBuilder();
            foreach (ReplayFrame f in ReplayFrames)
                repframes.Append($"\t{f}\n");
            return $"Extra: {Extra}\nReplayFrames: [\n{repframes}]\nAction: {Action}\n{ScoreFrame}";
        }
    }

    public class ReplayFrame : ISerializer
    {
        public byte Button;
        public byte ButtonState;
        public float MouseX;
        public float MouseY;
        public int Time;

        public void ReadFromStream(MStreamReader sr)
        {
            ButtonState = sr.ReadByte();
            Button      = sr.ReadByte();
            MouseX      = sr.ReadSingle();
            MouseY      = sr.ReadSingle();
            Time        = sr.ReadInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(ButtonState);
            sw.Write(Button);
            sw.Write(MouseX);
            sw.Write(MouseY);
            sw.Write(Time);
        }

        public override string ToString()
        {
            return $"ButtonState: {ButtonState} Button: {Button} MouseX: {MouseX} MouseY: {MouseY} Time: {Time}";
        }
    }
}