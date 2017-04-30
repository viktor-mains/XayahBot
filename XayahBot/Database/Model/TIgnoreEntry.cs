﻿namespace XayahBot.Database.Model
{
    public class TIgnoreEntry
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong SubjectId { get; set; }
        public string SubjectName { get; set; }
        public bool IsChannel { get; set; }
    }
}
