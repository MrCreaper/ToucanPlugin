using CommandSystem;
using Exiled.API.Features;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    //[CommandHandler(typeof(ClientCommandHandler))]
    class Kill : ICommand
    {
        public string Command { get; } = "kill";

        public string[] Aliases { get; } = { "terminate" };

        public string Description { get; } = "Kill someone";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender.CheckPermission(PlayerPermissions.PlayersManagement))
            {
                if (arguments.Array[1] != null)
                {
                    if (arguments.Array[1] == "all")
                    {
                        Player.List.ToList().ForEach(user => user.Kill());
                        response = "Killed everyone";
                        return true;
                    }
                    else
                    {
                        if (arguments.Array[1].Contains("."))
                        {
                            String[] usersToSize = arguments.Array[1].Split('.');
                            for (int i = 0; i < usersToSize.Length; i++)
                            {
                                Player.List.ToList().Find(x => x.Id.ToString().Contains(usersToSize[i])).Kill();
                            }
                            response = "Killed";
                            return true;
                        }
                        else
                        {
                            Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1])).Kill();
                            response = "Killed";
                            return true;
                        }
                    }
                }
                else
                {
                    response = "No user id given";
                    return false;
                }
            }
            else
            {
                response = "You need permission Player mangment";
                return false;
            }
        }
    }
}
