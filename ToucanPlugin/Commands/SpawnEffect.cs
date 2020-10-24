using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;
using Exiled.API.Features;
using System.Linq;
using Respawning;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class SpwanEffect : ICommand
    {
        public string Command { get; } = "spawneffect";

        public string[] Aliases { get; } = { "se" };

        public string Description { get; } = "Spwan some bullshit";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender PCplayer)
            {
                RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.ChaosInsurgency);
                response = $"Spawning chaos carrr";
                return false;
            }
            else
            {
                response = $"Fuck off";
                return true;
            }
        }
    }
}
