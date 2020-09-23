using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions;
using System;
using System.Linq;
using UnityEngine;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Scale : ICommand
    {
        public string Command { get; } = "scale";

        public string[] Aliases { get; } = { "size" };

        public string Description { get; } = "A command that can change a entities size";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender.CheckPermission(PlayerPermissions.PlayersManagement))
            {
                if (arguments.Array[1] != null)
                {
                    if (arguments.Array[2] != null)
                    {
                        if (arguments.Array[3] != null)
                        {
                            if (arguments.Array[4] != null)
                            {
                                if (arguments.Array[1] == "all")
                                {
                                    Player.List.ToList().ForEach(user =>
                                    {
                                        user.Scale = new Vector3(float.Parse(arguments.Array[2]), float.Parse(arguments.Array[3]), float.Parse(arguments.Array[4]));
                                    });
                                    response = "Size set to everyone!";
                                    return true;
                                }
                                else
                                {
                                    if (arguments.Array[1].Contains("."))
                                    {
                                        String[] usersToSize = arguments.Array[1].Split('.');
                                        for (int i = 0; i < usersToSize.Length; i++)
                                        {
                                            Player.List.ToList().Find(x => x.Id.ToString().Contains(usersToSize[i])).Scale = new Vector3(float.Parse(arguments.Array[2]), float.Parse(arguments.Array[3]), float.Parse(arguments.Array[4]));
                                        }
                                        response = "Size set!";
                                        return true;
                                    }
                                    else
                                    {
                                        Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1])).Scale = new Vector3(float.Parse(arguments.Array[2]), float.Parse(arguments.Array[3]), float.Parse(arguments.Array[4]));
                                        response = "Size set!";
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                response = "No Z Size given";
                                return false;
                            }
                        }
                        else
                        {
                            response = "No Y Size given";
                            return false;
                        }
                    }
                    else
                    {
                        response = "No X Size given";
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
                response = "You need player managment permission to use this command!";
                return false;
            }
        }
    }
}
