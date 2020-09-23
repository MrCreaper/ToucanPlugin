using CommandSystem;
using Exiled.API.Features;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    //[CommandHandler(typeof(ClientCommandHandler))]
    class InchByInch : ICommand
    {
        public string Command { get; } = "Inch";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "You become smaller over time and your health will go down as well, and picking up items will become a lot more harder, the only cure is 500 (From the game \"Inch by Inch\")";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (arguments.Array[1] != null)
            {
                    if (arguments.Array[1].Contains("."))
                {
                    String[] usersToSize = arguments.Array[1].Split('.');
                    for (int i = 0; i < usersToSize.Length; i++)
                    {
                        Player.List.ToList().Find(x => x.Id.ToString().Contains(usersToSize[i]));
                    }
                    response = "He has drunk the coffee!";
                    return true;
                }
                else
                {
                    Player.List.ToList().Find(x => x.Id.ToString().Contains(arguments.Array[1]));
                    response = "He has drunk the coffee!";
                    return true;
                }
            }
            else
            {
                response = "No user id given";
                return false;
            }
        }
    }
}
