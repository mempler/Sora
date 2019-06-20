using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Beatmaps.Legacy;
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
using Sora.Enums;

namespace Sora
{
    // a hardly modified version of https://github.com/ppy/osu-tools in literally one file. Licensed under MIT license!
    public static class LegacyHelper
    {
        public static Ruleset Convert(PlayMode mode)
        {
            switch (mode)
            {
                case PlayMode.Osu:
                    return new OsuRuleset();
                case PlayMode.Taiko:
                    return new CatchRuleset();
                case PlayMode.Ctb:
                    return new TaikoRuleset();
                case PlayMode.Mania:
                    return new ManiaRuleset();
                default:
                    return new OsuRuleset();
            }
        }
    }

    public class ProcessorWorkingBeatmap : WorkingBeatmap
    {
        private readonly Beatmap beatmap;

        public ProcessorWorkingBeatmap(string file, int? beatmapId = null)
            : this(readFromFile(file), beatmapId)
        {
        }

        private ProcessorWorkingBeatmap(Beatmap beatmap, int? beatmapId = null)
            : base(beatmap.BeatmapInfo, null)
        {
            this.beatmap = beatmap;

            switch ((PlayMode) beatmap.BeatmapInfo.RulesetID)
            {
                case PlayMode.Osu:
                    beatmap.BeatmapInfo.Ruleset = new OsuRuleset().RulesetInfo;
                    break;
                case PlayMode.Taiko:
                    beatmap.BeatmapInfo.Ruleset = new CatchRuleset().RulesetInfo;
                    break;
                case PlayMode.Ctb:
                    beatmap.BeatmapInfo.Ruleset = new TaikoRuleset().RulesetInfo;
                    break;
                case PlayMode.Mania:
                    beatmap.BeatmapInfo.Ruleset = new ManiaRuleset().RulesetInfo;
                    break;
                default:
                    beatmap.BeatmapInfo.Ruleset = new OsuRuleset().RulesetInfo;
                    break;
            }

            if (beatmapId.HasValue)
                beatmap.BeatmapInfo.OnlineBeatmapID = beatmapId;
        }

        private static Beatmap readFromFile(string filename)
        {
            using var stream = File.OpenRead(filename);
            using var streamReader = new StreamReader(stream);

            return Decoder.GetDecoder<Beatmap>(streamReader).Decode(streamReader);
        }

        protected override IBeatmap GetBeatmap() => beatmap;

        protected override Texture GetBackground() => null;

        protected override Track GetTrack() => null;
    }

    public class ProcessorScoreParser : LegacyScoreParser
    {
        private readonly WorkingBeatmap beatmap;
        private Ruleset ruleset;

        public ProcessorScoreParser(WorkingBeatmap beatmap) => this.beatmap = beatmap;

        private ReplayFrame convertFrame(LegacyReplayFrame legacyFrame)
        {
            var convertible = ruleset.CreateConvertibleReplayFrame();
            if (convertible == null)
                throw new InvalidOperationException(
                    $"Legacy replay cannot be converted for the ruleset: {ruleset.Description}"
                );

            convertible.ConvertFrom(legacyFrame, beatmap.Beatmap);

            var frame = (ReplayFrame) convertible;
            frame.Time = legacyFrame.Time;

            return frame;
        }

        private void readLegacyReplay(Replay replay, TextReader reader)
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
                    convertFrame(
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

        public Score Parse(Scores dbScore)
        {
            using var rawReplay = File.OpenRead("data/replays/" + dbScore.ReplayMd5);

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

            ruleset = LegacyHelper.Convert(dbScore.PlayMode);

            var score = new Score
            {
                ScoreInfo = new ScoreInfo
                {
                    Accuracy = dbScore.Accuracy,
                    Beatmap = beatmap.BeatmapInfo,
                    Combo = dbScore.MaxCombo,
                    MaxCombo = dbScore.MaxCombo,
                    User = new User {Username = dbScore.ScoreOwner.Username},
                    RulesetID = (int) dbScore.PlayMode,
                    Date = dbScore.Date,
                    Files = null,
                    Hash = null,
                    Mods = LegacyHelper.Convert(dbScore.PlayMode).ConvertLegacyMods((LegacyMods) dbScore.Mods)
                                       .ToArray(),
                    Ruleset = LegacyHelper.Convert(dbScore.PlayMode).RulesetInfo,
                    Passed = true,
                    TotalScore = (long) dbScore.TotalScore,
                    Statistics = new Dictionary<HitResult, int>
                    {
                        [HitResult.Perfect] = (int) dbScore.Count300,
                        [HitResult.Good] = (int) dbScore.Count100,
                        [HitResult.Great] = (int) dbScore.CountGeki,
                        [HitResult.Meh] = (int) dbScore.Count50,
                        [HitResult.Miss] = (int) dbScore.CountMiss,
                        [HitResult.Ok] = (int) dbScore.CountKatu,
                        [HitResult.None] = 0
                    }
                },
                Replay = new Replay()
            };

            using (var lzma = new LzmaStream(properties, rawReplay, compressedSize, outSize))
            using (var reader = new StreamReader(lzma))
            {
                readLegacyReplay(score.Replay, reader);
            }

            CalculateAccuracy(score.ScoreInfo);

            return score;
        }

        protected override Ruleset GetRuleset(int rulesetId) => LegacyHelper.Convert((PlayMode) rulesetId);

        protected override WorkingBeatmap GetBeatmap(string md5Hash) => beatmap;
    }

    public class PerformancePointsProcessor
    {
        public double Compute(Scores dbscore)
        {
            var workingBeatmap = new ProcessorWorkingBeatmap("data/beatmaps/" + dbscore.FileMd5);
            var psp = new ProcessorScoreParser(workingBeatmap);
            var score = psp.Parse(dbscore);

            var categoryAttribs = new Dictionary<string, double>();
            var pp = score.ScoreInfo.Ruleset
                          .CreateInstance()
                          .CreatePerformanceCalculator(workingBeatmap, score.ScoreInfo)
                          .Calculate(categoryAttribs);

            return pp;
        }
    }
}
