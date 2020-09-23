using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Helloworld : ICommand
    {
        public string Command { get; } = "hello";

        public string[] Aliases { get; } = { "helloworld" };

        public string Description { get; } = "A command that says hello to the world.";

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
