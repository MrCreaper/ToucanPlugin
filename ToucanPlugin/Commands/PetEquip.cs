using CommandSystem;
using RemoteAdmin;
using System;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class PetEquip : ICommand
    {
        private readonly Tcp Tcp = new Tcp();
        public string Command { get; } = "petequip";

        public string[] Aliases { get; } = { "pe" };

        public string Description { get; } = "Equip one of your cute pets...";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is PlayerCommandSender PCplayer)
            {
                string[] args = arguments.Array;
                if (args[1] != null)
                {
                    var isNumeric = int.TryParse(args[1], out int itemNum);
                    if (isNumeric == true)
                    {
                        if (itemNum >= 1)
                        {
                            Tcp.Send($"pet {PCplayer.CCM.UserId} {arguments.Array[1]}");
                            response = $"Equiping pet...";
                            return true;
                        }
                        else
                        {
                            response = "OK, maybe, if i (Mr.Creaper) am asked to make the pets into dinnerbone then maybe, one day, you can use this command.\nFor now dont.";
                            return false;
                        }
                    }
                    else
                    {
                        response = $"A number would be nice...";
                        return false;
                    }
                }
                else
                {
                    if (Handlers.Player.petConnections[PCplayer.CCM.UserId] != null) // ???
                    {
                        Handlers.Player.petConnections.Remove(PCplayer.CCM.UserId);
                        NPCS.Npc.Dictionary.ToList().Find(x => x.Value.GetInstanceID() == Handlers.Player.petConnections[PCplayer.CCM.UserId]).Value.Kill(false);
                        response = $"Unequiping...";
                        return true;
                    }
                    else
                    {
                        response = $"No pet equiped!";
                        return false;
                    }
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
