using CommandSystem;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Balance : ICommand
    {
        readonly Tcp tcp = new Tcp();
        public string Command { get; } = "balance";

        public string[] Aliases { get; } = { "bal", "coins" };

        public string Description { get; } = "See how many coins do you have.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender player)
            {
                tcp.Send($"balance {player.SenderId}");
                response = $"Getting balance...";
                return true;
            }
            else
            {
                response = $"Fuck off";
                return false;
            }
        }
    }
}
