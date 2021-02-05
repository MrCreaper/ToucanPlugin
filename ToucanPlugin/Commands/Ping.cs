using CommandSystem;
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
        public string Command { get; } = "tping";

        public string[] Aliases { get; } = { "toucanping"  };

        public string Description { get; } = "Ping the connected Toucan server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (!ToucanPlugin.Instance.Config.Debug)
            {
                response = "Maybe dont..";
                return false;
            }
            if (!Tcp.IsConnected() || !Tcp.connected)
            {
                response = "What the hell do you want me to ping here?";
                return false;
            }
            if (!Tcp.auth)
            {
                response = "Not authenticated";
                return false;
            }
            if (IdleMode.IdleModeActive)
            {
                response = "Yeah, connection disabled during idle mode.";
                return false;
            }
            if (st.IsRunning)
            {
                response = "Oh shit, is it that slow?\nWell idk just wait a bit alrigth?";
                return false;
            }
            if (arguments.Array[1] == "cur")
            {
                tcp.Send("ping");
                st.Start();
            }
            else
            {
                Exiled.API.Features.Log.Info($"results:\n{tcp.Ping()} ");
            }
            response = "Pinging...";
            return true;
        }
    }
}
