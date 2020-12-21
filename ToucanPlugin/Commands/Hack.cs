using CommandSystem;
using Exiled.API.Features;
using GameCore;
using RemoteAdmin;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Hack : ICommand
    {
        public static bool RadioHacked { get; set; } = false;
        public string Command { get; } = "hack";

        public string[] Aliases { get; } = { "h" };

        public string Description { get; } = "CI Hacker Abilities";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            string[] args = arguments.Array;
            if (Sender is CommandSender PCplayer)
            {
                Player p = Player.List.ToList().Find(x => x.Sender == PCplayer);
                Handlers.CustomPersonelSpawns per = Handlers.Player.perConnections[p];
                switch (args[1])
                {
                    default:
                        response = $"Missing Subcommand.\nicom - light - radio";
                        return false;
                    case "icom":
                        if (!per.Abilities.Contains(Handlers.AbilityType.IcomDisabling))
                        {
                            response = $"You dont have this ability!";
                            return false;
                        }
                        if (p.AdrenalineHealth >= 60)
                        {
                            response = $"You dont have enoght ap to hack the intercom, you need {60 - p.AdrenalineHealth} more ap.";
                            return false;
                        }
                        Intercom.host.remainingCooldown += 35;
                        Intercom.host.Network_intercomText = $"Lol u ben haxed\n-CI haxer men ({PCplayer.Nickname})";
                        response = $"Intercom Disabled";
                        return false;
                    case "light":
                    case "lights":
                        if (!per.Abilities.Contains(Handlers.AbilityType.Blackout))
                        {
                            response = $"You dont have this ability!";
                            return false;
                        }
                        if (p.AdrenalineHealth >= 75)
                        {
                            response = $"You dont have enoght ap to hack the lights, you need {75 - p.AdrenalineHealth} more ap.";
                            return false;
                        }
                        Map.TurnOffAllLights(10);
                        response = $"Lights Disabled";
                        return false;
                    case "radio":
                        if (!per.Abilities.Contains(Handlers.AbilityType.RadioDisabling))
                        {
                            response = $"You dont have this ability!";
                            return false;
                        }
                        if (p.AdrenalineHealth >= 65)
                        {
                            response = $"You dont have enoght ap to hack the radio, you need {65 - p.AdrenalineHealth} more ap.";
                            return false;
                        }
                        Task.Factory.StartNew(() =>
                                {
                                    RadioHacked = true;
                                    Task.Delay(25000); // 25s
                                    RadioHacked = false;
                                });
                        response = $"Radio Disabled";
                        return false;
                }
            }
            else
            {
                response = $"Fuck off";
                return true;
            }
        }
        public class CIHackingAbilityConfig
        {
            public string Name { get; set; }
            public int Cost { get; set; }
        }
    }
}
