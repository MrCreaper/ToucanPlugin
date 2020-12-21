using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Nuke : ICommand
    {
        public string Command { get; } = "nuke";

        public string[] Aliases { get; } = { "nuke" };

        public string Description { get; } = "Manage the Nuke: lock, unlock, on, off, open, close, timer, shake, start, stop, detonate, blast";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            List<string> args = new List<string>(arguments.Array);
            if (Sender.CheckPermission(PlayerPermissions.WarheadEvents))
                switch (args[1])
                {
                    case "lock":
                        Warhead.IsLocked = true;
                        response = "Nuke locked.";
                        return true;
                    case "unlock":
                        Warhead.IsLocked = false;
                        response = "Nuke unlocked.";
                        return true;
                    case "on":
                        Warhead.LeverStatus = true;
                        response = "Nuke active.";
                        return true;
                    case "off":
                        Warhead.LeverStatus = false;
                        response = "Nuke deactivated.";
                        return true;
                    case "open":
                        Warhead.IsKeycardActivated = true;
                        response = "Nuke key thing open.";
                        return true;
                    case "close":
                        Warhead.IsKeycardActivated = false;
                        response = "Nuke key thing closed.";
                        return true;
                    case "timer":
                        if (arguments.Array[2] != null)
                        {
                            Warhead.DetonationTimer = float.Parse(arguments.Array[2]);
                            response = $"Nuke timer set to {arguments.Array[2]}";
                            return true;
                        }
                        else
                        {
                            response = $"No time given";
                            return true;
                        }
                    case "shake":
                        Warhead.Shake();
                        response = "Shaking..";
                        return true;
                    case "start":
                        Warhead.Start();
                        response = "Starting nuke...";
                        return true;
                    case "stop":
                        Warhead.Stop();
                        response = "Restarting systems...";
                        return true;
                    case "detonate":
                        Warhead.Detonate();
                        response = "! DETONATING ALPHA WARHEAD !";
                        return true;
                    case "blast":
                        Warhead.Controller._blastDoors[0].isClosed = true;
                        response = "Closing blast DOOR?";
                        return true;
                    default:
                        response = "Please add a subcommand";
                        return false;
                }
            else
            {
                response = "You need permission WarheadEvents to use this command.";
                return false;
            }
        }
    }
}
