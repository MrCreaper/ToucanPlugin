using CommandSystem;
using Exiled.API.Features;
using GameCore;
using RemoteAdmin;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class HackLights : ICommand
    {
        MessageResponder mr = new MessageResponder();
        public string Command { get; } = "HackLight";

        public string[] Aliases { get; } = { "HackLights", "HackI", "HaI" };

        public string Description { get; } = "Hack the lights as a CI Hacker";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is PlayerCommandSender PCplayer)
            {
                Player p = Player.List.ToList().Find(x => x.UserId == PCplayer.CCM.UserId);
                if (mr.ChaosHacker.Contains(p))
                {
                    if (p.AdrenalineHealth >= 75)
                    {
                        Map.TurnOffAllLights(10);
                        response = $"Hacking the lights...";
                        return false;
                    }
                    else
                    {
                        response = $"You dont have enoght ap to hack the lights, you need {75 - p.AdrenalineHealth} more ap.";
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
