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
    class ShipStatusPatch
    {
        public static GameObject funnyflash;
        public static SpriteRenderer flashRend;
        public static GameObject doormat;
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnEnable))]
        class ShipStatusPatch_OnEnable
        {
            public static void Postfix(ShipStatus __instance) {
                doormat = new GameObject();
                doormat.name = "doormat";
                doormat.transform.parent = __instance.transform;
                doormat.transform.position = Vector3.zero;

                funnyflash = new GameObject();
                funnyflash.transform.parent = HudManager.Instance.transform;
                funnyflash.name = "doormat";
                funnyflash.layer = LayerMask.NameToLayer("UICollide");
                funnyflash.transform.position = Vector3.zero;
                flashRend = funnyflash.AddComponent<SpriteRenderer>();
                flashRend.sprite = DillyzUtil.getSprite(System.Reflection.Assembly.GetExecutingAssembly(), "ClassicRolePackage.flash.png");
                flashRend.color = new Color(1f, 1f, 1f, 0.5f);
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
