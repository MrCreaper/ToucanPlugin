﻿using CommandSystem;
using RemoteAdmin;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Pets : ICommand
    {
        private readonly Tcp Tcp = new Tcp();
        public string Command { get; } = "pets";

        public string[] Aliases { get; } = { "petslist" };

        public string Description { get; } = "Know what pets you have unlocked.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is PlayerCommandSender PCplayer)
            {
                Tcp.Send($"petsList {PCplayer.CCM.UserId}");
                response = $"Getting Pets list...";
                return true;
            }
            else
            {
                response = $"Fuck off";
                return false;
            }
        }
    }
}
