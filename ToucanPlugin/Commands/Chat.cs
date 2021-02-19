using CommandSystem;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Chat : ICommand
    {
        readonly Tcp Tcp = new Tcp();
        public string Command { get; } = "tchat";

        public string[] Aliases { get; } = { "tc" };

        public string Description { get; } = "Chat throght in game or discord.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender player)
            {
                if (Tcp.IsConnected())
                {
                    string msg = "";
                    for (int i = 1; i <= arguments.Array.Length - 1; i++)
                    {
                        if (i == 1)
                            msg += $"{arguments.Array[i]}";
                        else
                            msg += $" {arguments.Array[i]}";
                    }
                    string FullMsgD = $"**{player.Nickname}** (*{player.SenderId}*): `{msg}`";
                    SendMsgInGame(player.Nickname,msg);
                    Tcp.Send($"msg {FullMsgD}");
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
        public void SendMsgInGame(string Sender, string Msg)
        {
            Exiled.API.Features.Player.List.ToList().ForEach(p => { 
                p.SendConsoleMessage($"{Sender}: {Msg}", "#ff8c00");
                p.ShowHint(ToucanPlugin.Singleton.Config.ChatHint.Replace("{M}", Msg).Replace("{S}", Sender));
            });
        }
    }
}
