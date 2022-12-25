using BepInEx;
using BepInEx.IL2CPP;
using DillyzRoleApi_Rewritten;
using HarmonyLib;

namespace ClassicRolePack
{
    [BepInPlugin(HarmonyMain.MOD_ID, HarmonyMain.MOD_NAME, HarmonyMain.MOD_VERSION)]
    [BepInDependency("com.github.dillyzthe1.dillyzroleapi")]
    [BepInProcess("Among Us.exe")]
    [BepInProcess("Among Us2.exe")]
    [BepInProcess("Among Us3.exe")]
    [BepInProcess("Among Us4.exe")]
    [BepInProcess("Among Us5.exe")]
    public class HarmonyMain : BasePlugin
    {
        // Replace all of this with your own custom data.
        public const string MOD_NAME = "ClassicRolePack", MOD_VERSION = "1.0.0", MOD_ID = "com.github.dillyzthe1.rolepacks.classic";
        public static Harmony harmony = new Harmony(HarmonyMain.MOD_ID);
        public static HarmonyMain Instance;

        public override void Load()
        {
            Instance = this;
            // You can change the LogInfo call as you like.
            Log.LogInfo($"{HarmonyMain.MOD_NAME} v{HarmonyMain.MOD_VERSION} loaded.");
            harmony.PatchAll();

            // You should view the docs, but here's two functions you might like:
            // DillyzUtil.createRole
            // DillyzUtil.addButton
            //
            // Please read up at https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/wiki/ for more information.
        }
    }
}