/*using CommandSystem;
using Exiled.API.Features;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Pbc : ICommand
    {
        public string Command { get; } = "privetBrodcast";

        public string[] Aliases { get; } = { "pbc" };

        public string Description { get; } = "Privetly brodcast someone";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (arguments.Array[1] != null)
            {
                if (arguments.Array[2] != null)
                {
                    if (arguments.Array[3] != null)
                    {
                        String Message = "";
                        for (int i = 3; i < arguments.Array.Length; i++)
                        {
                            Message += arguments.Array[i];
                        }
                            if (arguments.Array[1].Contains("."))
                        {
                            String[] usersToSize = arguments.Array[1].Split('.');
                            for (int i = 0; i < usersToSize.Length; i++)
                            {
                                Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1])).Broadcast((ushort)int.Parse(arguments.Array[2]), Message);
                            }
                            response = "Brodcast sent!";
                            return true;
                        }
                        else
                        {
                            Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1])).Broadcast((ushort)int.Parse(arguments.Array[2]), Message);
                            response = "Brodcast sent!";
                            return true;
                        }
                    }
                    else
                    {
                        response = "No message given";
                        return false;
                    }
                }
                else
                {
                    response = "No duration given";
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
}*/
