using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Chat : ICommand
    {
        readonly Tcp Tcp = new Tcp();
        public string Command { get; } = "tchat";

        public string[] Aliases { get; } = { "tc" };

        public string Description { get; } = "Chat throght in game, and thorugh our discord!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender player)
            {
                if (Tcp.IsConnected())
                {
                    String Message = "msg ";
                    String msg = "";
                    for (int i = 0; i <= arguments.Array.Length; i++)
                    {
                        msg += $" {arguments.Array[i]}";
                    }
                    Message += player.Nickname;
                    Message += $" ({player.SenderId}): {msg}";
                    Tcp.Send(Message);
                    response = "Message Sent!";
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
                response = "Sorry, cant do that.";
                return true;
            }
        }
    }
}
