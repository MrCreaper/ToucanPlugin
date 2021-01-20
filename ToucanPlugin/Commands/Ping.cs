using CommandSystem;
using Exiled.API.Features;
using Grenades;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Ping : ICommand
    {
        Tcp tcp = new Tcp();
        public string Command { get; } = "ping";

        public string[] Aliases { get; } = {  };

        public string Description { get; } = "Ping the connected Toucan server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            Player p = Player.List.ToList().Find(x => x.Sender == Sender);
            if(!p.IsHost)
            {
                response = "No, sorry.";
                return false;
            }
            if (!Tcp.IsConnected())
            {
                response = "What the hell do you want me to ping here?";
                return false;
            }
            if (!Tcp.auth)
            {
                response = "Not yet authenticated";
                return false;
            }
            tcp.Send("ping");
            response = "Pinging...";
            return true;
        }
    }
}
