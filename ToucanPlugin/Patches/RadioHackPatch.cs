﻿using HarmonyLib;
using ToucanPlugin.Commands;

namespace ToucanPlugin.Patches
{
    [HarmonyPatch(typeof(Radio))]
    [HarmonyPatch(nameof(Radio.Start))]
    internal static class RadioHackPatch
    {
        static bool Prefix(Radio __instance)
        {
            if (HackRadio.radioHacked)
            {
                return false;
            }
            else
            {
                RadioDisplay.label = "Lol u been haxxx";
                return true;
            }
        }
    }
}