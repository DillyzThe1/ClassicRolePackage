﻿using CustomRolePackage;
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
        public static Dictionary<byte, Vector2> oldPos = new Dictionary<byte, Vector2>();
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
                Vector2 curpos = player.GetTruePosition();
                if (player.inVent || (oldPos.ContainsKey(player.PlayerId) && oldPos[player.PlayerId].x == curpos.x && oldPos[player.PlayerId].y == curpos.y))
                    return;

                GameObject newStep = new GameObject();
                newStep.layer = player.gameObject.layer;// - 1;
                newStep.name = "footstep_" + newStep.name + "_" + lastTrace.ToString();
                FootstepBehaviour footstep = newStep.AddComponent<FootstepBehaviour>();
                footstep.BeginFunnies(player);

                oldPos[player.PlayerId] = curpos;
            }
        }
    }
}