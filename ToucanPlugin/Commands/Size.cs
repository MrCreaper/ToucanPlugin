using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Size : ICommand
    {
        public string Command { get; } = "size";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "A command that can change a players size [size [id] [x] [y] [z]]";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender.CheckPermission(PlayerPermissions.PlayersManagement))
            {
                List<string> args = new List<string>(arguments.Array.ToList());
                float x = 0;
                float y = 0;
                float z = 0;
                if (args[1] != null)
                {
                    if (args[2] != null)
                    {
                        var isNumericX = float.TryParse(args[2], out float xout);
                        if (isNumericX)
                        {
                            x = xout;
                            if (args[3] != null)
                            {
                                var isNumericY = float.TryParse(args[3], out float yout);
                                if (isNumericY)
                                {
                                    y = yout;
                                    if (args[4] != null)
                                    {
                                        var isNumericZ = float.TryParse(args[4], out float zout);
                                        if (isNumericZ)
                                            z = zout;
                                        else
                                        {
                                            response = $"Z is invalid";
                                            return true;
                                        }
                                    }
                                    else
                                        z = 1;
                                }
                                else
                                {
                                    response = $"Y is invalid";
                                    return true;
                                }
                            }
                            else
                                y = 1;
                        }
                        else
                        {
                            response = $"X is invalid";
                            return true;
                        }
                    }
                    else
                        x = 1;
                    if (args[1] == "all")
                    {
                        Player.List.ToList().ForEach(user =>
                        {
                            user.Scale = new Vector3(x, y, z);
                        });
                        response = "Size set to everyone!";
                        return true;
                    }
                    else
                    {
                        if (args[1].Contains("."))
                        {
                            String[] usersToSize = args[1].Split('.');
                            for (int i = 0; i < usersToSize.Length; i++)
                            {
                                Player.List.ToList().Find(u => u.Id.ToString().Contains(usersToSize[i])).Scale = new Vector3(x, y, z);
                            }
                            response = "Size set!";
                            return true;
                        }
                        else
                        {
                            Player.List.ToList().Find(u => u.Id.ToString().Contains(args[1])).Scale = new Vector3(x, y, z);
                            response = "Size set!";
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
                response = "You need player managment permission to use this command!";
                return false;
            }
        }
    }
}
