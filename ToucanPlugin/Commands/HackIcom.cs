using CommandSystem;
using Exiled.API.Features;
using GameCore;
using RemoteAdmin;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class HackIcom : ICommand
    {
        readonly MessageResponder mr = new MessageResponder();
        public string Command { get; } = "HackIcom";

        public string[] Aliases { get; } = { "HackI", "HaI" };

        public string Description { get; } = "Disable the Intercom as a CI Hacker";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is PlayerCommandSender PCplayer)
            {
                Player p = Player.List.ToList().Find(x => x.UserId == PCplayer.CCM.UserId);
                if (mr.ChaosHacker.Contains(p))
                {
                    if (p.AdrenalineHealth >= 60)
                    {
                        Intercom.host.remainingCooldown =+ 35;
                        Intercom.host._intercomText = $"Lol u ben haxed\n-CI haxer men ({PCplayer.Nickname})";
                        response = $"Hacking the intercom...";
                        return false;
                    }
                    else
                    {
                        response = $"You dont have enoght ap to hack the intercom, you need {60 - p.AdrenalineHealth} more ap.";
                        return false;
                    }
                }
                else
                {
                    response = $"You need to be a CI hacker to use thees commands!";
                    return false;
                }
            }
            else
            {
                response = $"Fuck off";
                return true;
            }
        }
    }
}
