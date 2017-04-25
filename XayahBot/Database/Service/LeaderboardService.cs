﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Utility;

namespace XayahBot.Database.Service
{
    public static class LeaderboardService
    {
        public static List<TLeaderboardEntry> GetLeaderboard(ulong guild)
        {
            CheckForReset();
            using (GeneralContext db = new GeneralContext())
            {
                return db.Leaderboard.Where(x => x.Guild.Equals(guild)).ToList();
            }
        }

        public static Task IncrementAnswerAsync(ulong guild, ulong userId, string userName)
        {
            CheckForReset();
            using (GeneralContext db = new GeneralContext())
            {
                TLeaderboardEntry quizStat = db.Leaderboard.FirstOrDefault(x => x.Guild.Equals(guild) && x.UserName.Equals(userName));
                if (quizStat == null)
                {
                    db.Leaderboard.Add(new TLeaderboardEntry { Guild = guild, UserId = userId, UserName = userName, Answers = 1 });
                }
                else
                {
                    quizStat.Answers++;
                }
                db.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

        //

        private static void CheckForReset()
        {
            if (!string.IsNullOrWhiteSpace(Property.QuizLastReset.Value))
            {
                DateTime now = DateTime.UtcNow;
                string[] lastResetValue = Property.QuizLastReset.Value.Split('/');
                if (new DateTime(int.Parse(lastResetValue.ElementAt(1)), int.Parse(lastResetValue.ElementAt(0)), 1, 0, 0, 0).AddMonths(1) < now)
                {
                    using (GeneralContext db = new GeneralContext())
                    {
                        foreach (TLeaderboardEntry entry in db.Leaderboard)
                        {
                            db.Remove(entry);
                        }
                        db.SaveChangesAsync();
                    }
                    Property.QuizLastReset.Value = $"{now.Month}/{now.Year}";
                }
            }
        }
    }
}