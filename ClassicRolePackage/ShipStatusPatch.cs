﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ClassicRolePackage
{
    class ShipStatusPatch
    {
        public static GameObject doormat;
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnEnable))]
        class ShipStatusPatch_OnEnable
        {
            public static void Postfix(ShipStatus __instance) {
                doormat = new GameObject();
                doormat.name = "doormat";
                doormat.transform.parent = __instance.transform;
                doormat.transform.position = Vector3.zero;
            }
        }
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
        class ShipStatusPatch_OnDestroy
        {
            public static void Prefix(ShipStatus __instance)
            {
                GameObject.Destroy(doormat);
            }
        }
    }
}
