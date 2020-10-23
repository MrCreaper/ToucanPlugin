using CommandSystem;
using GameCore;
using RemoteAdmin;
using ToucanPlugin.Gamemodes;
using System;
using Exiled.API.Features;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Report : ICommand
    {
        public string Command { get; } = "report";

        public string[] Aliases { get; } = { "r" };

        public string Description { get; } = "Report a body in the among us gamemode!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender PCplayer)
            {
                Player p = Player.List.ToList().Find(x => x.Sender == PCplayer);
                if (p.IsAlive)
                {
                    bool bodyClose = false;
                    AmongUs.DeathCords.ForEach(cords => {
                        //if (cords.x =< Player.List.ToList().Find(x => x.UserId == PCplayer.CCM.UserId).Position.x + 10) 
                            bodyClose = true;
                            });
                    if (bodyClose) {
                        AmongUs.ReportBody(p.UserId);
                        response = "Reporting!";
                        return false;
                    }
                    else
                    {
                        response = "No body close!";
                        return false;
                    }
                }
                else
                {
                    response = $"Nah, ur ded fam";
                    return false;
                }
            }
            else
            {
                response = "Fuck off";
                return false;
            }
        }
    }
}
