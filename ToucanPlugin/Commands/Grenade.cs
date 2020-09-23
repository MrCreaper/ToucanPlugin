using CommandSystem;
using Exiled.API.Features;
using Grenades;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Grenade : ICommand
    {
        public string Command { get; } = "grenade";

        public string[] Aliases { get; } = { "gre" };

        public string Description { get; } = "Throw a grenade at someones feet";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
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
                               p.GrenadeManager.CmdThrowGrenade(p.Id, bool.Parse(arguments.Array[2]), int.Parse(arguments.Array[3]));
                            });
                            response = "Grenaded everyone";
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
                                        user.GrenadeManager.CmdThrowGrenade(user.Id, bool.Parse(arguments.Array[2]), int.Parse(arguments.Array[3]));
                                });
                                response = "Grenaded";
                                return true;
                            }
                            else
                            {
                                Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1])).GrenadeManager.CmdThrowGrenade(int.Parse(arguments.Array[1]), bool.Parse(arguments.Array[2]), int.Parse(arguments.Array[3]));
                                response = "Grenaded";
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
    }
}
