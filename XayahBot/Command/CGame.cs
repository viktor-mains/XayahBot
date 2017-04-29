﻿#pragma warning disable 4014

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using XayahBot.Utility;

namespace XayahBot.Command
{
    public class CGame : ModuleBase
    {
        private readonly string _logRequest = "\"{0}\" requested \"status\".";
        private readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";
        private readonly string _logChanged = "Status changed to \"{0}\".";

        //

        private readonly DiscordSocketClient _client;

        public CGame(DiscordSocketClient client)
        {
            this._client = client;
        }

        //

        [Command("game")]
        [RequireOwner]
        [RequireContext(ContextType.DM)]
        [Summary("Updates the current game.")]
        public async Task Game([Remainder] string text = "")
        {
            Logger.Debug(string.Format(this._logRequest, this.Context.User));
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
                Logger.Error(string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string game = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
            Property.GameActive.Value = game;
            this._client.SetGameAsync(game);
            Logger.Debug(string.Format(this._logChanged, game));
        }
    }
}
