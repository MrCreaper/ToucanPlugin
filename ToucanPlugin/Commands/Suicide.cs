using CommandSystem;
using System;
using Exiled.API.Features;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Suicide : ICommand
    {
        readonly Tcp Tcp = new Tcp();
        public static string StoreStock { get; set; } = null;
        public string Command { get; } = "suicide";

        public string[] Aliases { get; } = { "die" };

        public string Description { get; } = "Kill your self, the cost will change whit your class";
        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender player)
            {
                Player player1 = Player.List.ToList().Find(x => x.UserId.Contains(player.SenderId));
                if (player1.Team != Team.TUT)
                {
                    if (player1.IsAlive)
                    {
                        Tcp.Send($"suicide {player1.UserId} {player1.Team}");
                        response = $"Billing you a few coins...";
                        return true;
                    }
                    else
                    {
                        response = $"Nah.";
                        return false;
                    }
                }
                else
                {
                    response = $"Sorry but you cant kill yourself as tutorial.";
                    return false;
                }
            }
            else
            {
                response = $"You cant even kill yourself XD";
                return false;
            }
        }
    }
}
