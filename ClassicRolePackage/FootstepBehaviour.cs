using DillyzRoleApi_Rewritten;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx.IL2CPP.Utils;
using CustomRolePackage;
using UnityEngine.UI;

namespace ClassicRolePackage
{
    [Il2CppItem]
    class FootstepBehaviour : MonoBehaviour
    {
        public static List<FootstepBehaviour> AllFootsteps = new List<FootstepBehaviour>();
        SpriteRenderer sprrend;
        GameObject outline;
        SpriteRenderer sprrend_outline;

        public SpriteRenderer Renderer => sprrend;
        public SpriteRenderer OutlineRenderer => sprrend_outline;

        Color ogColor;
        Color fadedColor;

        Color ogColor_outline;
        Color fadedColor_outline;

        public void Start()
        {
        }

        public void BeginFunnies(PlayerControl player) {
            int colorid = (HarmonyMain.anonSteps || HudOverrideSystemTypePatch.isActive) ? 15 : player.cosmetics.bodyMatProperties.ColorId;

            ogColor = Palette.PlayerColors[colorid] - new Color(0.1f, 0.1f, 0.1f, 0.45f);
            fadedColor = new Color(ogColor.r, ogColor.g, ogColor.b, 0f);
            sprrend = this.gameObject.AddComponent<SpriteRenderer>();
            sprrend.sprite = GetFootStepSprite();
            sprrend.color = ogColor;

            if (ShipStatusPatch.doormat != null)
                this.transform.parent = ShipStatusPatch.doormat.transform;

            outline = new GameObject();
            outline.name = this.name + "_outline";
            outline.transform.parent = this.transform;
            ogColor_outline = Palette.ShadowColors[colorid] - new Color(0.35f, 0.35f, 0.35f, 0.45f);
            fadedColor_outline = new Color(ogColor_outline.r, ogColor_outline.g, ogColor_outline.b, 0f);
            sprrend_outline = outline.gameObject.AddComponent<SpriteRenderer>();
            sprrend_outline.sprite = GetFootStepOutlineSprite();
            sprrend_outline.color = ogColor_outline;

            outline.transform.position = this.transform.position = player.transform.position - new Vector3(0f, 0.25f, 0f);
            this.transform.position += new Vector3(0f, 0f, 1f);
            AllFootsteps.Add(this);

            MonoBehaviourExtensions.StartCoroutine(this, FadeOut());
        }

        IEnumerator FadeOut() {
            for (float t = 0f; t < HarmonyMain.footDur; t += Time.deltaTime)
            {
                sprrend.color = Color.Lerp(ogColor, fadedColor, t / HarmonyMain.footDur);
                sprrend_outline.color = Color.Lerp(ogColor_outline, fadedColor_outline, t / HarmonyMain.footDur);
                yield return null;
            }
            AllFootsteps.Remove(this);
            GameObject.Destroy(outline);
            GameObject.Destroy(this);
            yield break;
        }

        public static Sprite stepSprite;
        public static Sprite GetFootStepSprite() { 
            if (stepSprite == null)
                stepSprite = DillyzUtil.getSprite(Assembly.GetExecutingAssembly(), "ClassicRolePackage.Assets.step.png");
            return stepSprite;
        }

        public static Sprite stepSprite_Outline;
        public static Sprite GetFootStepOutlineSprite()
        {
            if (stepSprite_Outline == null)
                stepSprite_Outline = DillyzUtil.getSprite(Assembly.GetExecutingAssembly(), "ClassicRolePackage.Assets.step_outline.png");
            return stepSprite_Outline;
        }
    }
}
