using HarmonyLib;

namespace ClassicRolePackage
{
    class HudOverrideSystemTypePatch
    {
        public static bool isActive = false;
        [HarmonyPatch(typeof(HudOverrideSystemType), nameof(HudOverrideSystemType.IsActive), MethodType.Getter)]
        class HudOverrideSystemTypePatch_IsActive_get {
            public static void Postfix(HudOverrideSystemType __instance, ref bool __result) {
                isActive = __result;
            }
        }
    }
}
