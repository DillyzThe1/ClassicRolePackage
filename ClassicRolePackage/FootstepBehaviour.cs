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

namespace ClassicRolePackage
{
    [Il2CppItem]
    class FootstepBehaviour : MonoBehaviour
    {
        SpriteRenderer sprrend;
        Color ogColor;
        Color fadedColor;

        public void Start()
        {
        }

        public void BeginFunnies(PlayerControl player) {
            ogColor = new Color(1f, 0f, 0f, 1f); //player.cosmetics.normalBodySprite.BodySprite.color;
            fadedColor = new Color(ogColor.r, ogColor.g, ogColor.b, 0f);
            sprrend = this.gameObject.AddComponent<SpriteRenderer>();
            sprrend.sprite = GetFootStepSprite();
            sprrend.color = ogColor;

            this.transform.position = player.transform.position - new Vector3(0f, 0.25f, 0f);

            MonoBehaviourExtensions.StartCoroutine(this, FadeOut());
        }

        IEnumerator FadeOut() {
            for (float t = 0f; t < HarmonyMain.footDir; t += Time.deltaTime)
            {
                sprrend.color = Color.Lerp(ogColor, fadedColor, t / HarmonyMain.footDir);
                yield return null;
            }
            GameObject.Destroy(this);
            yield break;
        }

        public static Sprite stepSprite;
        public static Sprite GetFootStepSprite() { 
            if (stepSprite == null)
                stepSprite = DillyzUtil.getSprite(Assembly.GetExecutingAssembly(), "ClassicRolePackage.step.png");
            return stepSprite;
        }
    }
}
