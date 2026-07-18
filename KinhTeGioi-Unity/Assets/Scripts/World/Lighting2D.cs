using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace KTG
{
    // Ho tro anh sang 2D cho HD-2D (Phase B): material Sprite-Lit chia se +
    // ham tao Light2D bang code — giu triet ly 100% procedural cua game.
    public static class Lighting2D
    {
        static Material litMat;

        // Material Sprite-Lit dung chung cho world sprites (tile/prop/nhan vat).
        // UI va cac sprite tu phat sang (Glow, dom dom, spark) giu unlit.
        public static Material LitMaterial
        {
            get
            {
                if (litMat == null)
                    litMat = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
                return litMat;
            }
        }

        public static void MakeLit(SpriteRenderer sr) => sr.sharedMaterial = LitMaterial;

        // Anh sang nen toan map — mau/cuong do doc tu GameContent (canh TorchColor).
        public static Light2D AddGlobal(Transform root, int mapIndex)
        {
            var go = new GameObject("GlobalLight2D");
            go.transform.SetParent(root, false);
            var l = go.AddComponent<Light2D>();
            l.lightType = Light2D.LightType.Global;
            l.color = GameContent.MapAmbientColor(mapIndex);
            l.intensity = GameContent.MapAmbientIntensity(mapIndex);
            return l;
        }

        // Diem sang cho nguon sang cu the (duoc, crystal, cua so nha, portal...).
        public static Light2D AddPoint(Transform parent, Vector3 localPos, Color color, float radius, float intensity)
        {
            var go = new GameObject("Light2D");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            var l = go.AddComponent<Light2D>();
            l.lightType = Light2D.LightType.Point;
            l.color = color;
            l.intensity = intensity;
            l.pointLightInnerRadius = radius * 0.25f;
            l.pointLightOuterRadius = radius;
            l.falloffIntensity = 0.6f;
            return l;
        }
    }
}
