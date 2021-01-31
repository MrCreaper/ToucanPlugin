using CommandSystem;
using Exiled.API.Features;
using Grenades;
using System;
using System.Diagnostics;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    class Ping : ICommand
    {
        public static Stopwatch st = new Stopwatch();
        Tcp tcp = new Tcp();
        public string Command { get; } = "ping";

        public string[] Aliases { get; } = {  };

        public string Description { get; } = "Ping the connected Toucan server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            Player p = Player.List.ToList().Find(x => x.Sender == Sender);
            if (!Tcp.IsConnected())
            {
                response = "What the hell do you want me to ping here?";
                return false;
            }
            if (!Tcp.auth)
            {
                response = "Not authenticated";
                return false;
            }
            if (st.IsRunning)
            {
                response = "Oh shit, is it that slow?\nWell just wait a bit alrigth?";
                return false;
            }
            tcp.Send("ping");
            st.Start();
            response = "Pinging...";
            return true;
        }
    }
}
