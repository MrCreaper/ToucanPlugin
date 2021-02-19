/*using Assets._Scripts.Dissonance;
using HarmonyLib;
using System.Linq;

namespace ToucanPlugin.Patches
{
    [HarmonyPatch(typeof(DissonanceUserSetup), nameof(DissonanceUserSetup.CallCmdAltIsActive))]
    public class SCPSpeak
    {
        public static void Prefix(DissonanceUserSetup __Singleton, bool value)
        {
            var player = Exiled.API.Features.Player.Get(__Singleton.gameObject);
            if (string.IsNullOrEmpty(player?.UserId) || player.Team != Team.SCP) return;
            else if (ToucanPlugin.Singleton.Config.AltSpeakScps.Contains(player.Role)) { __Singleton.MimicAs939 = value; return; }
            else if (string.IsNullOrEmpty(ServerStatic.GetPermissionsHandler()._groups.FirstOrDefault(g => g.Value == player.ReferenceHub.serverRoles.Group).Key) && !string.IsNullOrEmpty(player.ReferenceHub.serverRoles.MyText)) return;
            else /*if (player.CheckPermission($"scputils_speak.{player.Role.ToString().ToLower()}"))*//*
            {
                //__Singleton.IntercomAsHuman = true;
                __Singleton.MimicAs939 = value;
            }
        }
    }
}*/