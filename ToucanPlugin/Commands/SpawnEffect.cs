using CommandSystem;
using GameCore;
using RemoteAdmin;
using System;
using Exiled.API.Features;
using System.Linq;
using Respawning;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class SpwanEffect : ICommand
    {
        public string Command { get; } = "spawneffect";

        public string[] Aliases { get; } = { "se" };

        public string Description { get; } = "Spwan some bullshit";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Sender is CommandSender PCplayer)
            {
                if (Sender.CheckPermission(PlayerPermissions.RespawnEvents))
                {
                    if (arguments.Array[1] != null)
                {
                    if (arguments.Array[1] == "car")
                    {
                        RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.ChaosInsurgency);
                        response = $"Chaos Car Incoming!";
                        return true;
                    }
                    else
                    {
                        RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.NineTailedFox);
                        response = $"MTF Helicopter Incoming!";
                        return true;
                    }
                }
                else
                {
                    response = $"Missing car or heli";
                    return false;
                }
                }
                else
                {
                    response = $"Missing Respawn Events Permission";
                    return true;
                }
            }
            else
            {
                response = $"Fuck off";
                return true;
            }
        }
    }
}
