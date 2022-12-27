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
    [HarmonyPatch(typeof(DillyzUtil), nameof(DillyzUtil.commitAssassination))]
    class DillyzUtilPatch
    {
        public static void Prefix(DillyzUtil __instance, PlayerControl assassinator, PlayerControl target) { 
            if (target == PlayerControl.LocalPlayer)
                ShipStatusPatch.funnyflash?.FadeToColor(1f, new Color(1f, 1f, 1f, 0f));
        }
    }
}
