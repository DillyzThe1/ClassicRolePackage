using DillyzRoleApi_Rewritten;
using HarmonyLib;

namespace ClassicRolePackage
{
    class PlayerControlPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
        class PlayerControlPatch_Exiled
        {
            public static void Prefix(PlayerControl __instance)
            {
                if (DillyzUtil.getRoleName(__instance) == "Jester")
                    CustomRole.getByName("Jester").WinGame(__instance);
            }
        }
    }
}
