using CommandSystem;
using Exiled.API.Features;
using Grenades;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Kick : ICommand
    {
        public string Command { get; } = "kick";

        public string[] Aliases { get; } = {  };

        public string Description { get; } = "Kick people in the ass.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender.CheckPermission(PlayerPermissions.KickingAndShortTermBanning))
            {
                if (arguments.Array[1] != null)
                {
                    if (arguments.Array[2] != null)
                    {
                        if (arguments.Array[3] != null)
                        {
                            if (arguments.Array[1] == "all")
                            {
                                Player.List.ToList().ForEach(p =>
                                {
                                    p.Kick("Lol you have been kicked");
                                });
                                response = "Kicked everyone";
                                return true;
                            }
                            else
                            {
                                if (arguments.Array[1].Contains("."))
                                {
                                    String[] usersToSize = arguments.Array[1].Split('.');
                                    Player.List.ToList().ForEach(user =>
                                    {
                                        if (usersToSize.Contains(user.Id.ToString()))
                                            user.Kick("Lol you have been kicked");
                                    });
                                    response = "Kicked";
                                    return true;
                                }
                                else
                                {
                                    Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1])).Kick("Lol you have been kicked");
                                    response = "Kicked";
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            response = "No time given";
                            return false;
                        }
                    }
                    else
                    {
                        response = "slow throw not given";
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
                response = "You need permissiong for kicking and short term banning to use this.";
                return false;
            }
        }
    }
}
