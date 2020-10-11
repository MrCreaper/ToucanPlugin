using CommandSystem;
using Exiled.API.Features;
using GameCore;
using System;
using System.Collections.Generic;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class WhitelistUser : ICommand
    {
        readonly Whitelist wl = new Whitelist();

        public string Command { get; } = "whitelistUser";

        public string[] Aliases { get; } = { "whiteUser", "wlu" };

        public string Description { get; } = "Add someone to the whitlist";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender.CheckPermission(PlayerPermissions.PermissionsManagement))
            {
                List<string> args = new List<string>(arguments.Array);
                if (args[1] == null)
                {
                    response = $"Missing user id";
                    return false;
                }
                else
                {
                    if (!Whitelist.WhitelistUsers.Contains(args[1]))
                    {
                        wl.Add(args[1]);
                        response = $"User whit id of {args[1]} now whitelisted!";
                        return true;
                    }
                    else
                    {
                        wl.Remove(args[1]);
                        response = $"User whit id of {args[1]} is now OFF the whitelisted.";
                        return true;
                    }
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
