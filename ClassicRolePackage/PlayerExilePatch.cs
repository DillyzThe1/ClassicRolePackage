using DillyzRoleApi_Rewritten;
using HarmonyLib;

namespace ClassicRolePackage
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    class PlayerExilePatch
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (DillyzUtil.getRoleName(__instance) == "Jester")
                CustomRole.getByName("Jester").WinGame(__instance);
        }
    }
}
