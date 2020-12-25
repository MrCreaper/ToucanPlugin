using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;
using Exiled.API.Features;
using System.Linq;
using System.IO;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Play : ICommand
    {
        public string Command { get; } = "play";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "play sound via path";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender PCplayer)
            {
                new CommsHack.AudioAPI().PlayFile(arguments.Array[1], 0.5f);
                response = $"Playing?";
                return false;
            }
            else
            {
                response = $"Fuck off";
                return true;
            }
        }
    }
}
