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
    class HackRadio : ICommand
    {
        readonly MessageResponder mr = new MessageResponder();

        public static bool RadioHacked { get; set; } = false;
        public string Command { get; } = "HackRadio";

        public string[] Aliases { get; } = { "HackR", "HaR" };

        public string Description { get; } = "Disable the Radio for the MTF as a CI Hacker";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender PCplayer)
            {
                Player p = Player.List.ToList().Find(x => x.Sender == PCplayer);
                if (mr.ChaosHacker.Contains(p))
                {
                    if (p.AdrenalineHealth >= 65)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            RadioHacked = true;
                            Task.Delay(25000); // 25s
                            RadioHacked = false;
                        });
                        response = $"Hacking the radio...";
                        return false;
                    }
                    else
                    {
                        response = $"You dont have enoght ap to hack the radio, you need {65 - p.AdrenalineHealth} more ap.";
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
