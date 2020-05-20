using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Video;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Beatmaps.Legacy;
using osu.Game.IO;
using osu.Game.Replays;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Taiko;
using osu.Game.Scoring;
using osu.Game.Scoring.Legacy;
using osu.Game.Users;
using SharpCompress.Compressors.LZMA;
using Sora.Database.Models;
using Sora.Framework.Enums;

namespace Sora
{
    // a hardly modified version of https://github.com/ppy/osu-tools in literally one file. Licensed under MIT license!
    public static class LegacyHelper
    {
        public static Ruleset Convert(PlayMode mode)
        {
            return mode switch
            {
                PlayMode.Osu => (Ruleset) new OsuRuleset(),
                PlayMode.Taiko => new CatchRuleset(),
                PlayMode.Ctb => new TaikoRuleset(),
                PlayMode.Mania => new ManiaRuleset(),
                _ => new OsuRuleset()
            };
        }
    }

    public class ProcessorWorkingBeatmap : WorkingBeatmap
    {
        private readonly Beatmap _beatmap;

        public ProcessorWorkingBeatmap(string file, int? beatmapId = null)
            : this(ReadFromFile(file), beatmapId)
        {
        }

        private ProcessorWorkingBeatmap(Beatmap beatmap, int? beatmapId = null)
            : base(beatmap.BeatmapInfo, null)
        {
            _beatmap = beatmap;

            beatmap.BeatmapInfo.Ruleset = (PlayMode) beatmap.BeatmapInfo.RulesetID switch
            {
                PlayMode.Osu => new OsuRuleset().RulesetInfo,
                PlayMode.Taiko => new CatchRuleset().RulesetInfo,
                PlayMode.Ctb => new TaikoRuleset().RulesetInfo,
                PlayMode.Mania => new ManiaRuleset().RulesetInfo,
                _ => new OsuRuleset().RulesetInfo
            };

            if (beatmapId.HasValue)
                beatmap.BeatmapInfo.OnlineBeatmapID = beatmapId;
        }

        private static Beatmap ReadFromFile(string filename)
        {
            using var stream = File.OpenRead(filename);
            using var streamReader = new LineBufferedReader(stream);

            return Decoder.GetDecoder<Beatmap>(streamReader).Decode(streamReader);
        }

        protected override IBeatmap GetBeatmap() => _beatmap;

        protected override Texture GetBackground() => null;
        protected override VideoSprite GetVideo()
        {
            throw new NotImplementedException();
        }

        protected override Track GetTrack() => null;
    }

    public class ProcessorScoreParser : LegacyScoreParser
    {
        private readonly WorkingBeatmap _beatmap;
        private Ruleset _ruleset;

        public ProcessorScoreParser(WorkingBeatmap beatmap) => _beatmap = beatmap;

        private ReplayFrame ConvertFrame(LegacyReplayFrame legacyFrame)
        {
            var convertible = _ruleset.CreateConvertibleReplayFrame();
            if (convertible == null)
                throw new InvalidOperationException(
                    $"Legacy replay cannot be converted for the ruleset: {_ruleset.Description}"
                );

            convertible.ConvertFrom(legacyFrame, _beatmap.Beatmap);

            var frame = (ReplayFrame) convertible;
            frame.Time = legacyFrame.Time;

            return frame;
        }

        private void ReadLegacyReplay(Replay replay, TextReader reader)
        {
            float lastTime = 0;

            foreach (var l in reader.ReadToEnd().Split(','))
            {
                var split = l.Split('|');

                if (split.Length < 4)
                    continue;

                if (split[0] == "-12345")
                    continue;

                var diff = Parsing.ParseFloat(split[0]);
                lastTime += diff;

                if (diff < 0)
                    continue;

                replay.Frames.Add(
                    ConvertFrame(
                        new LegacyReplayFrame(
                            lastTime,
                            Parsing.ParseFloat(
                                split[1], Parsing.MAX_COORDINATE_VALUE
                            ),
                            Parsing.ParseFloat(
                                split[2], Parsing.MAX_COORDINATE_VALUE
                            ),
                            (ReplayButtonState) Parsing.ParseInt(split[3])
                        )
                    )
                );
            }
        }

        public Score Parse(DbScore dbScore, string replayPath = null)
        {
            using var rawReplay = replayPath == null ? File.OpenRead("data/replays/" + dbScore.ReplayMd5) : File.OpenRead(replayPath);

            var properties = new byte[5];
            if (rawReplay.Read(properties, 0, 5) != 5)
                throw new IOException("input .lzma is too short");

            long outSize = 0;

            for (var i = 0; i < 8; i++)
            {
                var v = rawReplay.ReadByte();
                if (v < 0)
                    throw new IOException("Can't Read 1");

                outSize |= (long) (byte) v << (8 * i);
            }

            var compressedSize = rawReplay.Length - rawReplay.Position;

            _ruleset = LegacyHelper.Convert(dbScore.PlayMode);

            var score = new Score
            {
                ScoreInfo = new ScoreInfo
                {
                    Accuracy = dbScore.Accuracy,
                    Beatmap = _beatmap.BeatmapInfo,
                    Combo = dbScore.MaxCombo,
                    MaxCombo = dbScore.MaxCombo,
                    User = new User {Username = dbScore.ScoreOwner.UserName},
                    RulesetID = (int) dbScore.PlayMode,
                    Date = dbScore.Date,
                    Files = null,
                    Hash = null,
                    Mods = LegacyHelper.Convert(dbScore.PlayMode).ConvertLegacyMods((LegacyMods) dbScore.Mods)
                                       .ToArray(),
                    Ruleset = LegacyHelper.Convert(dbScore.PlayMode).RulesetInfo,
                    Passed = true,
                    TotalScore = dbScore.TotalScore,
                    Statistics = new Dictionary<HitResult, int>
                    {
                        [HitResult.Perfect] = dbScore.Count300,
                        [HitResult.Good] = dbScore.Count100,
                        [HitResult.Great] = dbScore.CountGeki,
                        [HitResult.Meh] = dbScore.Count50,
                        [HitResult.Miss] = dbScore.CountMiss,
                        [HitResult.Ok] = dbScore.CountKatu,
                        [HitResult.None] = 0
                    }
                },
                Replay = new Replay()
            };

            using (var lzma = new LzmaStream(properties, rawReplay, compressedSize, outSize))
            using (var reader = new StreamReader(lzma))
            {
                ReadLegacyReplay(score.Replay, reader);
            }

            CalculateAccuracy(score.ScoreInfo);

            return score;
        }

        protected override Ruleset GetRuleset(int rulesetId) => LegacyHelper.Convert((PlayMode) rulesetId);

        protected override WorkingBeatmap GetBeatmap(string md5Hash) => _beatmap;
    }

    public class PerformancePointsProcessor
    {
        /// <summary>
        /// Compute Performance Points from given Score and Replay!
        /// </summary>
        /// <param name="dbScore"></param>
        /// <param name="replayPath"></param>
        /// <param name="beatmapPath"></param>
        /// <returns>Performance Points</returns>
        public static double Compute(DbScore dbScore, string replayPath = null, string beatmapPath = null)
        {
            var workingBeatmap = beatmapPath == null ?
                new ProcessorWorkingBeatmap("data/beatmaps/" + dbScore.FileMd5) :
                new ProcessorWorkingBeatmap(beatmapPath);
            
            var psp = new ProcessorScoreParser(workingBeatmap);
            var score = psp.Parse(dbScore, replayPath);

            var categoryAttribs = new Dictionary<string, double>();
            var pp = score.ScoreInfo.Ruleset
                          .CreateInstance()
                          .CreatePerformanceCalculator(workingBeatmap, score.ScoreInfo)
                          .Calculate(categoryAttribs);

            return pp;
        }
    }
}
