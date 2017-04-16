﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using XayahBot.Utility;
using System.Collections.Generic;
using System.Linq;
using XayahBot.Command.Attribute;
using XayahBot.Service;

namespace XayahBot.Command
{
    public class CHelp : ModuleBase
    {
        public CommandService CmdService { get; set; }

        //

        public CHelp(CommandService cmdService)
        {
            this.CmdService = cmdService;
        }

        //

        private readonly string _logNoReplyChannel = "Could not reply to \"{0}\" because no appropriate channel could be found!";

        private readonly string _finishHelp = "You can also trigger commands with mentioning me instead of the prefix. " +
            "Also always remember using a space between each argument!" + Environment.NewLine +
            "If you have problems, questions and/or suggestions do not hesitate to message {0}.";

        //

#pragma warning disable 4014 // Intentional
        [Command("help"), Alias("h")]
        [Summary("Displays the list of commands.")]
        public async Task Help()
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
                Logger.Log(LogSeverity.Error, nameof(CProperty), string.Format(this._logNoReplyChannel, this.Context.User));
                return;
            }
            string message = string.Empty;

            string prefix = Property.CmdPrefix.Value;
            List<HelpLine> normalCmdList = new List<HelpLine>();
            List<HelpLine> modCmdList = new List<HelpLine>();
            List<HelpLine> adminCmdList = new List<HelpLine>();
            foreach (CommandInfo cmd in this.CmdService.Commands)
            {
                if (cmd.Preconditions.Contains(new RequireAdminAttribute()))
                {
                    adminCmdList.Add(this.GetCommandStringSimple(cmd));
                }
                else if (cmd.Preconditions.Contains(new RequireModAttribute()))
                {
                    modCmdList.Add(this.GetCommandStringSimple(cmd));
                }
                else
                {
                    normalCmdList.Add(this.GetCommandStringSimple(cmd));
                }
            }
            message = "__**Command List**__```";
            message += this.GetCommandBlockString(normalCmdList);
            if (PermissionService.IsAdminOrMod(this.Context) && modCmdList.Count > 0)
            {
                message += $"```{Environment.NewLine}__Mod-Commands__```";
                message += this.GetCommandBlockString(modCmdList);
            }
            if (PermissionService.IsAdmin(this.Context) && adminCmdList.Count > 0)
            {
                message += $"```{Environment.NewLine}__Admin-Commands__```";
                message += this.GetCommandBlockString(adminCmdList);
            }
            message += $"```{Environment.NewLine}{string.Format(_finishHelp, Property.Author)}";

            channel.SendMessageAsync(message);
        }
#pragma warning restore 4014

        //

        private HelpLine GetCommandStringSimple(CommandInfo cmd)
        {
            string command = $".{string.Join("|.", cmd.Aliases)}";
            foreach (ParameterInfo param in cmd.Parameters)
            {
                if (param.IsOptional)
                {
                    command += $" [<{param.Name}>]";
                }
                else
                {
                    command += $" <{param.Name}>";
                }
            }
            return new HelpLine()
            {
                Command = command,
                Summary = cmd.Summary
            };
        }

        private string GetCommandBlockString(List<HelpLine> cmdList)
        {
            string text = string.Empty;
            for (int i = 0; i < cmdList.Count; i++)
            {
                if (i > 0)
                {
                    text += Environment.NewLine;
                }
                HelpLine line = cmdList.ElementAt(i);
                text += $"{line.Command.PadRight(cmdList.OrderByDescending(x => x.Command.Length).First().Command.Length)} - {line.Summary}";
            }
            return text;
        }
    }
}
