using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Plugins : ICommand
    {
        public string Command { get; } = "plugins";

        public string[] Aliases { get; } = {  };

        public string Description { get; } = "Displaies all the plugins";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender.CheckPermission(PlayerPermissions.PlayerSensitiveDataAccess))
            {
                response = Exiled.API.Features.Paths.Plugins;
                return true;
            }
            else
            {
                response = "Requires permission for player sensitive data.";
                return true;
            }
        }
    }
}
