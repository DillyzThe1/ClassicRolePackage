using DillyzRoleApi_Rewritten;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BepInEx.IL2CPP.Utils;
using CustomRolePackage;

namespace ClassicRolePackage
{
    [Il2CppItem]
    class FootstepBehaviour : MonoBehaviour
    {
        public static List<FootstepBehaviour> AllFootsteps = new List<FootstepBehaviour>();
        SpriteRenderer sprrend;

        public SpriteRenderer Renderer => sprrend;

        Color ogColor;
        Color fadedColor;

        public void Start()
        {
        }

        public void BeginFunnies(PlayerControl player) {
            int colorid = (HarmonyMain.anonSteps || HudOverrideSystemTypePatch.isActive) ? 15 : player.cosmetics.bodyMatProperties.ColorId;
            // Palette.PlayerColors[colorid];

            ogColor = new Color(0.9f, 0.9f, 0.9f, 0.65f);
            fadedColor = new Color(ogColor.r, ogColor.g, ogColor.b, 0f);


            sprrend = this.gameObject.AddComponent<SpriteRenderer>();
            sprrend.sprite = GetFootStepSprite();
            sprrend.color = ogColor;

            sprrend.material = new Material(Shader.Find("Unlit/PlayerShader"));
            sprrend.material.SetColor(PlayerMaterial.BackColor, Palette.ShadowColors[colorid]);
            sprrend.material.SetColor(PlayerMaterial.BodyColor, Palette.PlayerColors[colorid]);
            sprrend.material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);

            if (ShipStatusPatch.doormat != null)
                this.transform.parent = ShipStatusPatch.doormat.transform;

            sprrend.enabled = false;

            this.transform.position = player.transform.position - new Vector3(0f, 0.25f, 0f);
            this.transform.position += new Vector3(0f, 0f, 1f);
            AllFootsteps.Add(this);

            MonoBehaviourExtensions.StartCoroutine(this, FadeOut());
        }

        IEnumerator FadeOut() {
            for (float t = 0f; t < HarmonyMain.footDur; t += Time.deltaTime)
            {
                sprrend.color = Color.Lerp(ogColor, fadedColor, t / HarmonyMain.footDur);
                yield return null;
            }
            AllFootsteps.Remove(this);
            GameObject.Destroy(this);
            yield break;
        }

        public static Sprite stepSprite;
        public static Sprite GetFootStepSprite() { 
            if (stepSprite == null)
                stepSprite = DillyzUtil.getSprite(Assembly.GetExecutingAssembly(), "ClassicRolePackage.Assets.step.png");
            return stepSprite;
        }
    }
}
