﻿#pragma warning disable 4014

using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XayahBot.Utility;
using XayahBot.Database.Service;

namespace XayahBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private IDependencyMap _dependencyMap = new DependencyMap();

        private FileReader _fileReader = new FileReader();

        //

        public static void Main(string[] args)
        {
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        //

        private Program()
        {
            this._commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });
        }

        private async Task StartAsync()
        {
            this._client.Log += Logger.Log;

            await InitializeAsync();

            string token = this._fileReader.ReadFirstLine(Property.FilePath.Value + Property.FileToken.Value);
            if (!string.IsNullOrWhiteSpace(token))
            {
                await this._client.LoginAsync(TokenType.Bot, token);
                await this._client.StartAsync();
                //
                bool exit = false;
                while (!exit)
                {
                    if (Console.ReadLine().ToLower().Equals("exit"))
                    {
                        exit = true;
                    }
                }
                //
                await this._client.SetGameAsync(Property.GameShutdown.Value);
                await this._client.StopAsync();
            }
            else
            {
                Logger.Error("No token supplied.");
            }
            await Task.Delay(2500); // Wait a bit
        }

        private async Task InitializeAsync()
        {
            this._client.Ready += this.HandleReady;
            this._client.ChannelUpdated += this.HandleChannelUpdated;
            this._client.ChannelDestroyed += this.HandleChannelDestroyed;
            this._client.LeftGuild += this.HandleLeftGuild;
            this._client.MessageReceived += this.HandleMessageReceived;

            this._dependencyMap.Add(this._client);

            await this._commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        //

        private Task HandleReady()
        {
            string game = Property.GameActive.Value;
            if (!string.IsNullOrWhiteSpace(game))
            {
                this._client.SetGameAsync(game);
            }
            else
            {
                this._client.SetGameAsync(null);
            }
            //RiotStatusService.StartAsync(this._client); WiP
            return Task.CompletedTask;
        }

        private Task HandleChannelUpdated(SocketChannel preUpdateChannel, SocketChannel postUpdateChannel)
        {
            IgnoreService.UpdateAsync(preUpdateChannel.Id, ((IChannel)postUpdateChannel).Name);
            return Task.CompletedTask;
        }

        private Task HandleChannelDestroyed(SocketChannel deletedChannel)
        {
            IgnoreService.RemoveBySubjectIdAsync(deletedChannel.Id);
            return Task.CompletedTask;
        }

        private Task HandleLeftGuild(SocketGuild leftGuild)
        {
            IgnoreService.RemoveByGuildAsync(leftGuild.Id);
            return Task.CompletedTask;
        }

        private async Task HandleMessageReceived(SocketMessage arg)
        {
            int pos = 0;
            if (arg is SocketUserMessage message && (message.HasCharPrefix(char.Parse(Property.CmdPrefix.Value), ref pos) || message.HasMentionPrefix(message.Discord.CurrentUser, ref pos)))
            {
                CommandContext context = new CommandContext(message.Discord, message);
                IResult result = await this._commandService.ExecuteAsync(context, pos, this._dependencyMap);
                if (!result.IsSuccess)
                {
                    if (result.ErrorReason.Contains("Invalid context for command") ||
                        result.ErrorReason.Contains("have the required permission") ||
                        result.ErrorReason.Contains("on the ignore list for this bot") ||
                        result.ErrorReason.Contains("can only be run by the owner"))
                    {
                        IMessageChannel dmChannel = await context.User.CreateDMChannelAsync();
                        dmChannel?.SendMessageAsync($"This did not work! Reason: `{result.ErrorReason}`");
                    }
                    else if(result.Error != CommandError.UnknownCommand)
                    {
                        Logger.Debug($"Command failed: {result.ErrorReason}");
                    }
                }
            }
        }
    }
}
