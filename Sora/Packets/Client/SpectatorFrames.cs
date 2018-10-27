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
using System.Text;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Client
{
    public class SpectatorFrames : IPacket
    {
        public PacketId Id => PacketId.ClientSpectateFrames;

        public SpectatorFrame Frames;
        
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
        public int Time;
        public byte Id;

        public ushort Count300;
        public ushort Count100;
        public ushort Count50;
        public ushort CountGeki;
        public ushort CountKatu;
        public ushort CountMiss;

        public int TotalScore;
        public ushort MaxCombo;
        public ushort CurrentCombo;
        public bool Perfect;

        public byte CurrentHp;
        public byte TagByte;

        public bool ScoreV2;
        public double ComboPortion;
        public double BonusPortion;

        public override string ToString()
        {
            return $"Time: {Time} Id: {Id}\n" +
                   $"Count300: {Count300} Count100: {Count100} Count50: {Count50} " +
                   $"CountGeki: {CountGeki} CountKatu: {CountKatu} CountMiss: {CountMiss}\n" +
                   $"TotalScore: {TotalScore} MaxCombo: {MaxCombo} CurrentCombo: {CurrentCombo} Perfect: {Perfect}\n" +
                   $"CurrentHp: {CurrentHp} TagByte: {TagByte}\n" +
                   $"ScoreV2: {ScoreV2} ComboPortion: {ComboPortion} BonusPortion: {BonusPortion}";
        }
        
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
    }

    public class SpectatorFrame : ISerializer
    {
        public int Extra;
        public List<ReplayFrame> ReplayFrames;
        public byte Action;
        public ScoreFrame ScoreFrame;

        public override string ToString()
        {
            StringBuilder repframes = new StringBuilder();
            foreach (ReplayFrame f in ReplayFrames)
                repframes.Append($"\t{f}\n");
            return $"Extra: {Extra}\nReplayFrames: [\n{repframes}]\nAction: {Action}\n{ScoreFrame}";
        }

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
            sw.Write((short)ReplayFrames.Count);
            foreach (ReplayFrame rframe in ReplayFrames)
                sw.Write(rframe);
            sw.Write(Action);
            sw.Write(ScoreFrame);
        }
    }
    
    public class ReplayFrame : ISerializer
    {
        public byte ButtonState;
        public byte Button;
        public float MouseX;
        public float MouseY;
        public int Time;
        
        public override string ToString()
        {
            return $"ButtonState: {ButtonState} Button: {Button} MouseX: {MouseX} MouseY: {MouseY} Time: {Time}";
        }

        public void ReadFromStream(MStreamReader sr)
        {
            ButtonState = sr.ReadByte();
            Button = sr.ReadByte();
            MouseX = sr.ReadSingle();
            MouseY = sr.ReadSingle();
            Time = sr.ReadInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(ButtonState);
            sw.Write(Button);
            sw.Write(MouseX);
            sw.Write(MouseY);
            sw.Write(Time);
        }
    }
}