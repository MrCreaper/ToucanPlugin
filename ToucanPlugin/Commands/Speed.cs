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
                if (args.Count == 1)
                {
                    response = $"speed [speed] [id [opt]] [w/r [walk/run] [opt]]";
                    return true;
                }
                var isNumeric = float.TryParse(args[1], out float speed);
                if (!isNumeric)
                {
                    response = $"Thats not a float (number)";
                    return true;
                }
                Player p = Player.List.ToList().Find(x => x.UserId == PCplayer.SenderId); //me
                if (args.Count >= 3)
                {
                    var isNumericId = int.TryParse(args[1], out int customId);
                    if (!isNumericId)
                    {
                        response = $"That id is not a number";
                        return true;
                    }
                    p = Player.List.ToList().Find(x => x.Id == customId);
                }
                if (p == null)
                {
                    response = $"Invalid Player.";
                    return true;
                }
                if(args.Count < 4)
                {
                    p.ReferenceHub.characterClassManager.Classes.ToList().Find(x => x.roleId == p.Role).runSpeed = speed;
                    response = $"Set run speed to {speed}";
                    return true;
                }
                switch (args[3])
                {
                    default:
                    case "walk":
                    case "w":
                        p.ReferenceHub.characterClassManager.Classes.ToList().Find(x => x.roleId == p.Role).walkSpeed = speed;
                        response = $"Set walk speed to {speed} for {p.Nickname}";
                        return true;
                    case "run":
                    case "r":
                        p.ReferenceHub.characterClassManager.Classes.ToList().Find(x => x.roleId == p.Role).runSpeed = speed;
                        response = $"Set run speed to {speed} for {p.Nickname}";
                        return true;
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
