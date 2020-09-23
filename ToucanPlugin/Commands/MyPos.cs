using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class MyPos : ICommand
    {
        public string Command { get; } = "mypos";

        public string[] Aliases { get; } = { "ms" };

        public string Description { get; } = "A command to get your posistion";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is PlayerCommandSender PCplayer)
            {
                response = $"Hello {PCplayer.Nickname}";
                return false;
            }
            else
            {
                if (Sender is ConsoleCommandSender CCplayer)
                {
                    response = $"hello {CCplayer.Nickname}";
                    return false;
                }
                else
                {
                    response = $"World";
                    return true;
                }
            }
        }
    }
}
