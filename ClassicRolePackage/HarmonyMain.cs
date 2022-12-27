using BepInEx;
using BepInEx.IL2CPP;
using DillyzRoleApi_Rewritten;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomRolePackage
{
    [BepInPlugin(HarmonyMain.MOD_ID, HarmonyMain.MOD_NAME, HarmonyMain.MOD_VERSION)]
    [BepInDependency("com.github.dillyzthe1.dillyzroleapi")]
    public class HarmonyMain : BasePlugin
    {
        public const string MOD_NAME = "ClassicRolePackage", MOD_VERSION = "1.0.0", MOD_ID = "com.github.dillyzthe1.dillyzroleapi.packages.classic";
        public static Harmony harmony = new Harmony(HarmonyMain.MOD_ID);

        public static float searchDuration = 15f;
        public static bool anonSteps = false;
        public static float footDir = 15f;
        public static float footPulse = 0.5f;

        public static HarmonyMain Instance;

        public override void Load()
        {
            Instance = this;
            Log.LogInfo(HarmonyMain.MOD_NAME + " v" + HarmonyMain.MOD_VERSION + " loaded. Hooray!");
            harmony.PatchAll();

            #region jester
            Log.LogInfo("Adding a Jester!");
            CustomRole jester = DillyzUtil.createRole("Jester", "Get voted out to win.", true, false, new Color32(90, 50, 200, 255), false,
                                                                    CustomRoleSide.LoneWolf, VentPrivilege.None, false, true);
            jester.a_or_an = "a";
            jester.SetSprite(Assembly.GetExecutingAssembly(), "ClassicRolePackage.jester.png");
            #endregion

            #region sheriff
            Log.LogInfo("Adding a Sheriff!");
            CustomRole sheriffRole = DillyzUtil.createRole("Sheriff", "Kill the impostor or suicide.", true, true, new Color32(255, 185, 30, 255), false,
                                                                    CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            sheriffRole.a_or_an = "a";
            sheriffRole.SetSprite(Assembly.GetExecutingAssembly(), "ClassicRolePackage.sheriff.png");

            bool killoncrewkill = true;

            Log.LogInfo("Adding a Sheriff button!");
            CustomButton sheriffButton = DillyzUtil.addButton(Assembly.GetExecutingAssembly(), "Sheriff Kill Button", "ClassicRolePackage.sheriff_kill.png", -1f, true,
            new string[] { "Sheriff" }, new string[] { }, delegate (KillButtonCustomData button, bool success)
            {
                if (!success)
                    return;

                Log.LogInfo(button.killButton.currentTarget.name + " was targetted by " + PlayerControl.LocalPlayer.name + "!");

                DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, button.killButton.currentTarget);

                if (killoncrewkill && DillyzUtil.roleSide(button.killButton.currentTarget) == CustomRoleSide.Crewmate)
                    DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);

            });

            sheriffButton.buttonText = "Kill";
            sheriffButton.textOutlineColor = new Color32(255, 185, 30, 255);

            sheriffRole.AddAdvancedSetting_Boolean("Punished", true, delegate (bool newvalue) {
                killoncrewkill = newvalue;
            });
            sheriffRole.AddAdvancedSetting_Int("Kill Cooldown", 30, 5, 90, 5, delegate (int newvalue) {
                sheriffButton.cooldown = newvalue;
            });
            #endregion

            #region detective
            Log.LogInfo("Adding a Detective!");
            CustomRole detective = DillyzUtil.createRole("Detective", "Use evidence to trace impostors.", true, false, new Color32(110, 150, 255, 255), false,
                                                                    CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            detective.a_or_an = "a";
            detective.SetSprite(Assembly.GetExecutingAssembly(), "ClassicRolePackage.detective.png");

            Log.LogInfo("Adding the Detectives's search button!");
            CustomButton detectiveSearchButton = DillyzUtil.addButton(Assembly.GetExecutingAssembly(), "Search", "ClassicRolePackage.detective_search.png", 35f, false,
                        new string[] { "Detective" }, new string[] { }, delegate(KillButtonCustomData button, bool success) 
            {
                if (!success)
                    return;


                DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
            });
            detectiveSearchButton.textOutlineColor = detective.roleColor;
            detective.AddAdvancedSetting_Boolean("Classic Mode", false, delegate (bool newvalue) {
                detectiveSearchButton.allowedRoles.Clear();

                if (!newvalue)
                    detectiveSearchButton.allowedRoles.Add("Detective");
            });
            detective.AddAdvancedSetting_Int("Search Cooldown", 35, 5, 75, 5, delegate (int newvalue) { detectiveSearchButton.cooldown = newvalue; });
            detective.AddAdvancedSetting_Int("Search Duration", 15, 5, 60, 5, delegate (int newvalue) { searchDuration = newvalue; });
            detective.AddAdvancedSetting_Boolean("Anonymous Footsteps", false, delegate(bool newvalue) { anonSteps = newvalue; });
            detective.AddAdvancedSetting_Int("Footstep Duration", 15, 1, 30, 1, delegate (int newvalue) { footDir = newvalue; });
            #endregion
        }
    }
}