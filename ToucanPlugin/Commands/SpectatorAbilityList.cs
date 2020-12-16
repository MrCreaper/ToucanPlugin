using CommandSystem;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SpectatorAbilityList : ICommand
    {
        public static string SpectatorAbilityStock { get; set; } = null;
        public string Command { get; } = "spectatorabilitylist";

        public string[] Aliases { get; } = { "sal" };

        public string Description { get; } = "Get your bullying shit!";
        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (SpectatorAbilityStock != null)
            {
                response = SpectatorAbilityStock;
                return true;
            }
            else
            {
                response = $"Sorry the Spectator Ability List hasnt been deliverd yet, try again later.";
                return false;
            }
        }
    }
}
