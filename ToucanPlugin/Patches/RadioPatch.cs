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
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}