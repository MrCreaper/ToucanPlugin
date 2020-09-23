using CommandSystem;
using Exiled.API.Features;
using System;
using System.Linq;
using UnityEngine;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Rotation : ICommand
    {
        public string Command { get; } = "rotation";

        public string[] Aliases { get; } = { "rot" };

        public string Description { get; } = "A command that can change a entities rotation";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
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
                                    user.Rotation = new Vector3(float.Parse(arguments.Array[2]), float.Parse(arguments.Array[3]), float.Parse(arguments.Array[4]));
                                });
                                response = "Rotation set to everyone!";
                                return true;
                            }
                            else
                            {
                                if (arguments.Array[1].Contains("."))
                                {
                                    String[] usersToSize = arguments.Array[1].Split('.');
                                    for (int i = 0; i < usersToSize.Length; i++)
                                    {
                                        Player.List.ToList().Find(x => x.Id.ToString().Contains(usersToSize[i])).Rotation = new Vector3(float.Parse(arguments.Array[2]), float.Parse(arguments.Array[3]), float.Parse(arguments.Array[4]));
                                    }
                                    response = "Rotation set!";
                                    return true;
                                }
                                else
                                {
                                    Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1])).Rotation = new Vector3(float.Parse(arguments.Array[2]), float.Parse(arguments.Array[3]), float.Parse(arguments.Array[4]));
                                    response = "Rotation set!";
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            response = "No Z Rotation given";
                            return false;
                        }
                    }
                    else
                    {
                        response = "No Y Rotation given";
                        return false;
                    }
                }
                else
                {
                    response = "No X Rotation given";
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
