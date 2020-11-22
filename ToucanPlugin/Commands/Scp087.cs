using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;
using Exiled.API.Features;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Scp087 : ICommand
    {
        public static Player Subject = null;
        public static bool InEffect = false;
        public string Command { get; } = "Scp087";

        public string[] Aliases { get; } = { "087", "87" };

        public string Description { get; } = "Send someone down 087";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            List<string> args = new List<string>(arguments.Array);
            if (Sender.CheckPermission(PlayerPermissions.FacilityManagement))
            {
                if (args[1] != null)
                {
                    if (args[1] == "relase" || args[1] == "r")
                    {
                        if (Subject != null && InEffect == true)
                        {
                            InEffect = false;
                            Subject = null;
                            response = "Subject Relased.";
                            return true;
                        }
                        else
                        {
                            response = "No subject to relase.";
                            return false;
                        }
                    }
                    else
                    {
                        var isNumeric = int.TryParse(args[1], out int SubjectId);
                        if (isNumeric == true)
                        {
                            if (Player.List.ToList().Find(x => x.Id == SubjectId) != null)
                            {
                                Room Scp173Room = Map.Rooms.ToList().Find(x => x.Type == Exiled.API.Enums.RoomType.Lcz173);
                                Subject = Player.List.ToList().Find(x => x.Id == SubjectId);
                                Scp173Room.TurnOffLights(5);
                                Scp173Room.Doors.ToList().ForEach(d =>
                                {
                                    d.isOpen = false;
                                    d.locked = true;
                                    d.GrenadesResistant = true;
                                    d.Buttons.ToList().ForEach(b => b.button.name = "SCP-087");
                                });
                                InEffect = true;
                                Subject.Position = Scp173Room.Position;
                                Subject.MaxEnergy = 0;
                                Subject.Energy = 0;
                                Subject.DisableAllEffects();
                                Subject.ClearInventory();
                                Subject.Inventory.AddNewItem(ItemType.Flashlight);
                                response = $"Subject {Subject.Nickname} Sent into Scp-087";
                                return true;
                            }
                            else
                            {
                                response = "Invalid Id.";
                                return false;
                            }
                        }
                        else
                        {
                            response = "Invalid Id.";
                            return false;
                        }
                    }
                }
                else
                {
                    response = "Missing Subject ID";
                    return false;
                }
            }
            else
            {
                response = "Facility Managment is required to access SCP-087";
                return false;
            }
        }
        public static void StartChecking087Room()
        {
            Task.Factory.StartNew(() =>
            {
                while (InEffect && Round.IsStarted)
                {
                    Room Scp173Room = Map.Rooms.ToList().Find(x => x.Type == Exiled.API.Enums.RoomType.Lcz173);
                    if (!Scp173Room.LightsOff)
                        Scp173Room.TurnOffLights(5);
                }
            });
        }
    }
}
