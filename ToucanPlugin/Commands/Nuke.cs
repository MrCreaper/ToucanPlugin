using CommandSystem;
using Exiled.API.Features;
using System;

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
            if (Sender.CheckPermission(PlayerPermissions.WarheadEvents))
            {
                if (arguments.Array[1] == "lock")
                {
                    Warhead.IsLocked = true;
                    response = "Nuke locked.";
                    return true;
                }
                else if (arguments.Array[1] == "unlock")
                {
                    Warhead.IsLocked = false;
                    response = "Nuke unlocked.";
                    return true;
                }
                else
                if (arguments.Array[1] == "on")
                {
                    Warhead.LeverStatus = true;
                    response = "Nuke active.";
                    return true;
                }
                else
                if (arguments.Array[1] == "off")
                {
                    Warhead.LeverStatus = false;
                    response = "Nuke deactivated.";
                    return true;
                }
                else
                if (arguments.Array[1] == "open")
                {
                    Warhead.IsKeycardActivated = true;
                    response = "Nuke key thing open.";
                    return true;
                }
                else
                if (arguments.Array[1] == "close")
                {
                    Warhead.IsKeycardActivated = false;
                    response = "Nuke key thing closed.";
                    return true;
                }
                else
                if (arguments.Array[1] == "timer")
                {
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
                }
                else
                if (arguments.Array[1] == "shake")
                {
                    Warhead.Shake();
                    response = "Shaking..";
                    return true;
                }
                else
                if (arguments.Array[1] == "start")
                {
                    Warhead.Start();
                    response = "Starting nuke...";
                    return true;
                }
                else
                if (arguments.Array[1] == "stop")
                {
                    Warhead.Stop();
                    response = "Restarting systems...";
                    return true;
                }
                else
                if (arguments.Array[1] == "detonate")
                {
                    Warhead.Detonate();
                    response = "! DETONATING ALPHA WARHEAD !";
                    return true;
                }
                else
                if (arguments.Array[1] == "blast")
                {
                    Warhead.Controller._blastDoors[0].isClosed = true;
                    response = "Closing blast DOOR?";
                    return true;
                }
                else
                {
                    response = "Please add a subcommand";
                    return false;
                }
            }
            else
            {
                response = "You need permission WarheadEvents to use this command.";
                return false;
            }
        }
    }
}
