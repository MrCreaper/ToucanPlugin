using CommandSystem;
using CustomPlayerEffects;
using Exiled.API.Features;
using GameCore;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class SpecMode : ICommand
    {
        public static List<String> TUTSpecList { get; set; } = new List<String> { };
        public string Command { get; } = "specmode";

        public string[] Aliases { get; } = { "sm" };

        public string Description { get; } = "Lets you become a NOCLIPING spectator";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is PlayerCommandSender PCplayer)
            {
                if (PCplayer.CCM.CurClass != RoleType.Spectator || PCplayer.CCM.CurClass != RoleType.Tutorial)
                {
                    response = $"Can only use this command while being a dead";
                    return true;
                }
                else
                {
                    if (!TUTSpecList.Contains(PCplayer.SenderId))
                    {
                        TUTSpecList.Add(PCplayer.SenderId);
                        PCplayer.CCM.SetClassID(RoleType.Tutorial);
                        PCplayer.CCM.NoclipEnabled = true;
                        for (int i = 0; i <= 8; i++)
                            Player.List.ToList().Find(x => x.Id.ToString() == PCplayer.SenderId).Inventory.AddNewItem(ItemType.Coin);
                        Player.List.ToList().Find(x => x.Id.ToString() == PCplayer.SenderId).ReferenceHub.playerEffectsController.EnableEffect<Scp268>();
                        Player.List.ToList().Find(x => x.Id.ToString() == PCplayer.SenderId).ReferenceHub.playerEffectsController.EnableEffect<Amnesia>();
                        response = $"Enjoy flying around!";
                        return true;
                    }
                    else
                    {
                        TUTSpecList.Remove(PCplayer.SenderId);
                        PCplayer.CCM.SetClassID(RoleType.Spectator);
                        PCplayer.CCM.NoclipEnabled = false;
                        Player.List.ToList().Find(x => x.Id.ToString() == PCplayer.SenderId).ReferenceHub.playerEffectsController.DisableEffect<Scp268>();
                        Player.List.ToList().Find(x => x.Id.ToString() == PCplayer.SenderId).ReferenceHub.playerEffectsController.DisableEffect<Amnesia>();
                        response = $"Back to spectating.";
                        return true;
                    }
                }
            }
            else
            {
                response = $"Shit fucked up";
                return true;
            }
        }
    }
}
