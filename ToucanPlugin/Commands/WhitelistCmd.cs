using CommandSystem;
using Exiled.API.Features;
using GameCore;
using System;
using System.Collections.Generic;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class WhitelistCmd : ICommand
    {
        readonly Whitelist wl = new Whitelist();

        public string Command { get; } = "whitelist";

        public string[] Aliases { get; } = { "white", "wl" };

        public string Description { get; } = "Make the server whitelisted or not...";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender.CheckPermission(PlayerPermissions.PermissionsManagement))
            {
                if (Whitelist.Whitelisted)
                {
                    Whitelist.Whitelisted = false;
                    response = $"Server is now open!";
                    return true;
                }
                else
                {
                    Whitelist.Whitelisted = true;
                    wl.Read();
                    wl.KickAllNoneWhite();
                    response = $"Server is now closed!";
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
