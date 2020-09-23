using CommandSystem;
using Exiled.API.Features;
using System;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class StartDec : ICommand
    {
        public string Command { get; } = "startdecondamination";

        public string[] Aliases { get; } = { "startdec", "sdec" };

        public string Description { get; } = "Activate decondamination when ever you want to.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Map.IsLCZDecontaminated == false)
            {
                response = "Starting Decontamination...";
                Map.StartDecontamination();
                return true;
            }
            else
            {
                response = "Decontamination has already happend.";
                return false;
            }
        }
    }
}
