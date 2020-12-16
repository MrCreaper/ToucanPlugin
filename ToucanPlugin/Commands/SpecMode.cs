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
    public class SpecMode : ICommand
    {
        public static List<String> TUTSpecList { get; set; } = new List<String> { };
        public string Command { get; } = "specmode";

        public string[] Aliases { get; } = { "sm" };

        public string Description { get; } = "Lets you become a NOCLIPING spectator";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender PCplayer)
            {
                Player p = Player.List.ToList().Find(x => x.Sender == PCplayer);
                Exiled.API.Features.Log.Info($"{p.Role == RoleType.Spectator} {p.Role == RoleType.Tutorial} {p.Role == RoleType.Spectator || p.Role == RoleType.Tutorial}");
                if (!Round.IsStarted)
                {
                    response = $"No.";
                    return true;
                }
                if (p.Role == RoleType.Spectator || p.Role == RoleType.Tutorial && TUTSpecList.Contains(PCplayer.SenderId))
                {
                    response = $"Can only use this command while being a dead";
                    return true;
                }
                if (!TUTSpecList.Contains(PCplayer.SenderId))
                {
                    TUTSpecList.Add(PCplayer.SenderId);
                    p.SetRole(RoleType.Tutorial);
                    p.NoClipEnabled = true;
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
                    p.Inventory.Clear();
                    p.SetRole(RoleType.Spectator);
                    p.NoClipEnabled = false;
                    Player.List.ToList().Find(x => x.Id.ToString() == PCplayer.SenderId).ReferenceHub.playerEffectsController.DisableEffect<Scp268>();
                    Player.List.ToList().Find(x => x.Id.ToString() == PCplayer.SenderId).ReferenceHub.playerEffectsController.DisableEffect<Amnesia>();
                    response = $"Back to spectating.";
                    return true;
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
