using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;
using Exiled.API.Features;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interactables.Interobjects.DoorUtils;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
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
                    Room Scp173Room = Map.Rooms.ToList().Find(x => x.Type == Exiled.API.Enums.RoomType.Lcz173);
                    //UnityEngine.Vector3 Scp173Room000 = new UnityEngine.Vector3(Scp173Room.Position.x - 6, Scp173Room.Position.y, Scp173Room.Position.z + 2);
                    UnityEngine.Vector3 Scp173Room000 = Scp173Room.Position;
                    Scp173Room000.y += 2f;
                    float Scp087Top = Scp173Room000.y + 15;
                    float Scp087Bottom = Scp173Room000.y;
                    if (args[1] == "relase" || args[1] == "r")
                    {
                        if (Subject != null && InEffect == true)
                        {
                            InEffect = false;
                            Subject = null;
                            Scp173Room.Doors.ToList().ForEach(d =>
                            {
                                d.NetworkTargetState = true;
                                d.ServerChangeLock(DoorLockReason.SpecialDoorFeature, false);
                                d.GetComponent<Door>().GrenadesResistant = false;
                                //d.Buttons.ToList().ForEach(b => b.button.name = "SCP-087");
                            });
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
                                Subject = Player.List.ToList().Find(x => x.Id == SubjectId);
                                Scp173Room.TurnOffLights(5);
                                Scp173Room.Doors.ToList().ForEach(d =>
                                {
                                    d.NetworkTargetState = false;
                                    d.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                                    d.GetComponent<Door>().GrenadesResistant = true;
                                    //d.Buttons.ToList().ForEach(b => b.button.name = "SCP-087");
                                });
                                InEffect = true;
                                Subject.Position = Scp173Room000;
                                Subject.MaxEnergy = 0;
                                Subject.Energy = 0;
                                Subject.DisableAllEffects();
                                Subject.ClearInventory();
                                Subject.Inventory.AddNewItem(ItemType.Flashlight);
                                Task.Factory.StartNew(() =>
                                {
                                    while (true)
                                    {
                                        if (!InEffect) return;
                                        if (InEffect && Round.IsStarted)
                                        {
                                            if (!Scp173Room.LightsOff)
                                                Scp173Room.TurnOffLights(5);
                                        }
                                        if (Subject.Position.y > Scp087Bottom)
                                            Subject.Position = new UnityEngine.Vector3(Subject.Position.x, Subject.Position.y + 5, Subject.Position.z);
                                        if (Subject.Position.y < Scp087Bottom)
                                            Subject.Position = new UnityEngine.Vector3(Subject.Position.x, Subject.Position.y - 5, Subject.Position.z);
                                    }
                                });
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
    }
}
