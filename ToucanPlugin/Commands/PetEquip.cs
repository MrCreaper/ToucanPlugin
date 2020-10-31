using CommandSystem;
using Exiled.API.Features;
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
            if (Sender is CommandSender PCplayer)
            {
                Player p = Player.List.ToList().Find(x => x.Sender == PCplayer);
                if (Tcp.IsConnected())
                {
                    string[] args = arguments.Array;
                    if (args[1] != null)
                    {
                        var isNumeric = int.TryParse(args[1], out int itemNum);
                        if (isNumeric == true)
                        {
                            if (itemNum >= 1)
                            {
                                Tcp.Send($"pet {p.UserId} {itemNum - 1}");
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
                        Log.Info(Handlers.Player.petConnections[p.UserId]);
                        response = $"No pet equiped!";
                        return false;
                        /*if (Handlers.Player.petConnections[PCplayer.CCM.UserId]) // ???
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
                        }*/
                    }
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
