using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Pets : ICommand
    {
        private readonly Tcp Tcp = new Tcp();
        public string Command { get; } = "pets";

        public string[] Aliases { get; } = { "pets" };

        public string Description { get; } = "Know what pets you have unlocked.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is PlayerCommandSender PCplayer)
            {
                if (Tcp.IsConnected())
                {
                    Tcp.Send($"petsList {PCplayer.SenderId}");
                    response = $"Getting Pets list...";
                    return true;
                }
                else
                {
                    response = $"Sorry, we have lost connection to Toucan Servers. Try again in a few minutes.";
                    return false;
                }
            }
            else
            {
                response = $"Fuck off";
                return false;
            }
        }
    }
}
