using CommandSystem;
using System;
using Exiled.API.Features;
using System.Linq;
using System.Collections.Generic;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Speed : ICommand
    {
        public string Command { get; } = "speed";

        public string[] Aliases { get; } = { "sp" };

        public string Description { get; } = "Change your/someone movement speed";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            List<string> args = new List<string>(arguments.Array.ToList());
            if (Sender is CommandSender PCplayer)
            {
                if (args[1] != null)
                {
                    var isNumeric = float.TryParse(args[1], out float speed);
                    if (isNumeric)
                    {
                        Player p;
                        if (args[2] == null)
                            p = Player.List.ToList().Find(x => x.Sender == PCplayer);
                        else
                        {
                            var isNumericId = int.TryParse(args[1], out int customId);
                            if (isNumericId)
                                p = Player.List.ToList().Find(x => x.Id == customId);
                            else
                            {
                                response = $"That id is not a number";
                                return false;
                            }
                        }
                        switch (args[2])
                        {
                            default:
                                p.ReferenceHub.characterClassManager.Classes.ToList().ForEach(x => x.walkSpeed = speed);
                                response = $"Set run speed to {speed}";
                                return true;
                                break;
                            case "walk":
                            case "w":
                                p.ReferenceHub.characterClassManager.Classes.ToList().ForEach(x => x.walkSpeed = speed);
                                response = $"Set walk speed to {speed} for {p.Nickname}";
                                return true;
                                break;
                            case "run":
                            case "r":
                                p.ReferenceHub.characterClassManager.Classes.ToList().ForEach(x => x.runSpeed = speed);
                                response = $"Set run speed to {speed} for {p.Nickname}";
                                return true;
                                break;
                        }
                    }
                    else
                    {
                        response = $"Thats not a float (number)";
                        return false;
                    }
                }
                else
                {
                    response = $"Speed";
                    return false;
                }
            }
            else
            {
                response = $"Fuck off";
                return false;
            }
        }
    }
}
