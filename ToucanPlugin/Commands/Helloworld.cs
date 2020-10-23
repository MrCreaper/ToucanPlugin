using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;
using Exiled.API.Features;
using System.Linq;

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
            if (Sender is CommandSender PCplayer)
            {
                Player p = Player.List.ToList().Find(x => x.Sender == PCplayer);
                response = $"Hello {PCplayer.Nickname}";
                return false;
            }
            else
            {
                if (Sender is CommandSender CCplayer)
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
