using CommandSystem;
using Exiled.API.Features;
using System;
using System.Linq;
using UnityEngine;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Rotations : ICommand
    {
        public string Command { get; } = "rotations";

        public string[] Aliases { get; } = { "rots" };

        public string Description { get; } = "A command that can change a rotation size 2";

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
                                Player.List.ToList().ForEach(user =>
                                {
                                    user.Rotations = new Vector2(float.Parse(arguments.Array[2]), float.Parse(arguments.Array[3]));
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
                                        Player.List.ToList().Find(x => x.Id.ToString().Contains(usersToSize[i])).Rotations = new Vector2(float.Parse(arguments.Array[2]), float.Parse(arguments.Array[3]));
                                    }
                                    response = "Size set!";
                                    return true;
                                }
                                else
                                {
                                    Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1])).Rotations = new Vector2(float.Parse(arguments.Array[2]), float.Parse(arguments.Array[3]));
                                    response = "Size set!";
                                    return true;
                                }
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
    }
}
