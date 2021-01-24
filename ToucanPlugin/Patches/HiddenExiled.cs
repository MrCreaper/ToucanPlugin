using HarmonyLib;

namespace ToucanPlugin
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.ReloadServerName))]
    internal static class HiddenExiled
    {
        private static void Postfix()
        {
            if (!ToucanPlugin.Instance.Config.HiddenExiled)
                Exiled.API.Features.Server.Name = Exiled.API.Features.Server.Name.Replace($"<color=#00000000><size=1>Exiled {Exiled.Loader.Loader.Version.ToString().Replace(".0", "")}</size></color>", "");
        }
    }
}