using CustomRolePackage;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ClassicRolePackage
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    class PlayerControlPatch
    {
        public static DateTime lastTrace = DateTime.MinValue;
        public static void Postfix(PlayerControl __instance) {
            if (__instance != PlayerControl.LocalPlayer)
                return;

            TimeSpan timeLeft = DateTime.UtcNow - lastTrace;
            float timeRemaining = HarmonyMain.footPulse - (float)timeLeft.TotalMilliseconds / 1000f;

            if (timeRemaining > 0)
                return;

            lastTrace = DateTime.UtcNow;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {

                GameObject newStep = new GameObject();
                newStep.layer = player.gameObject.layer;// - 1;
                newStep.name = "footstep_" + newStep.name + "_" + lastTrace.ToString();
                FootstepBehaviour footstep = newStep.AddComponent<FootstepBehaviour>();
                footstep.BeginFunnies(player);
            }
        }
    }
}
