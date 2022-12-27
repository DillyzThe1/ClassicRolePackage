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
        public static FlashOverlay funnyflash;
        public static GameObject doormat;
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnEnable))]
        class ShipStatusPatch_OnEnable
        {
            public static void Postfix(ShipStatus __instance) {
                doormat = new GameObject();
                doormat.name = "doormat";
                doormat.transform.parent = __instance.transform;
                doormat.transform.position = Vector3.zero;

                GameObject funnyflashh = new GameObject();
                funnyflashh.transform.parent = HudManager.Instance.transform;
                funnyflashh.name = "doormat";
                funnyflashh.layer = LayerMask.NameToLayer("UICollide");
                funnyflashh.transform.position = new Vector3(0f, 0.25f, 0f);
                SpriteRenderer flashRend = funnyflashh.AddComponent<SpriteRenderer>();
                flashRend.sprite = DillyzUtil.getSprite(System.Reflection.Assembly.GetExecutingAssembly(), "ClassicRolePackage.Assets.flash.png");
                flashRend.color = new Color(1f, 1f, 1f, 0f);
                funnyflash = funnyflashh.AddComponent<FlashOverlay>();
                funnyflash.sprrend = flashRend;
            }
        }
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
        class ShipStatusPatch_OnDestroy
        {
            public static void Prefix(ShipStatus __instance)
            {
                foreach (FootstepBehaviour footstep in FootstepBehaviour.AllFootsteps)
                    GameObject.Destroy(footstep);
                FootstepBehaviour.AllFootsteps.Clear();

                GameObject.Destroy(doormat);
                GameObject.Destroy(funnyflash);
                funnyflash = null;
            }
        }
    }
}
