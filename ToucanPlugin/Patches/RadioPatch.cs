using HarmonyLib;

namespace ToucanPlugin.Patches
{
    [HarmonyPatch(typeof(Radio))]
    [HarmonyPatch(nameof(Radio.UseBattery))]
    internal static class RadioPatch
    {
        static bool Prefix(Radio __instance)
        {
            if (__instance.ccm.CurRole.team == Team.MTF)
            {
                __instance.name = "RADIO-SPAMMER 9000";
                RadioDisplay.label = "CUM";
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}