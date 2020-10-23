using CommandSystem;
using GameCore;
using System;
using Exiled.API.Features;
using System.Linq;
using RemoteAdmin;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Link : ICommand
    {
        readonly Tcp tcp = new Tcp();
        public string Command { get; } = "link";

        public string[] Aliases { get; } = { "l" };

        public string Description { get; } = "Link your steam and your discord.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender PCplayer)
            {
                Player p = Player.List.ToList().Find(x => x.Sender == PCplayer);
                string[] args = arguments.Array;
                if (args[1] != null)
                {
                    if (args[1].Contains("#"))
                    {
                        response = "Connecting...";
                        tcp.Send($"link {p.Sender.SenderId} {args[1]}");
                        return true;
                    }
                    else
                    {
                        response = "Invalid discord user. Example: EpicPerson#0000";
                        return true;
                    }
                }
                else
                {
                    response = "Please add discord.";
                    return true;
                }
            }
            else
            {
                response = "Fuck off.";
                return true;
            }
        }
    }
}
