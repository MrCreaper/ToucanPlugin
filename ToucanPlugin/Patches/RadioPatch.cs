using HarmonyLib;

/*namespace ToucanPlugin.Patches
{
    [HarmonyPatch(typeof(Radio))]
    [HarmonyPatch(nameof(Radio.Us))]
    internal static class RadioPatch
    {
        static bool Prefix(Radio __Singleton)
        {
            if (__Singleton.ccm.CurRole.team == Team.MTF)
            {
                __Singleton.name = "RADIO-SPAMMER 9000";
                RadioDisplay.label = "CUM";
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}*/