using System.Collections.Generic;
using UnityEngine;

namespace KTG
{
    // Sinh toan bo sprite pixel-art bang code — khong can asset ngoai.
    public static class PixelArt
    {
        static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

        static Texture2D NewTex(int w, int h)
        {
            var t = new Texture2D(w, h, TextureFormat.RGBA32, false);
            t.filterMode = FilterMode.Point;
            t.wrapMode = TextureWrapMode.Clamp;
            var px = new Color32[w * h];
            for (int i = 0; i < px.Length; i++) px[i] = new Color32(0, 0, 0, 0);
            t.SetPixels32(px);
            return t;
        }

        // y tinh tu TREN xuong cho de ve
        static void P(Texture2D t, int x, int y, Color c)
        {
            if (x < 0 || y < 0 || x >= t.width || y >= t.height) return;
            t.SetPixel(x, t.height - 1 - y, c);
        }

        static void Rect(Texture2D t, int x, int y, int w, int h, Color c)
        {
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    P(t, x + i, y + j, c);
        }

        // Vien 1px quanh hinh: pixel trong suot canh pixel dac -> to mau vien.
        // Giup nhan vat/NPC noi bat khoi nen.
        static void OutlineTex(Texture2D t, Color oc)
        {
            int w = t.width, h = t.height;
            var src = t.GetPixels();
            var dst = (Color[])src.Clone();
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int i = y * w + x;
                    if (src[i].a > 0.01f) continue;
                    bool solid =
                        (x > 0 && src[i - 1].a > 0.5f) || (x < w - 1 && src[i + 1].a > 0.5f) ||
                        (y > 0 && src[i - w].a > 0.5f) || (y < h - 1 && src[i + w].a > 0.5f);
                    if (solid) dst[i] = oc;
                }
            t.SetPixels(dst);
        }

        static Sprite Make(Texture2D t, float ppu, Vector2 pivot)
        {
            t.Apply();
            return Sprite.Create(t, new UnityEngine.Rect(0, 0, t.width, t.height), pivot, ppu, 0,
                SpriteMeshType.FullRect);
        }

        static Sprite Cached(string key, System.Func<Sprite> gen)
        {
            if (cache.TryGetValue(key, out var s) && s != null) return s;
            s = gen();
            cache[key] = s;
            return s;
        }

        public static Sprite Solid(Color c, int size = 16)
        {
            return Cached("solid" + c + size, () =>
            {
                var t = NewTex(size, size);
                Rect(t, 0, 0, size, size, c);
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // O co / dat co dom mau
        public static Sprite Ground(Color baseC, Color speck, int seed)
        {
            return Cached("gr" + baseC + speck + seed, () =>
            {
                var t = NewTex(16, 16);
                Rect(t, 0, 0, 16, 16, baseC);
                var r = new System.Random(seed);
                for (int i = 0; i < 9; i++)
                    P(t, r.Next(16), r.Next(16), Color.Lerp(baseC, speck, 0.8f));
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        public static Sprite Water(Color deep, Color lite, int seed)
        {
            return Cached("wa" + deep + lite + seed, () =>
            {
                var t = NewTex(16, 16);
                Rect(t, 0, 0, 16, 16, deep);
                var r = new System.Random(seed * 7 + 3);
                for (int i = 0; i < 3; i++)
                {
                    int y = r.Next(14);
                    int x = r.Next(10);
                    Rect(t, x, y, 3 + r.Next(4), 1, Color.Lerp(deep, lite, 0.65f));
                }
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        public static Sprite Wall(Color baseC, Color line)
        {
            return Cached("wl" + baseC + line, () =>
            {
                var t = NewTex(16, 16);
                Rect(t, 0, 0, 16, 16, baseC);
                // van gach
                for (int y = 0; y < 16; y += 4)
                {
                    Rect(t, 0, y, 16, 1, line);
                    int off = (y / 4) % 2 == 0 ? 4 : 10;
                    Rect(t, off, y, 1, 4, line);
                }
                Rect(t, 0, 0, 16, 1, Color.Lerp(baseC, Color.white, 0.25f));
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Cay / san ho lon (16x24, pivot day)
        public static Sprite Tree(Color leaf, Color trunk)
        {
            return Cached("tr" + leaf + trunk, () =>
            {
                var t = NewTex(16, 24);
                Rect(t, 7, 14, 3, 10, trunk);
                var dark = Color.Lerp(leaf, Color.black, 0.3f);
                var lite = Color.Lerp(leaf, Color.white, 0.25f);
                // tan la hinh tron thô
                for (int y = 0; y < 16; y++)
                    for (int x = 0; x < 16; x++)
                    {
                        float dx = x - 7.5f, dy = y - 7.5f;
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        if (d < 7.5f)
                            P(t, x, y, d < 5f ? leaf : (dy > 2 ? dark : leaf));
                        if (d < 3.5f && dy < 0 && dx < 0) P(t, x, y, lite);
                    }
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Duoc phat sang (cot + ngon lua) 16x24 pivot day
        public static Sprite Torch(Color flame)
        {
            return Cached("to" + flame, () =>
            {
                var t = NewTex(16, 24);
                var wood = new Color32(110, 84, 60, 255);
                var metal = new Color32(170, 175, 190, 255);
                Rect(t, 7, 10, 2, 14, wood);
                Rect(t, 5, 9, 6, 2, metal);
                var core = Color.Lerp(flame, Color.white, 0.6f);
                // ngon lua
                Rect(t, 6, 4, 4, 5, flame);
                Rect(t, 7, 2, 2, 3, flame);
                Rect(t, 7, 5, 2, 3, core);
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Quang sang mem (glow) — dung cho duoc, portal, menu
        public static Sprite Glow(Color c, int size = 64)
        {
            return Cached("glow" + c + size, () =>
            {
                var t = NewTex(size, size);
                float half = size / 2f;
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x, y), new Vector2(half, half)) / half;
                        float a = Mathf.Clamp01(1f - d);
                        a = a * a;
                        t.SetPixel(x, y, new Color(c.r, c.g, c.b, a * c.a));
                    }
                t.filterMode = FilterMode.Bilinear;
                t.Apply();
                return Sprite.Create(t, new UnityEngine.Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16);
            });
        }

        // Vignette toi 4 goc man hinh
        public static Sprite Vignette(int size = 256)
        {
            return Cached("vig" + size, () =>
            {
                var t = NewTex(size, size);
                float half = size / 2f;
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x, y), new Vector2(half, half)) / half;
                        float a = Mathf.Clamp01(d - 0.82f) * 0.45f; // rat nhe, chi phot toi 4 goc
                        t.SetPixel(x, y, new Color(0f, 0f, 0.05f, Mathf.Clamp01(a)));
                    }
                t.filterMode = FilterMode.Bilinear;
                t.Apply();
                return Sprite.Create(t, new UnityEngine.Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16);
            });
        }

        // Nhan vat 16x24, pivot day.
        // facing: 0 = nhin xuong, 1 = nhin len (thay gay), 2 = nhin ngang (mac dinh phai, flipX de quay trai)
        // frame:  0 = dung yen, 1/2 = hai buoc di bo (chan + tay so le), 3 = tho nhe (dau nhun 1px)
        public static Sprite Character(Color hair, Color skin, Color shirt, Color pants, int facing = 0, int frame = 0)
        {
            return Cached("ch" + hair + skin + shirt + pants + "_" + facing + "_" + frame, () =>
            {
                var t = NewTex(16, 24);
                var outline = new Color32(25, 22, 34, 255);
                var blush = Color.Lerp(skin, Color.red, 0.35f);
                var shirtD = Color.Lerp(shirt, Color.black, 0.25f);
                int hy = frame == 3 ? 1 : 0; // dau nhun xuong khi tho

                // ---- dau + toc ----
                Rect(t, 4, 1 + hy, 8, 4, hair);
                Rect(t, 3, 2 + hy, 10, 3, hair);
                Rect(t, 4, 5 + hy, 8, 5, skin);
                Rect(t, 3, 5 + hy, 1, 3, hair); Rect(t, 12, 5 + hy, 1, 3, hair);
                if (facing == 0)
                {
                    P(t, 6, 6 + hy, outline); P(t, 9, 6 + hy, outline);
                    P(t, 7, 8 + hy, blush); P(t, 8, 8 + hy, blush);
                }
                else if (facing == 2)
                {
                    Rect(t, 3, 4 + hy, 5, 4, hair); // toc phu gay phia sau
                    P(t, 10, 6 + hy, outline);
                    P(t, 10, 8 + hy, blush);
                }
                else
                {
                    Rect(t, 4, 5 + hy, 8, 4, hair); // nhin len: toc phu het gay
                }

                // ---- than + tay (tay vung so le voi chan) ----
                Rect(t, 4, 10, 8, 7, shirt);
                Rect(t, 7, 12, 2, 3, shirtD);
                if (frame == 1)
                {
                    Rect(t, 3, 10, 1, 4, shirt); Rect(t, 3, 14, 1, 1, skin);   // tay trai vung truoc
                    Rect(t, 12, 12, 1, 4, shirt); Rect(t, 12, 16, 1, 1, skin); // tay phai vung sau
                }
                else if (frame == 2)
                {
                    Rect(t, 12, 10, 1, 4, shirt); Rect(t, 12, 14, 1, 1, skin);
                    Rect(t, 3, 12, 1, 4, shirt); Rect(t, 3, 16, 1, 1, skin);
                }
                else
                {
                    Rect(t, 3, 11, 1, 4, shirt); Rect(t, 12, 11, 1, 4, shirt);
                    Rect(t, 3, 15, 1, 1, skin); Rect(t, 12, 15, 1, 1, skin);
                }

                // ---- chan + giay ----
                if (frame == 1)
                {
                    Rect(t, 4, 17, 3, 3, pants); Rect(t, 9, 17, 3, 4, pants);  // chan trai buoc len
                    Rect(t, 4, 20, 3, 2, outline); Rect(t, 9, 21, 3, 2, outline);
                }
                else if (frame == 2)
                {
                    Rect(t, 4, 17, 3, 4, pants); Rect(t, 9, 17, 3, 3, pants);
                    Rect(t, 4, 21, 3, 2, outline); Rect(t, 9, 20, 3, 2, outline);
                }
                else
                {
                    Rect(t, 4, 17, 8, 4, pants);
                    Rect(t, 7, 18, 2, 3, outline);
                    Rect(t, 4, 21, 3, 2, outline); Rect(t, 9, 21, 3, 2, outline);
                }
                OutlineTex(t, new Color(0.08f, 0.06f, 0.11f)); // vien 1px cho noi bat
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Chan dung hoi thoai 48x48
        public static Sprite Portrait(Color hair, Color skin, Color shirt)
        {
            return Cached("po" + hair + skin + shirt, () =>
            {
                var t = NewTex(48, 48);
                var outline = new Color32(25, 22, 34, 255);
                // vai ao
                Rect(t, 6, 38, 36, 10, shirt);
                Rect(t, 6, 38, 36, 2, Color.Lerp(shirt, Color.white, 0.2f));
                // co + mat
                Rect(t, 20, 33, 8, 6, skin);
                Rect(t, 12, 10, 24, 26, skin);
                // toc
                Rect(t, 10, 4, 28, 10, hair);
                Rect(t, 9, 8, 5, 16, hair);
                Rect(t, 34, 8, 5, 16, hair);
                Rect(t, 12, 12, 24, 3, hair);
                // mat, mieng
                Rect(t, 17, 20, 4, 4, outline);
                Rect(t, 27, 20, 4, 4, outline);
                P(t, 18, 20, Color.white); P(t, 28, 20, Color.white);
                Rect(t, 21, 29, 6, 2, Color.Lerp(skin, Color.red, 0.4f));
                Rect(t, 15, 26, 3, 2, Color.Lerp(skin, Color.red, 0.25f));
                Rect(t, 30, 26, 3, 2, Color.Lerp(skin, Color.red, 0.25f));
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Vien kim cuong / crystal
        public static Sprite Crystal(Color c, int size = 16)
        {
            return Cached("cr" + c + size, () =>
            {
                var t = NewTex(size, size);
                float half = size / 2f;
                var lite = Color.Lerp(c, Color.white, 0.55f);
                var dark = Color.Lerp(c, Color.black, 0.3f);
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        float d = Mathf.Abs(x - half + 0.5f) / half + Mathf.Abs(y - half + 0.5f) / half;
                        if (d <= 1f)
                            P(t, x, y, y < half * 0.7f ? lite : (x > half ? dark : c));
                    }
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Sap hang cho (16x20 pivot day) — mai hien mau
        public static Sprite Stall(Color awning)
        {
            return Cached("st" + awning, () =>
            {
                var t = NewTex(16, 20);
                var wood = new Color32(122, 90, 62, 255);
                var woodD = new Color32(92, 66, 44, 255);
                var stripe = Color.Lerp(awning, Color.white, 0.55f);
                // mai
                for (int x = 0; x < 16; x++)
                    Rect(t, x, 0, 1, 4, (x / 3) % 2 == 0 ? awning : stripe);
                Rect(t, 0, 4, 16, 1, Color.Lerp(awning, Color.black, 0.35f));
                // ban
                Rect(t, 1, 10, 14, 4, wood);
                Rect(t, 1, 13, 14, 1, woodD);
                // chan
                Rect(t, 2, 14, 2, 6, woodD);
                Rect(t, 12, 14, 2, 6, woodD);
                // hang hoa
                Rect(t, 3, 8, 3, 2, new Color32(120, 190, 90, 255));
                Rect(t, 7, 8, 3, 2, new Color32(230, 160, 70, 255));
                Rect(t, 11, 8, 3, 2, new Color32(210, 90, 80, 255));
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Be da dat crystal (16x22 pivot day)
        public static Sprite Pedestal()
        {
            return Cached("ped", () =>
            {
                var t = NewTex(16, 22);
                var stone = new Color32(168, 175, 196, 255);
                var dark = new Color32(112, 118, 140, 255);
                Rect(t, 3, 20, 10, 2, dark);
                Rect(t, 5, 10, 6, 10, stone);
                Rect(t, 5, 10, 6, 1, Color.Lerp(stone, Color.white, 0.4f));
                Rect(t, 3, 8, 10, 3, stone);
                Rect(t, 3, 10, 10, 1, dark);
                Rect(t, 9, 11, 2, 9, dark);
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // San ho / nam phat sang (16x14 pivot day)
        public static Sprite Coral(Color c, int seed)
        {
            return Cached("co" + c + seed, () =>
            {
                var t = NewTex(16, 14);
                var r = new System.Random(seed);
                var lite = Color.Lerp(c, Color.white, 0.4f);
                for (int i = 0; i < 4; i++)
                {
                    int bx = 2 + r.Next(10);
                    int h = 4 + r.Next(7);
                    Rect(t, bx, 14 - h, 2, h, c);
                    Rect(t, bx, 14 - h, 2, 2, lite);
                }
                Rect(t, 2, 12, 12, 2, Color.Lerp(c, Color.black, 0.35f));
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Bong doi chan (ellipse mo)
        public static Sprite Shadow()
        {
            return Cached("shadow", () =>
            {
                var t = NewTex(16, 8);
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 16; x++)
                    {
                        float dx = (x - 7.5f) / 7.5f, dy = (y - 3.5f) / 3.5f;
                        if (dx * dx + dy * dy <= 1f)
                            t.SetPixel(x, 7 - y, new Color(0, 0, 0.02f, 0.35f));
                    }
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Icon vat pham 16x16
        public static Sprite Icon(string kind, Color c)
        {
            return Cached("ic" + kind + c, () =>
            {
                var t = NewTex(16, 16);
                var outline = new Color32(30, 26, 40, 255);
                var lite = Color.Lerp(c, Color.white, 0.4f);
                switch (kind)
                {
                    case "book":
                        Rect(t, 2, 2, 12, 12, c);
                        Rect(t, 2, 2, 2, 12, Color.Lerp(c, Color.black, 0.4f));
                        Rect(t, 5, 4, 8, 1, lite); Rect(t, 5, 6, 8, 1, lite); Rect(t, 5, 8, 6, 1, lite);
                        break;
                    case "bag":
                        Rect(t, 3, 5, 10, 9, c);
                        Rect(t, 5, 2, 6, 3, Color.Lerp(c, Color.black, 0.3f));
                        Rect(t, 6, 8, 4, 2, lite);
                        break;
                    case "scroll":
                        Rect(t, 3, 2, 10, 12, new Color32(232, 216, 178, 255));
                        Rect(t, 3, 2, 10, 2, c); Rect(t, 3, 12, 10, 2, c);
                        Rect(t, 5, 6, 6, 1, outline); Rect(t, 5, 8, 6, 1, outline);
                        break;
                    case "lens":
                        for (int y = 0; y < 16; y++)
                            for (int x = 0; x < 16; x++)
                            {
                                float d = Vector2.Distance(new Vector2(x, y), new Vector2(6, 6));
                                if (d < 5 && d > 3.4f) P(t, x, y, c);
                                else if (d <= 3.4f) P(t, x, y, new Color(lite.r, lite.g, lite.b, 0.75f));
                            }
                        Rect(t, 10, 10, 4, 2, outline);
                        break;
                    default:
                        Rect(t, 4, 4, 8, 8, c);
                        break;
                }
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Panel UI bo goc + vien (9-slice)
        public static Sprite Panel9(Color fill, Color border)
        {
            return Cached("p9" + fill + border, () =>
            {
                int s = 48, r = 10, bw = 3;
                var t = NewTex(s, s);
                for (int y = 0; y < s; y++)
                    for (int x = 0; x < s; x++)
                    {
                        // khoang cach den goc gan nhat
                        float cx = Mathf.Clamp(x, r, s - 1 - r);
                        float cy = Mathf.Clamp(y, r, s - 1 - r);
                        float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                        if (d > r) continue;
                        t.SetPixel(x, y, d > r - bw ? border : fill);
                    }
                t.filterMode = FilterMode.Bilinear;
                t.Apply();
                return Sprite.Create(t, new UnityEngine.Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 16, 0,
                    SpriteMeshType.FullRect, new Vector4(14, 14, 14, 14));
            });
        }

        // Thung go (16x16 pivot day)
        public static Sprite Barrel()
        {
            return Cached("barrel", () =>
            {
                var t = NewTex(16, 16);
                var wood = new Color32(146, 106, 70, 255);
                var woodD = new Color32(104, 74, 48, 255);
                var band = new Color32(96, 98, 112, 255);
                Rect(t, 4, 2, 8, 13, wood);
                Rect(t, 3, 3, 1, 11, wood); Rect(t, 12, 3, 1, 11, wood);
                Rect(t, 5, 2, 2, 13, Color.Lerp((Color)wood, Color.white, 0.18f));
                Rect(t, 10, 2, 2, 13, woodD);
                Rect(t, 3, 4, 10, 1, band); Rect(t, 3, 11, 10, 1, band);
                Rect(t, 4, 2, 8, 1, woodD); Rect(t, 4, 14, 8, 1, woodD);
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Hom go dong hang (16x14 pivot day)
        public static Sprite Crate()
        {
            return Cached("crate", () =>
            {
                var t = NewTex(16, 14);
                var wood = new Color32(160, 122, 78, 255);
                var woodD = new Color32(112, 82, 52, 255);
                var woodL = Color.Lerp((Color)wood, Color.white, 0.2f);
                Rect(t, 2, 2, 12, 12, wood);
                // khung vien
                Rect(t, 2, 2, 12, 2, woodL); Rect(t, 2, 12, 12, 2, woodD);
                Rect(t, 2, 2, 2, 12, woodL); Rect(t, 12, 2, 2, 12, woodD);
                // van cheo
                for (int i = 0; i < 8; i++) { P(t, 4 + i, 4 + i, woodD); P(t, 5 + i, 4 + i, woodD); }
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Tang da (16x10 pivot day)
        public static Sprite Rock(int seed)
        {
            return Cached("rock" + seed, () =>
            {
                var t = NewTex(16, 10);
                var stone = new Color32(138, 140, 152, 255);
                var dark = new Color32(96, 98, 112, 255);
                var lite = Color.Lerp((Color)stone, Color.white, 0.25f);
                var r = new System.Random(seed);
                int w = 10 + r.Next(4);
                int x0 = (16 - w) / 2;
                // khoi da bau duc tho
                Rect(t, x0 + 1, 2, w - 2, 7, stone);
                Rect(t, x0, 4, w, 5, stone);
                Rect(t, x0 + 2, 1, w - 4, 2, stone);
                Rect(t, x0 + 2, 1, 4, 2, lite);
                Rect(t, x0 + 1, 7, w - 2, 2, dark);
                P(t, x0 + w / 2, 4, dark); P(t, x0 + w / 2 + 1, 5, dark); // vet nut
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Bui cay tron co qua mong (16x12 pivot day)
        public static Sprite Bush(Color leaf, int seed)
        {
            return Cached("bush" + leaf + seed, () =>
            {
                var t = NewTex(16, 12);
                var dark = Color.Lerp(leaf, Color.black, 0.3f);
                var lite = Color.Lerp(leaf, Color.white, 0.22f);
                for (int y = 0; y < 12; y++)
                    for (int x = 0; x < 16; x++)
                    {
                        float dx = (x - 7.5f) / 7.5f, dy = (y - 6.5f) / 5.5f;
                        if (dx * dx + dy * dy <= 1f)
                            P(t, x, y, y > 8 ? dark : (y < 4 && x < 8 ? lite : leaf));
                    }
                var r = new System.Random(seed);
                var berry = new Color32(214, 80, 90, 255);
                for (int i = 0; i < 3; i++)
                    P(t, 4 + r.Next(9), 4 + r.Next(5), berry);
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Gieng nuoc (16x22 pivot day)
        public static Sprite Well()
        {
            return Cached("well", () =>
            {
                var t = NewTex(16, 22);
                var stone = new Color32(158, 162, 178, 255);
                var stoneD = new Color32(108, 112, 130, 255);
                var wood = new Color32(120, 88, 60, 255);
                var roof = new Color32(150, 70, 60, 255);
                // mai ngoi
                Rect(t, 2, 0, 12, 2, roof);
                Rect(t, 1, 2, 14, 2, Color.Lerp((Color)roof, Color.black, 0.25f));
                // cot do mai
                Rect(t, 3, 4, 1, 10, wood); Rect(t, 12, 4, 1, 10, wood);
                // truc quay + day
                Rect(t, 5, 5, 6, 1, wood);
                Rect(t, 7, 6, 1, 6, new Color32(70, 60, 50, 255));
                // thanh gieng
                Rect(t, 2, 13, 12, 8, stone);
                Rect(t, 2, 13, 12, 1, Color.Lerp((Color)stone, Color.white, 0.3f));
                Rect(t, 4, 15, 8, 4, new Color32(30, 44, 68, 255)); // long gieng toi
                Rect(t, 2, 19, 12, 2, stoneD);
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Bien chi dan bang go (16x18 pivot day)
        public static Sprite Sign()
        {
            return Cached("sign", () =>
            {
                var t = NewTex(16, 18);
                var wood = new Color32(150, 112, 72, 255);
                var woodD = new Color32(104, 74, 48, 255);
                var text = new Color32(70, 52, 36, 255);
                Rect(t, 7, 8, 2, 10, woodD); // cot
                Rect(t, 2, 2, 12, 7, wood);  // bang
                Rect(t, 2, 2, 12, 1, Color.Lerp((Color)wood, Color.white, 0.25f));
                Rect(t, 2, 8, 12, 1, woodD);
                Rect(t, 4, 4, 8, 1, text); Rect(t, 4, 6, 5, 1, text); // dong chu
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Khom hoa nho rai tren nen (16x16 pivot giua, ve de len tile dat)
        public static Sprite Flora(int seed)
        {
            return Cached("flora" + seed, () =>
            {
                var t = NewTex(16, 16);
                var r = new System.Random(seed);
                var stem = new Color32(70, 110, 55, 255);
                Color[] petals =
                {
                    new Color32(240, 240, 235, 255), new Color32(232, 120, 120, 255),
                    new Color32(240, 200, 90, 255), new Color32(190, 130, 220, 255)
                };
                int count = 1 + r.Next(2);
                for (int i = 0; i < count; i++)
                {
                    int fx = 3 + r.Next(9), fy = 4 + r.Next(7);
                    var petal = petals[r.Next(petals.Length)];
                    P(t, fx, fy + 2, stem); P(t, fx, fy + 3, stem);
                    P(t, fx - 1, fy, petal); P(t, fx + 1, fy, petal);
                    P(t, fx, fy - 1, petal); P(t, fx, fy + 1, petal);
                    P(t, fx, fy, new Color32(250, 220, 110, 255));
                }
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Cum co la nho rai tren nen (16x16 pivot giua)
        public static Sprite Tuft(int seed, Color grass)
        {
            return Cached("tuft" + seed + grass, () =>
            {
                var t = NewTex(16, 16);
                var r = new System.Random(seed * 3 + 1);
                var dark = Color.Lerp(grass, Color.black, 0.2f);
                int blades = 3 + r.Next(3);
                for (int i = 0; i < blades; i++)
                {
                    int bx = 3 + r.Next(10), by = 5 + r.Next(6);
                    int h = 2 + r.Next(2);
                    Rect(t, bx, by, 1, h, r.Next(2) == 0 ? grass : dark);
                }
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Ngoi nha 48x40 (rong 3 o), pivot day-giua. Mai + tuong go + cua + 2 cua so sang den.
        public static Sprite House(Color wall, Color roof)
        {
            return Cached("house" + wall + roof, () =>
            {
                var t = NewTex(48, 40);
                var roofD = Color.Lerp(roof, Color.black, 0.3f);
                var roofL = Color.Lerp(roof, Color.white, 0.2f);
                var wallD = Color.Lerp(wall, Color.black, 0.35f);
                var frame = new Color32(92, 62, 40, 255);
                var door = new Color32(96, 64, 40, 255);
                var glowWin = new Color32(250, 214, 120, 255);

                // mai dau hoi
                for (int y = 0; y < 14; y++)
                {
                    int hw = 5 + Mathf.RoundToInt(y * 1.35f);
                    int x0 = Mathf.Max(1, 24 - hw), x1 = Mathf.Min(47, 24 + hw);
                    Rect(t, x0, y, x1 - x0, 1, (y % 4 == 0) ? roofD : roof);
                }
                Rect(t, 19, 0, 10, 2, roofL);           // song mai
                Rect(t, 2, 14, 44, 2, roofD);           // diem mai
                // tuong
                Rect(t, 4, 16, 40, 20, wall);
                Rect(t, 4, 16, 40, 1, Color.Lerp(wall, Color.white, 0.2f));
                Rect(t, 4, 16, 2, 20, frame); Rect(t, 42, 16, 2, 20, frame);
                Rect(t, 23, 16, 2, 20, frame);          // cot go giua
                Rect(t, 4, 26, 40, 1, frame);           // xa ngang
                // cua ra vao
                Rect(t, 20, 24, 8, 12, door);
                Rect(t, 20, 24, 8, 1, frame);
                P(t, 26, 30, glowWin);                  // num cua
                // 2 cua so
                Rect(t, 8, 20, 8, 7, frame);
                Rect(t, 9, 21, 6, 5, glowWin);
                Rect(t, 32, 20, 8, 7, frame);
                Rect(t, 33, 21, 6, 5, glowWin);
                // mong nha
                Rect(t, 4, 36, 40, 4, wallD);
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Hang rao go (1 doan 16x14, pivot day) — ghep lien nhau thanh hang dai
        public static Sprite Fence()
        {
            return Cached("fence", () =>
            {
                var t = NewTex(16, 14);
                var wood = new Color32(150, 112, 72, 255);
                var woodD = new Color32(104, 74, 48, 255);
                var woodL = Color.Lerp((Color)wood, Color.white, 0.2f);
                // 2 cot
                Rect(t, 1, 3, 3, 11, wood);
                Rect(t, 12, 3, 3, 11, wood);
                Rect(t, 1, 3, 3, 1, woodL); Rect(t, 12, 3, 3, 1, woodL);
                Rect(t, 1, 13, 3, 1, woodD); Rect(t, 12, 13, 3, 1, woodD);
                // 2 thanh ngang
                Rect(t, 0, 5, 16, 2, wood);
                Rect(t, 0, 6, 16, 1, woodD);
                Rect(t, 0, 9, 16, 2, wood);
                Rect(t, 0, 10, 16, 1, woodD);
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Con ga 16x16, 4 frame (0=ngang dau, 1=trung gian, 2=cui mo thoc, 3=vo canh), pivot day
        public static Sprite Chicken(int frame)
        {
            return Cached("chick" + frame, () =>
            {
                var t = NewTex(16, 16);
                var body = new Color32(245, 242, 230, 255);
                var bodyD = new Color32(205, 200, 185, 255);
                var bodyL = new Color32(255, 253, 245, 255);
                var comb = new Color32(216, 62, 58, 255);
                var wattle = new Color32(200, 55, 60, 255);
                var beak = new Color32(232, 162, 60, 255);
                var eye = new Color32(30, 26, 34, 255);
                var wing = new Color32(224, 218, 200, 255);
                var wingD = new Color32(196, 190, 172, 255);

                // than
                Rect(t, 3, 7, 9, 7, body);
                Rect(t, 3, 12, 9, 2, bodyD);
                Rect(t, 3, 7, 9, 1, bodyL);
                Rect(t, 1, 7, 3, 4, bodyD); // duoi xoe

                // canh: gap sat than binh thuong, xoe len khi vo canh (frame 3)
                if (frame == 3)
                {
                    Rect(t, 4, 4, 6, 4, wing);
                    Rect(t, 4, 4, 6, 1, wingD);
                }
                else
                {
                    Rect(t, 4, 8, 6, 5, wing);
                    Rect(t, 4, 12, 6, 1, wingD);
                }

                int hy = frame == 2 ? 3 : (frame == 1 ? 1 : 0); // dau cui xuong dan khi mo thoc
                // dau
                Rect(t, 10, 3 + hy, 4, 4, body);
                P(t, 11, 2 + hy, comb); P(t, 12, 2 + hy, comb); P(t, 11, 1 + hy, comb);
                P(t, 10, 5 + hy, wattle);
                Rect(t, 13, 4 + hy, 2, 1, beak);
                P(t, 11, 4 + hy, eye);

                // chan
                Rect(t, 5, 14, 1, 2, beak); Rect(t, 8, 14, 1, 2, beak);
                OutlineTex(t, new Color(0.1f, 0.08f, 0.09f));
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Con cho 20x14, 4 frame (chu ky chay trot + vay duoi), pivot day
        public static Sprite Dog(int frame)
        {
            return Cached("dog" + frame, () =>
            {
                var t = NewTex(20, 14);
                var fur = new Color32(162, 120, 76, 255);
                var furD = new Color32(120, 86, 54, 255);
                var furL = new Color32(188, 148, 100, 255);
                var eye = new Color32(30, 26, 34, 255);

                // than
                Rect(t, 4, 4, 11, 6, fur);
                Rect(t, 4, 8, 11, 2, furD);
                Rect(t, 4, 4, 11, 1, furL);

                // dau + tai cup + mom
                Rect(t, 13, 1, 6, 6, fur);
                Rect(t, 13, 0, 2, 2, furD); Rect(t, 16, 0, 2, 2, furD);
                P(t, 16, 3, eye);
                Rect(t, 18, 4, 2, 2, furD);

                // duoi vay len/xuong xen ke theo frame
                if (frame == 0 || frame == 2) Rect(t, 1, 2, 3, 4, fur);
                else Rect(t, 1, 5, 3, 4, fur);

                // 4 chan, chu ky chay: chan cheo nhau nang len xen ke
                bool a = frame % 2 == 0;
                Rect(t, 5, 10, 2, a ? 4 : 3, furD);
                Rect(t, 9, 10, 2, a ? 3 : 4, furD);
                Rect(t, 12, 10, 2, a ? 3 : 4, furD);
                Rect(t, 15, 10, 2, a ? 4 : 3, furD);

                OutlineTex(t, new Color(0.08f, 0.06f, 0.07f));
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Bo 20x16, 2 frame (nhai co: dau nhun nhe), pivot day
        public static Sprite Cow(int frame)
        {
            return Cached("cow" + frame, () =>
            {
                var t = NewTex(20, 16);
                var body = new Color32(240, 236, 226, 255);
                var patch = new Color32(60, 52, 50, 255);
                var bodyD = new Color32(210, 204, 192, 255);
                var horn = new Color32(220, 210, 190, 255);
                var eye = new Color32(30, 26, 34, 255);
                var nose = new Color32(80, 60, 60, 255);

                Rect(t, 3, 6, 12, 7, body);
                Rect(t, 3, 11, 12, 2, bodyD);
                Rect(t, 5, 6, 4, 3, patch); Rect(t, 10, 9, 4, 3, patch);
                Rect(t, 1, 5, 3, 5, bodyD); // duoi

                int hy = frame == 1 ? 1 : 0;
                Rect(t, 14, 4 + hy, 5, 5, body);
                Rect(t, 13, 3 + hy, 2, 1, horn); Rect(t, 18, 3 + hy, 2, 1, horn);
                Rect(t, 17, 6 + hy, 2, 2, nose);
                P(t, 16, 5 + hy, eye);

                Rect(t, 4, 13, 2, 3, patch); Rect(t, 8, 13, 2, 3, patch);
                Rect(t, 12, 13, 2, 3, patch);
                OutlineTex(t, new Color(0.1f, 0.09f, 0.09f));
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Cuu 16x14, 2 frame (dau nhun nhe khi gam co), long xu bong, pivot day
        public static Sprite Sheep(int frame)
        {
            return Cached("sheep" + frame, () =>
            {
                var t = NewTex(16, 14);
                var wool = new Color32(238, 234, 224, 255);
                var woolD = new Color32(212, 206, 192, 255);
                var head = new Color32(60, 52, 50, 255);
                var eye = new Color32(230, 225, 210, 255);

                for (int y = 3; y < 11; y++)
                    for (int x = 2; x < 12; x++)
                    {
                        float dx = (x - 6.5f) / 5f, dy = (y - 6.5f) / 4f;
                        if (dx * dx + dy * dy <= 1f) P(t, x, y, y > 7 ? woolD : wool);
                    }

                int hy = frame == 1 ? 1 : 0;
                Rect(t, 11, 4 + hy, 4, 4, head);
                P(t, 14, 5 + hy, eye);

                Rect(t, 3, 11, 2, 2, head); Rect(t, 8, 11, 2, 2, head);
                OutlineTex(t, new Color(0.1f, 0.09f, 0.09f));
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Lon 16x12, 2 frame (dau nhun nhe), duoi xoan, pivot day
        public static Sprite Pig(int frame)
        {
            return Cached("pig" + frame, () =>
            {
                var t = NewTex(16, 12);
                var body = new Color32(240, 176, 186, 255);
                var bodyD = new Color32(214, 148, 158, 255);
                var snout = new Color32(224, 140, 150, 255);
                var eye = new Color32(40, 30, 32, 255);

                Rect(t, 3, 4, 9, 6, body);
                Rect(t, 3, 8, 9, 2, bodyD);
                P(t, 2, 4, bodyD); P(t, 2, 5, bodyD); P(t, 2, 3, bodyD); // duoi xoan

                int hy = frame == 1 ? 1 : 0;
                Rect(t, 10, 4 + hy, 4, 4, body);
                Rect(t, 12, 6 + hy, 2, 2, snout);
                P(t, 11, 5 + hy, eye);
                Rect(t, 9, 3 + hy, 1, 1, bodyD); Rect(t, 11, 3 + hy, 1, 1, bodyD); // tai

                Rect(t, 4, 10, 2, 2, bodyD); Rect(t, 8, 10, 2, 2, bodyD);
                OutlineTex(t, new Color(0.1f, 0.06f, 0.07f));
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Meo 12x10, 2 frame (ngoi cuon minh, duoi ve nhe), pivot day
        public static Sprite Cat(int frame)
        {
            return Cached("cat" + frame, () =>
            {
                var t = NewTex(12, 10);
                var fur = new Color32(90, 88, 96, 255);
                var furD = new Color32(64, 62, 70, 255);
                var eye = new Color32(210, 200, 90, 255);

                Rect(t, 2, 3, 6, 5, fur);
                Rect(t, 2, 6, 6, 2, furD);
                Rect(t, 2, 1, 4, 3, fur);
                P(t, 2, 0, fur); P(t, 5, 0, fur); // tai
                P(t, 4, 3, eye);

                if (frame == 0) Rect(t, 7, 4, 3, 1, furD);
                else Rect(t, 7, 2, 2, 3, furD);

                Rect(t, 3, 8, 1, 1, furD); Rect(t, 6, 8, 1, 1, furD);
                OutlineTex(t, new Color(0.06f, 0.06f, 0.07f));
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Diem lap lanh danh dau cau chuyen an (12x14 pivot day)
        public static Sprite Sparkle()
        {
            return Cached("sparkle", () =>
            {
                var t = NewTex(12, 14);
                var core = new Color(0.85f, 0.98f, 1f);
                var mid = new Color(0.5f, 0.85f, 0.95f);
                // ngoi sao 4 canh
                Rect(t, 5, 2, 2, 9, mid);
                Rect(t, 2, 5, 8, 2, mid);
                Rect(t, 5, 4, 2, 4, core);
                Rect(t, 4, 5, 4, 2, core);
                P(t, 2, 2, mid); P(t, 9, 2, mid); P(t, 2, 10, mid); P(t, 9, 10, mid);
                // hon da nho ben duoi
                Rect(t, 3, 11, 6, 3, new Color32(120, 124, 142, 255));
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Panel go 9-slice cho UI: vien go toi + chi vang + van go ngang (giong khung go RPG).
        // Do phan giai cao (144px) + Point filter + ppu 100 (bang referencePixelsPerUnit cua
        // CanvasScaler) de vien luon sac net, khong bi keo gian mo.
        public static Sprite Wood9()
        {
            return Cached("wood9", () =>
            {
                int s = 144, r = 14, bw = 6;
                var t = NewTex(s, s);
                var border = new Color(0.17f, 0.10f, 0.06f, 1f);
                var gold = new Color(0.80f, 0.62f, 0.30f, 1f);
                var wood = new Color(0.36f, 0.23f, 0.12f, 0.97f);
                var woodD = new Color(0.31f, 0.19f, 0.10f, 0.97f);
                var seam = new Color(0.26f, 0.16f, 0.08f, 0.97f);
                for (int y = 0; y < s; y++)
                    for (int x = 0; x < s; x++)
                    {
                        float cx = Mathf.Clamp(x, r, s - 1 - r);
                        float cy = Mathf.Clamp(y, r, s - 1 - r);
                        float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                        if (d > r) continue;
                        Color c;
                        if (d > r - bw) c = border;
                        else if (d > r - bw - 3f) c = gold;
                        else
                        {
                            int band = y % 18;
                            c = band < 2 ? seam : ((y / 18) % 2 == 0 ? wood : woodD);
                        }
                        t.SetPixel(x, y, c);
                    }
                t.Apply();
                return Sprite.Create(t, new UnityEngine.Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 100, 0,
                    SpriteMeshType.FullRect, new Vector4(16, 16, 16, 16));
            });
        }

        // Vai vien soi nho rai tren duong di (16x16 pivot giua)
        public static Sprite Pebble(int seed)
        {
            return Cached("pebble" + seed, () =>
            {
                var t = NewTex(16, 16);
                var r = new System.Random(seed * 5 + 2);
                var stone = new Color32(150, 152, 164, 255);
                var stoneD = new Color32(112, 114, 128, 255);
                int count = 2 + r.Next(2);
                for (int i = 0; i < count; i++)
                {
                    int px = 3 + r.Next(10), py = 5 + r.Next(7);
                    Rect(t, px, py, 2, 2, stone);
                    P(t, px + 1, py + 1, stoneD);
                }
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Chiec la nho bay trong gio (5x5 pivot giua)
        public static Sprite Leaf(Color c)
        {
            return Cached("leaf" + c, () =>
            {
                var t = NewTex(5, 5);
                var dark = Color.Lerp(c, Color.black, 0.25f);
                P(t, 1, 3, c); P(t, 2, 2, c); P(t, 3, 1, c);
                P(t, 2, 3, dark); P(t, 3, 2, dark);
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Con buom 10x8, 2 frame (canh cup/xoe), pivot giua
        public static Sprite Butterfly(int frame, Color wing)
        {
            return Cached("bfly" + frame + wing, () =>
            {
                var t = NewTex(10, 8);
                var body = new Color32(45, 38, 52, 255);
                var wingL = Color.Lerp(wing, Color.white, 0.35f);
                Rect(t, 4, 2, 2, 5, body);
                if (frame == 0)
                {
                    // canh cup len
                    Rect(t, 2, 1, 2, 3, wing); Rect(t, 6, 1, 2, 3, wing);
                    P(t, 2, 1, wingL); P(t, 7, 1, wingL);
                }
                else
                {
                    // canh xoe ngang
                    Rect(t, 1, 3, 3, 2, wing); Rect(t, 6, 3, 3, 2, wing);
                    P(t, 1, 3, wingL); P(t, 9, 3, wingL);
                }
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Dat ruong da cay (16x16 tile) — nen nau + vang cay ngang, giong Ground nhung co ranh
        public static Sprite TilledSoil(int seed)
        {
            return Cached("till" + seed, () =>
            {
                var t = NewTex(16, 16);
                var soil = new Color32(112, 78, 52, 255);
                var soilD = new Color32(84, 58, 38, 255);
                var soilL = new Color32(134, 96, 64, 255);
                Rect(t, 0, 0, 16, 16, soil);
                for (int y = 1; y < 16; y += 4)
                {
                    Rect(t, 0, y, 16, 1, soilD);
                    Rect(t, 0, y + 1, 16, 1, soilL);
                }
                var r = new System.Random(seed);
                for (int i = 0; i < 4; i++)
                    P(t, r.Next(16), r.Next(16), soilL);
                return Make(t, 16, new Vector2(0.5f, 0.5f));
            });
        }

        // Luong cay trong 16x20 pivot day — 3-4 nhanh, type chon mau (0=rau xanh,1=lua vang,2=ngo)
        public static Sprite Crop(int type, int seed)
        {
            return Cached("crop" + type + seed, () =>
            {
                var t = NewTex(16, 20);
                var stem = new Color32(70, 110, 55, 255);
                Color leafC = type == 0 ? new Color32(90, 165, 70, 255)
                    : type == 1 ? new Color32(215, 185, 80, 255)
                    : new Color32(200, 165, 60, 255);
                var leafD = Color.Lerp(leafC, Color.black, 0.25f);
                var r = new System.Random(seed * 11 + 3);
                int stalks = 3 + r.Next(2);
                for (int i = 0; i < stalks; i++)
                {
                    int bx = 2 + r.Next(12);
                    int h = 7 + r.Next(6);
                    Rect(t, bx, 20 - h, 1, h, stem);
                    P(t, bx - 1, 20 - h, leafD); P(t, bx + 1, 20 - h + 1, leafC);
                    P(t, bx, 20 - h - 1, leafC);
                    if (type == 2) P(t, bx, 20 - h, new Color32(235, 210, 120, 255)); // bap ngo
                }
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Bu nhin rom 16x22 pivot day — coc + ao rom + mu, tay dang ngang
        public static Sprite Scarecrow()
        {
            return Cached("scare", () =>
            {
                var t = NewTex(16, 22);
                var wood = new Color32(120, 88, 58, 255);
                var straw = new Color32(210, 178, 90, 255);
                var strawD = new Color32(180, 148, 68, 255);
                var cloth = new Color32(140, 70, 60, 255);
                var hat = new Color32(90, 62, 42, 255);
                var face = new Color32(224, 196, 150, 255);

                Rect(t, 7, 8, 2, 12, wood);      // coc doc
                Rect(t, 2, 12, 12, 2, wood);     // tay ngang
                Rect(t, 3, 13, 2, 4, cloth); Rect(t, 11, 13, 2, 4, cloth); // tay ao rom hai ben
                Rect(t, 5, 10, 6, 8, cloth);     // than ao
                Rect(t, 5, 15, 6, 1, strawD);
                P(t, 5, 18, straw); P(t, 10, 19, straw); P(t, 6, 20, straw); // rom lo ra chan

                Rect(t, 5, 4, 6, 5, face);       // mat rom (dau)
                P(t, 6, 6, new Color32(40, 30, 30, 255)); P(t, 9, 6, new Color32(40, 30, 30, 255));
                Rect(t, 4, 2, 8, 2, hat); Rect(t, 3, 4, 10, 1, hat); // non
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Dong rom 16x14 pivot day — bo rom vang tron
        public static Sprite Haystack(int seed)
        {
            return Cached("hay" + seed, () =>
            {
                var t = NewTex(16, 14);
                var straw = new Color32(216, 182, 92, 255);
                var strawD = new Color32(184, 150, 68, 255);
                var strawL = new Color32(232, 204, 128, 255);
                var r = new System.Random(seed);
                for (int y = 2; y < 13; y++)
                    for (int x = 1; x < 15; x++)
                    {
                        float dx = (x - 7.5f) / 7f, dy = (y - 8f) / 5.5f;
                        if (dx * dx + dy * dy <= 1f) P(t, x, y, strawD);
                    }
                for (int i = 0; i < 10; i++)
                    P(t, 3 + r.Next(10), 3 + r.Next(8), r.Next(2) == 0 ? straw : strawL);
                Rect(t, 3, 11, 10, 2, strawD);
                return Make(t, 16, new Vector2(0.5f, 0f));
            });
        }

        // Gradient doc cho nen menu
        public static Sprite VGradient(Color top, Color bottom, int size = 128)
        {
            return Cached("vg" + top + bottom, () =>
            {
                var t = NewTex(4, size);
                for (int y = 0; y < size; y++)
                {
                    var c = Color.Lerp(bottom, top, y / (float)(size - 1));
                    for (int x = 0; x < 4; x++) t.SetPixel(x, y, c);
                }
                t.filterMode = FilterMode.Bilinear;
                t.Apply();
                return Sprite.Create(t, new UnityEngine.Rect(0, 0, 4, size), new Vector2(0.5f, 0.5f), 16);
            });
        }
    }
}
