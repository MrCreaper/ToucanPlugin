using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Report : ICommand
    {
        public string Command { get; } = "report";

        public string[] Aliases { get; } = { "r" };

        public string Description { get; } = "Report a body in the among us gamemode!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is PlayerCommandSender PCplayer)
            {
                if (PCplayer.CCM.IsAlive)
                {
                    response = $"Nah, ur ded fam";
                    return false;
                }
                else
                {
                    response = $"Fuck off";
                    return false;
                }
            }
            else
            {
                response = "Fuck off";
                return false;
            }
        }
    }
}
