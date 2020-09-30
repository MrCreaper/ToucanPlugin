using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class AcGame : ICommand
    {
        public static GamemodeType NextGamemode { get; set; } = 0;
        public static GamemodeType RoundGamemode { get; set; } = 0;
        public string Command { get; } = "activateGamemode";

        public string[] Aliases { get; } = { "acGame", "acg" };

        public string Description { get; } = "Select a gamemode and next round we will play it";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender.CheckPermission(PlayerPermissions.Announcer))
            {
                string[] args = arguments.Array;
                    var isNumeric = int.TryParse(args[1], out int gameNum);
                    if (isNumeric == true)
                    {
                        NextGamemode = (GamemodeType)gameNum;
                        response = "Gamemode set for next round.";
                        return true;
                    }
                    else
                    {
                        response = "Please get the number from gamemodeList";
                        return false;
                    }
            }
            else
            {
                response = "You need Announcer permission to activate a gamemode.";
                return false;
            }
        }
    }
}