﻿#pragma warning disable 4014

using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Command.Attribute;
using XayahBot.Database.Service;
using XayahBot.Utility;
using XayahBot.Service;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Command.Ignore
{
    public class CIgnore : ModuleBase
    {
        private readonly string _logRequest = "\"{0}\" requested \"ignore\" command.";
        private readonly string _logIgnoreSuccess = "Added \"{0}\" to the ignore list.";

        private readonly string _ignoreSuccess = "Added `{0}` to the ignore list.";
        private readonly string _ignoreFailed = "Failed to add `{0}` to the ignore list.";
        private readonly string _ignoreExisting = "`{0}` is already on the ignore list.";

        private readonly List<string> _ignoredReactionList = new List<string>
        {
            "I warned you. Oh wait... did I? Crap. My fault.",
            "All out of second chances, sorry!",
            "And they won't be needing that anymore.",
            "My last nerve is long gone."
        };

        //

        private readonly IgnoreService _ignoreService;

        public CIgnore(IgnoreService ignoreService)
        {
            this._ignoreService = ignoreService;
        }

        //

        [Command("ignore")]
        [RequireMod]
        [RequireContext(ContextType.Guild)]
        [Summary("Adds all mentioned user and channel to the ignore list.")]
        public async Task Channel([Remainder] string text)
        {
            string message = string.Empty;
            Logger.Info(string.Format(this._logRequest, this.Context.User));
            foreach (ulong userId in this.Context.Message.MentionedUserIds.Distinct())
            {
                if (!userId.Equals(this.Context.Client.CurrentUser.Id) && !Permission.IsAdmin(this.Context))
                {
                    IUser user = await this.Context.Guild.GetUserAsync(userId);
                    message += await AddIgnore(user.Id, user.ToString()) + Environment.NewLine;
                }
            }
            foreach (ulong channelId in this.Context.Message.MentionedChannelIds.Distinct())
            {
                IChannel channel = await this.Context.Guild.GetChannelAsync(channelId);
                message += await AddIgnore(channel.Id, channel.Name, true) + Environment.NewLine;
            }
            if (!string.IsNullOrWhiteSpace(message))
            {
                await ReplyAsync(message);
                ReplyAsync(RNG.FromList(this._ignoredReactionList));
            }
        }

        private async Task<string> AddIgnore(ulong subjectId, string subjectName)
        {
            return await AddIgnore(subjectId, subjectName, false);
        }

        private async Task<string> AddIgnore(ulong subjectId, string subjectName, bool isChannel)
        {
            string message = string.Empty;
            try
            {
                await this._ignoreService.AddAsync(new TIgnoreEntry
                {
                    Guild = this.Context.Guild.Id,
                    IsChannel = isChannel,
                    SubjectId = subjectId,
                    SubjectName = subjectName
                });
                message = string.Format(this._ignoreSuccess, subjectName);
                Logger.Warning(string.Format(this._logIgnoreSuccess, subjectName));
            }
            catch (AlreadyExistingException)
            {
                message = string.Format(this._ignoreExisting, subjectName);
            }
            catch (NotSavedException)
            {
                message = string.Format(this._ignoreFailed, subjectName);
            }
            return message;
        }
    }
}