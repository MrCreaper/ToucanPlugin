using CommandSystem;
using Exiled.API.Features;
using GameCore;
using System;
using System.Collections.Generic;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class ReflectControl : ICommand
    {
        public static bool Reflect { get; set; } = !Server.FriendlyFire && ToucanPlugin.Singleton.Config.ReflectTeamDMG;
        public string Command { get; } = "reflectcontrol";

        public string[] Aliases { get; } = { "reflect", "rc" };

        public string Description { get; } = "Reflect Team damage or not";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender.CheckPermission(PlayerPermissions.PermissionsManagement))
            {
                if (Reflect)
                {
                    Reflect = false;
                    response = $"Feel Free to tk";
                    return true;
                }
                else
                {
                    Reflect = true;
                    response = $"Reflecting Team Damage.";
                    return true;
                }
            }
            else
            {
                response = $"Fuck off, need permission mangment.";
                return true;
            }
        }
    }
}
