﻿namespace XayahBot.Database.Model
{
    public class TIgnoreEntry
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong SubjectId { get; set; }
        public bool IsChannel { get; set; }
    }
}
