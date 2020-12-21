using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Scp079 : ICommand
    {
        public string Command { get; } = "079";

        public string[] Aliases { get; } = { "79" };

        public string Description { get; } = "Use 079 abilities";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            List<string> args = new List<string>(arguments.Array);
            Player p = Player.List.ToList().Find(x => x.Sender == Sender);
            if (p.Role == RoleType.Scp079)
            {
                switch (args[1]) {
                    default:
                        response = "Invalid subcommand";
                        return true;
                    case "blackout":
                        Map.TurnOffAllLights(10);
                        response = "Blackout...";
                        return true;
                }
            }
            else
            {
                response = "Bruh";
                return true;
            }
        }
    }
}
