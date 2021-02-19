using CommandSystem;
using GameCore;
using System;
using Exiled.API.Features;
using System.Linq;
using RemoteAdmin;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Buy : ICommand
    {
        readonly Tcp tcp = new Tcp();
        public string Command { get; } = "buy";

        public string[] Aliases { get; } = { "b" };

        public string Description { get; } = "Buy stuff to ruin other peoples lives";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (!Tcp.IsConnected())
            {
                response = $"Sorry, we have lost connection to Toucan Servers. Try again in a few minutes.";
                return false;
            }
            if (Store.StoreStock == null)
            {
                response = $"Sorry the store hasnt been deliverd yet, try again later.";
                return false;
            }
            if (!ToucanPlugin.Singleton.Config.CanBuy)
            {
                response = $"Sorry but buying stuff is currently disable by the owner.";
                return false;
            }
            if (!(Sender is CommandSender player))
            {
                response = $"Fuck off";
                return false;
            }
            if (!Round.IsStarted)
            {
                response = $"Wait for the round to start!";
                return false;
            }
            if (!Player.List.ToList().Find(x => x.UserId.Contains(player.SenderId)).IsAlive)
            {
                response = $"You can only buy stuff when your alive.";
                return false;
            }
            if (Player.List.ToList().Find(x => x.UserId.Contains(player.SenderId)).Team != Team.SCP)
            {
                response = $"How will you use the item whit your stinky fingies? (No buying stuff as SCP)";
                return false;
            }
            string[] args = arguments.Array;
            if (args.Count() == 1)
            {
                response = $"Didnt add wished item.";
                return false;
            }
            var isNumeric = int.TryParse(args[1], out int itemNum);
            if (!isNumeric)
            {
                response = "Hes power is over -9000! It isnt that good now right?";
                return false;
            }
            if (itemNum >= 1)
            {
                response = $"Yeah can i have a... {args[1]}?";
                return false;
            }
            if (itemNum > Store.StoreStock.Split('\n').Length)
            {
                response = "[That number] Its too big! Thats what she said.";
                return false;
            }
            // God so many fucking checks


            tcp.Send($"buy {player.SenderId} {args[1]}");
            response = "Please standby...";
            return true;

        }
    }
}
