using BepInEx;
using BepInEx.IL2CPP;
using ClassicRolePackage;
using DillyzRoleApi_Rewritten;
using HarmonyLib;
using System;
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

        #region detective stuff
        public static CustomButton detectiveSearchButton;
        public static bool classicMode = false;
        public static float searchDuration = 25f;
        public static bool anonSteps = false;
        public static float footDur = 15f;
        public static float footPulse = 0.25f;

        public static AudioClip searchUpClip;
        public static AudioClip searchDownClip;
        #endregion

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
            jester.blurb = "The Jester is a role with a simple mission, that being to get voted out. They work with no team and are an independent force of their own.\n\ngithub.com/DillyzThe1";
            jester.SetSprite(Assembly.GetExecutingAssembly(), "ClassicRolePackage.Assets.jester.png");
            #endregion

            #region sheriff
            Log.LogInfo("Adding a Sheriff!");
            CustomRole sheriffRole = DillyzUtil.createRole("Sheriff", "Kill suspects with caution.", true, false, new Color32(255, 185, 30, 255), false,
                                                                    CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            sheriffRole.a_or_an = "a";
            sheriffRole.blurb = "The Sheriff is a role in which may attempt to kill an impostor at will, however, killing a fellow crewmate will kill themselves!\n\ngithub.com/DillyzThe1";
            sheriffRole.SetSprite(Assembly.GetExecutingAssembly(), "ClassicRolePackage.Assets.sheriff.png");

            bool killoncrewkill = true;

            Log.LogInfo("Adding a Sheriff button!");
            CustomButton sheriffButton = DillyzUtil.addButton(Assembly.GetExecutingAssembly(), "Sheriff Kill Button", "ClassicRolePackage.Assets.sheriff_kill.png", -1f, true,
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

            sheriffRole.AddAdvancedSetting_Boolean("Known Publicly", false, delegate (bool newvalue) {
                sheriffRole.nameColorPublic = newvalue;
            });
            sheriffRole.AddAdvancedSetting_Boolean("Punished", true, delegate (bool newvalue) {
                killoncrewkill = newvalue;
            });
            sheriffRole.AddAdvancedSetting_Float("Kill Cooldown", 30, 5, 90, 5, delegate (float newvalue) {
                sheriffButton.cooldown = newvalue;
            }).suffix = "s";
            #endregion

            #region detective
            Log.LogInfo("Adding a Detective!");
            CustomRole detective = DillyzUtil.createRole("Detective", "Use evidence to trace impostors.", true, false, new Color32(110, 150, 255, 255), false,
                                                                    CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            detective.a_or_an = "a";
            detective.blurb = "The Detective is a role in which is able to follow previous footsteps back a set amount of time, caching those of whom are guilty in their path. The catch is that they can only do this with searching ON.\n\ngithub.com/DillyzThe1";
            detective.SetSprite(Assembly.GetExecutingAssembly(), "ClassicRolePackage.Assets.detective.png");

            Log.LogInfo("Adding the Detectives's search button!");
            detectiveSearchButton = DillyzUtil.addButton(Assembly.GetExecutingAssembly(), "Detective Search", "ClassicRolePackage.Assets.detective_search.png", 35f, 
                false, new string[] { "Detective" }, new string[] { }, delegate(KillButtonCustomData button, bool success) 
            {
                if (!success)
                    return;

                if (ShipStatusPatch.funnyflash == null)
                    return;

                ShipStatusPatch.funnyflash.FadeToColor(1f, new Color(1f, 1f, 1f, 0.95f), new Color(110f/255f, 150f/255f, 1f, 0.35f));
                try
                {
                    if (SoundManager.Instance != null && Minigame.Instance != null)
                        SoundManager.Instance.PlaySound(Minigame.Instance.OpenSound, false, 1f, null);
                    if (SoundManager.Instance != null && ShipStatus.Instance != null)
                        SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 0.5f, null);
                }
                catch (Exception e) {
                    HarmonyMain.Instance.Log.LogError("goober sounds failed " + e.Message + "\n" + e.StackTrace);
                }
            });
            detectiveSearchButton.buttonText = "Search";
            detectiveSearchButton.textOutlineColor = detective.roleColor;
            detectiveSearchButton.SetUseTimeButton(searchDuration, delegate (KillButtonCustomData button, bool interrupted) {

                if (interrupted) {
                    ShipStatusPatch.funnyflash?.FadeToColor(1f, new Color(1f, 1f, 1f, 0f));
                    return;
                }

                ShipStatusPatch.funnyflash?.FadeToColor(1f, new Color(1f, 1f, 1f, 0.95f), new Color(1f, 1f, 1f, 0f));
                try
                {
                    if (SoundManager.Instance != null && Minigame.Instance != null)
                        SoundManager.Instance.PlaySound(Minigame.Instance.CloseSound, false, 1f, null);
                }
                catch (Exception e)
                {
                    HarmonyMain.Instance.Log.LogError("goober sound failed " + e.Message + "\n" + e.StackTrace);
                }
            });
            detective.AddAdvancedSetting_Boolean("Classic Mode", false, delegate (bool b) {
                detectiveSearchButton.allowedRoles.Clear();

                classicMode = b;

                if (!b)
                    detectiveSearchButton.allowedRoles.Add("Detective");
            });
            detective.AddAdvancedSetting_Float("Search Cooldown", 35, 5, 75, 5, delegate (float t) { detectiveSearchButton.cooldown = t; }).suffix = "s";
            detective.AddAdvancedSetting_Float("Search Duration", 25, 5, 60, 5, delegate (float t) { searchDuration = t; detectiveSearchButton.useTime = t; }).suffix = "s";
            detective.AddAdvancedSetting_Boolean("Anonymous Footsteps", false, delegate(bool v) { anonSteps = v; });
            detective.AddAdvancedSetting_Float("Footstep Duration", 15, 1, 30, 1, delegate (float t) { footDur = t; }).suffix = "s";
            #endregion
        }
    }
}