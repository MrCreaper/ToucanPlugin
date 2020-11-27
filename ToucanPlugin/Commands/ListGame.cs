using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListGame : ICommand
    {
        readonly GamemodeLogic gl = new GamemodeLogic();
        public string Command { get; } = "listgamemodes";

        public string[] Aliases { get; } = { "listgames", "lg" };

        public string Description { get; } = "Get the list of avaiable gamemodes";
        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            string Gamemodes = "\nGamemodes:";
            List<GamemodeType> GamemodeList = new List<GamemodeType>(Enum.GetValues(typeof(GamemodeType)).Cast<GamemodeType>().ToList());
            for (int i = 0; i < GamemodeList.Count; i++)
                Gamemodes += $"\n{gl.ConvertToNice(GamemodeList[i]),-20} [{i,2} ]";
            response = Gamemodes;
            return true;
        }
    }
}
