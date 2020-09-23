using CommandSystem;
using Exiled.API.Features;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Ahp : ICommand
    {
        public string Command { get; } = "adrenalinhp";

        public string[] Aliases { get; } = { "ahp" };

        public string Description { get; } = "Set some fuckers ahp over 9000!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender.CheckPermission(PlayerPermissions.PlayersManagement))
            {
                if (arguments.Array[1] != null)
                {
                    if (arguments.Array[2] != null)
                    {
                        if (arguments.Array[1].Contains("."))
                        {
                            String[] usersToSize = arguments.Array[1].Split('.');
                            for (int i = 0; i < usersToSize.Length; i++)
                            {
                                Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1])).AdrenalineHealth = int.Parse(arguments.Array[2]);
                            }
                            response = "Ahp set";
                            return true;
                        }
                        else
                        {
                            Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1])).AdrenalineHealth = int.Parse(arguments.Array[2]);
                            response = "Ahp set";
                            return true;
                        }
                    }
                    else
                    {
                        response = "No ahp given";
                        return false;
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
