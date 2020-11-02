using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;
using Exiled.API.Features;
using System.Linq;
using NPCS;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class TempPet : ICommand
    {
        public string Command { get; } = "temppet";

        public string[] Aliases { get; } = { "tp" };

        public string Description { get; } = "Temp Pet";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender CCplayer)
            {
                Player petOwner = Player.List.ToList().Find(x => x.UserId == CCplayer.SenderId);
                Npc pet = NPCS.Methods.CreateNPC(new UnityEngine.Vector3(0f, 0f, 0f), new UnityEngine.Vector2(0f, 0f), new UnityEngine.Vector3(1f, 1f, 1f), RoleType.Scp173, ItemType.GunLogicer, "N U T");
                pet.Follow(petOwner);
                response = $"hello {CCplayer.Nickname}";
                return false;
            }
            else
            {
                response = $"Fuck You";
                return true;
            }
        }
    }
}
