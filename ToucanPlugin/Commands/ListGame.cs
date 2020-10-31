using CommandSystem;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListGame : ICommand
    {
        public string Command { get; } = "listgamemode";

        public string[] Aliases { get; } = { "listgame", "lg" };

        public string Description { get; } = "Get the list of avaiable gamemodes";
        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            string Gamemodes = "";
            Gamemodes.ToList().ForEach(g => Gamemodes += g);
            response = Gamemodes;
            return true;
        }
    }
}
