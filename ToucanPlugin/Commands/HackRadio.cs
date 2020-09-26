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
        MessageResponder mr = new MessageResponder();

        public static bool radioHacked { get; set; } = false;
        public string Command { get; } = "HackRadio";

        public string[] Aliases { get; } = { "HackR", "HaR" };

        public string Description { get; } = "Disable the Radio for the MTF as a CI Hacker";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is PlayerCommandSender PCplayer)
            {
                Player p = Player.List.ToList().Find(x => x.UserId == PCplayer.CCM.UserId);
                if (mr.ChaosHacker.Contains(p))
                {
                    if (p.AdrenalineHealth >= 65)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            radioHacked = true;
                            Task.Delay(25000); // 25s
                            radioHacked = false;
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
