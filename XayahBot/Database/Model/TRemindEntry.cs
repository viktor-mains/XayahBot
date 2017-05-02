﻿using System;

namespace XayahBot.Database.Model
{
    public class TRemindEntry
    {
        public int Id { get; set; }
        public ulong UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Message { get; set; }
    }
}