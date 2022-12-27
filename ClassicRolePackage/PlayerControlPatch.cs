using CustomRolePackage;
using DillyzRoleApi_Rewritten;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ClassicRolePackage
{
    class PlayerControlPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        class PlayerControlPatch_FixedUpdate
        {
            public static DateTime lastTrace = DateTime.MinValue;
            public static Dictionary<byte, Vector2> oldPos = new Dictionary<byte, Vector2>();
            public static void Postfix(PlayerControl __instance)
            {
                if (!(DillyzUtil.InGame() || DillyzUtil.InFreeplay()) || __instance != PlayerControl.LocalPlayer || HarmonyMain.detectiveSearchButton.GameInstance == null)
                    return;

                string rolename = DillyzUtil.getRoleName(PlayerControl.LocalPlayer);

                foreach (FootstepBehaviour footstep in FootstepBehaviour.AllFootsteps)
                    footstep.Renderer.enabled = footstep.OutlineRenderer.enabled = (rolename == "Detective" && (HarmonyMain.classicMode || HarmonyMain.detectiveSearchButton.GameInstance.useTimerMode));

                if (rolename != "Detective")
                    return;

                TimeSpan timeLeft = DateTime.UtcNow - lastTrace;
                float timeRemaining = HarmonyMain.footPulse - (float)timeLeft.TotalMilliseconds / 1000f;

                if (timeRemaining > 0)
                    return;

                lastTrace = DateTime.UtcNow;

                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    Vector2 curpos = player.GetTruePosition();
                    if (player.inVent || player.Data.IsDead ||
                        (oldPos.ContainsKey(player.PlayerId) && InRangeOf(oldPos[player.PlayerId].x, curpos.x, 0.01f) && InRangeOf(oldPos[player.PlayerId].y, curpos.y, 0.01f)))
                        return;

                    GameObject newStep = new GameObject();
                    newStep.layer = player.gameObject.layer;// - 1;
                    newStep.name = "footstep_" + newStep.name + "_" + lastTrace.ToString();
                    FootstepBehaviour footstep = newStep.AddComponent<FootstepBehaviour>();
                    footstep.BeginFunnies(player);

                    oldPos[player.PlayerId] = curpos;
                }
            }

            public static bool InRangeOf(float x, float y, float z)
            {
                return (Math.Abs(Math.Abs(x) - Math.Abs(y)) < z);
            }
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnGameStart))]
        class PlayerControlPatch_OnGameStart
        {
            public static void Postfix(PlayerControl __instance) {
                FootstepBehaviour.AllFootsteps.Clear();
            }
        }
    }
}
