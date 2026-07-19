using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace KTG
{
    // Phase D1: bong do dong tren nen diorama D3, dung ShadowCaster2D cua URP 2D.
    public static class Shadow2D
    {
        // Cong tac tat nhanh toan bo bong dong neu FPS tut hoac bong bi loi.
        public const bool Enabled = true;

        // Gan ShadowCaster2D cho go qua 1 GO con "ShadowShape". ShadowCaster2D.shapePath
        // chi doc nhung tu khoi tao hinh vuong don vi khi rong, nen chinh co bong qua localScale.
        public static void AddCaster(GameObject go, float width, float height)
        {
            if (!Enabled || go == null) return;

            var shapeGO = new GameObject("ShadowShape");
            shapeGO.transform.SetParent(go.transform, false);
            shapeGO.transform.localScale = new Vector3(width, height, 1f);

            var caster = shapeGO.AddComponent<ShadowCaster2D>();
            caster.selfShadows = false;
        }
    }
}
