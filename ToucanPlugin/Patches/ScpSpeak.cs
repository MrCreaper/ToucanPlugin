/*using Assets._Scripts.Dissonance;
using HarmonyLib;
using System.Linq;

namespace ToucanPlugin.Patches
{
    [HarmonyPatch(typeof(DissonanceUserSetup), nameof(DissonanceUserSetup.CallCmdAltIsActive))]
    public class SCPSpeak
    {
        public static void Prefix(DissonanceUserSetup __instance, bool value)
        {
            var player = Exiled.API.Features.Player.Get(__instance.gameObject);
            if (string.IsNullOrEmpty(player?.UserId) || player.Team != Team.SCP) return;
            else if (ToucanPlugin.Instance.Config.AltSpeakScps.Contains(player.Role)) { __instance.MimicAs939 = value; return; }
            else if (string.IsNullOrEmpty(ServerStatic.GetPermissionsHandler()._groups.FirstOrDefault(g => g.Value == player.ReferenceHub.serverRoles.Group).Key) && !string.IsNullOrEmpty(player.ReferenceHub.serverRoles.MyText)) return;
            else /*if (player.CheckPermission($"scputils_speak.{player.Role.ToString().ToLower()}"))*//*
            {
                //__instance.IntercomAsHuman = true;
                __instance.MimicAs939 = value;
            }
        }
    }
}*/