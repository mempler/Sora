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
            Frames = new SpectatorFrame
            {
                Extra = sr.ReadInt32()
            };

            int count = sr.ReadInt16();
            Frames.ReplayFrames = new List<ReplayFrame>(count);
            for (int i = 0; i < count; i++)
            {
                ReplayFrames rframes = new ReplayFrames();
                rframes.ReadFromStream(sr);
                Frames.ReplayFrames.Add(rframes.Frame);
            }
            Frames.Action = sr.ReadByte();
            
            ScoreFrames sframe = new ScoreFrames();
            sframe.ReadFromStream(sr);
            Frames.ScoreFrame = sframe.Frame;
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new NotImplementedException();
        }
    }

    public class ReplayFrames : ISerializer
    {
        public ReplayFrame Frame;

        public void ReadFromStream(MStreamReader sr)
        {
            Frame = new ReplayFrame
            {
                ButtonState   = sr.ReadByte(),
                Button        = sr.ReadByte(),
                MouseX        = sr.ReadSingle(),
                MouseY        = sr.ReadSingle(),
                Time          = sr.ReadInt32()
            };
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Frame.ButtonState);
            sw.Write(Frame.Button);
            sw.Write(Frame.MouseX);
            sw.Write(Frame.MouseY);
            sw.Write(Frame.Time);
        }
    }
    public class ScoreFrames : ISerializer
    {
        public ScoreFrame Frame;
        
        public void ReadFromStream(MStreamReader sr)
        {
            Frame = new ScoreFrame
            {
                Time         = sr.ReadInt32(),
                Id           = sr.ReadByte(),
                Count300     = sr.ReadUInt16(),
                Count100     = sr.ReadUInt16(),
                Count50      = sr.ReadUInt16(),
                CountGeki    = sr.ReadUInt16(),
                CountKatu    = sr.ReadUInt16(),
                CountMiss    = sr.ReadUInt16(),
                TotalScore   = sr.ReadInt32(),
                MaxCombo     = sr.ReadUInt16(),
                CurrentCombo = sr.ReadUInt16(),
                Perfect      = sr.ReadBoolean(),
                CurrentHp    = sr.ReadByte(),
                TagByte      = sr.ReadByte(),
                ScoreV2      = sr.ReadBoolean(),
                ComboPortion = Frame.ScoreV2 ? sr.ReadDouble() : 0,
                BonusPortion = Frame.ScoreV2 ? sr.ReadDouble() : 0
            };
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Frame.Time);
            sw.Write(Frame.Id);
            sw.Write(Frame.Count300);
            sw.Write(Frame.Count100);
            sw.Write(Frame.Count50);
            sw.Write(Frame.CountGeki);
            sw.Write(Frame.CountKatu);
            sw.Write(Frame.CountMiss);
            sw.Write(Frame.TotalScore);
            sw.Write(Frame.MaxCombo);
            sw.Write(Frame.CurrentCombo);
            sw.Write(Frame.Perfect);
            sw.Write(Frame.CurrentHp);
            sw.Write(Frame.TagByte);
            sw.Write(Frame.ScoreV2);
            if (!Frame.ScoreV2) return;
            sw.Write(Frame.ComboPortion);
            sw.Write(Frame.BonusPortion);
        }
    }

    public struct SpectatorFrame
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
    }
    
    public struct ScoreFrame
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
    }
    public struct ReplayFrame
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
    }
}