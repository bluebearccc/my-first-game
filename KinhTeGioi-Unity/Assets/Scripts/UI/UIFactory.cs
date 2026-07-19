using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace KTG
{
    // Ham dung chung de dung Canvas/Panel/Button/Text bang code, dung sprite tu PixelArt.
    public static class UIFactory
    {
        static Font cachedFont;

        public static Font GetFont()
        {
            if (cachedFont != null) return cachedFont;
            cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (cachedFont == null) cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return cachedFont;
        }

        public static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }

        public static Canvas CreateCanvas(string name, int sortOrder = 0)
        {
            EnsureEventSystem();
            var go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            canvas.pixelPerfect = true; // ep UI ve luoi pixel man hinh cho chu va vien sac net
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 0.5f;
            // Raster glyph Text va hinh 9-slice Wood9 o mat do gap doi -> chu/vien UI sac net hon,
            // dac biet khi cua so chay gan do phan giai tham chieu 1280x720. Chi phi ~0.
            scaler.dynamicPixelsPerUnit = 2f;
            return canvas;
        }

        public static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public static void SetAnchor(RectTransform rt, Vector2 anchorMinMax, Vector2 pivot, Vector2 pos)
        {
            rt.anchorMin = anchorMinMax;
            rt.anchorMax = anchorMinMax;
            rt.pivot = pivot;
            rt.anchoredPosition = pos;
        }

        // Moi panel dung chung khung go vien vang (phong cach RPG); fill/border giu lai
        // trong chu ky de tuong thich cho code cu, khong dung nua.
        public static Image CreatePanel(Transform parent, string name, Vector2 size, Color fill, Color border)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var img = go.GetComponent<Image>();
            img.rectTransform.SetParent(parent, false);
            img.rectTransform.sizeDelta = size;
            img.sprite = PixelArt.Wood9();
            img.type = Image.Type.Sliced;
            return img;
        }

        public static Image CreateImage(Transform parent, string name, Sprite sprite, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var img = go.GetComponent<Image>();
            img.rectTransform.SetParent(parent, false);
            img.rectTransform.sizeDelta = size;
            img.sprite = sprite;
            img.preserveAspect = false;
            return img;
        }

        public static Text CreateText(Transform parent, string name, string content, int fontSize, Color color, TextAnchor anchor, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            var txt = go.GetComponent<Text>();
            txt.rectTransform.SetParent(parent, false);
            txt.rectTransform.sizeDelta = size;
            txt.font = GetFont();
            txt.fontSize = fontSize;
            txt.color = color;
            txt.text = content;
            txt.alignment = anchor;
            txt.horizontalOverflow = HorizontalWrapMode.Wrap;
            txt.verticalOverflow = VerticalWrapMode.Overflow;
            return txt;
        }

        public static Button CreateButton(Transform parent, string name, string label, Vector2 size, System.Action onClick, int fontSize = 20)
        {
            var img = CreatePanel(parent, name, size, new Color(0.14f, 0.16f, 0.26f, 0.95f), new Color(0.85f, 0.7f, 0.3f, 1f));
            var btn = img.gameObject.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = new Color(0.92f, 0.92f, 0.92f, 1f);
            colors.highlightedColor = new Color(1.0f, 0.98f, 0.85f, 1f); // sang len anh vang khi hover
            colors.pressedColor = new Color(0.68f, 0.64f, 0.58f, 1f);
            btn.colors = colors;

            var txt = CreateText(img.transform, "Label", label, fontSize, new Color(0.95f, 0.92f, 0.82f), TextAnchor.MiddleCenter, size);
            Stretch(txt.rectTransform);
            // Chua le trong: chu khong cham vien go, nam gon trong o va de doc hon
            txt.rectTransform.offsetMin = new Vector2(14, 4);
            txt.rectTransform.offsetMax = new Vector2(-14, -4);
            txt.raycastTarget = false;

            if (onClick != null) btn.onClick.AddListener(() => onClick());
            return btn;
        }
    }
}
