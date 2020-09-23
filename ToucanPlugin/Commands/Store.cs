using CommandSystem;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Store : ICommand
    {
        public static string StoreStock { get; set; } = null;
        public string Command { get; } = "store";

        public string[] Aliases { get; } = { "s" };

        public string Description { get; } = "Find stuff to buy and help you in game!";
        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (StoreStock != null)
            {
                response = StoreStock;
                return true;
            }
            else
            {
                response = $"Sorry the store hasnt been deliverd yet, try again later.";
                return false;
            }
        }
    }
}
