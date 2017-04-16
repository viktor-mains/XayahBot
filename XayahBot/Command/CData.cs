﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Service;
using XayahBot.Utility;
using System;

namespace XayahBot.Command
{
    [Group("data")]
    public class CData : ModuleBase
    {
        private static readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        //

#pragma warning disable 4014 // Intentional
        [Command("champ"), Alias("c")]
        [Summary("Displays data of the specified champion (sponsored by Riot API).")]
        public async Task Champ([Remainder] string name)
        {
            IMessageChannel channel = null;
            if (this.Context.IsPrivate)
            {
                channel = this.Context.Channel;
            }
            else
            {
                channel = await this.Context.Message.Author.CreateDMChannelAsync();
            }
            if (channel == null)
            {
                Logger.Log(LogSeverity.Error, nameof(CData), string.Format(_logNoReplyChannel, this.Context.User));
                return;
            }
            InfoService.GetChampionData(channel, name);
        }
#pragma warning restore 4014
    }
}