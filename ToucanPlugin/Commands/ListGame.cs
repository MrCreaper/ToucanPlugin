using CommandSystem;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ListGame : ICommand
    {
        public string Command { get; } = "listGamemode";

        public string[] Aliases { get; } = { "listGame" };

        public string Description { get; } = "Get the list of avaiable gamemodes";
        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
                response = "0 - No gamemode\n1 - Quiet Place";
                return true;
        }
    }
}
