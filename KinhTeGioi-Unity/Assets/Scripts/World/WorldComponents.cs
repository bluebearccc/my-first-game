using System.Collections.Generic;
using UnityEngine;

namespace KTG
{
    public class CameraFollow : MonoBehaviour
    {
        const float TargetVerticalUnits = 11f; // muon thay ~11 o theo chieu doc
        const int PPU = 16;

        public Transform target;
        Camera cam;
        int lastScreenH;

        void Awake() { cam = GetComponent<Camera>(); }

        void LateUpdate() { Apply(); }

        // Pixel-perfect: chon he so phong to nguyen (1 texel = zoom x zoom pixel man hinh)
        // de pixel art deu tam va sac net o moi do phan giai.
        void UpdatePixelPerfectSize()
        {
            if (Screen.height == lastScreenH) return;
            lastScreenH = Screen.height;
            int zoom = Mathf.Max(1, Mathf.RoundToInt(Screen.height / (PPU * TargetVerticalUnits)));
            cam.orthographicSize = Screen.height / (2f * PPU * zoom);
        }

        float shakeTime, shakeDur, shakeAmp;

        // Rung camera nhe khi co phan hoi tuong tac (nhan crystal, don danh boss...)
        public void Shake(float amp = 0.12f, float dur = 0.22f)
        {
            shakeAmp = amp; shakeDur = dur; shakeTime = dur;
        }

        public void Apply()
        {
            if (target == null || cam == null || MapBuilder.Width <= 0) return;
            if (Hd2dView.Diorama) { ApplyDiorama(); return; }
            UpdatePixelPerfectSize();
            float vert = cam.orthographicSize;
            float horz = vert * cam.aspect;

            float minX = horz, maxX = MapBuilder.Width - horz;
            float minY = -MapBuilder.Height + vert, maxY = -vert;
            if (minX > maxX) { float mid = (minX + maxX) * 0.5f; minX = maxX = mid; }
            if (minY > maxY) { float mid = (minY + maxY) * 0.5f; minY = maxY = mid; }

            float x = Mathf.Clamp(target.position.x, minX, maxX);
            float y = Mathf.Clamp(target.position.y, minY, maxY);

            // Bam camera vao luoi pixel de tranh texel bi lech/nhap nhay khi di chuyen
            float grid = PPU * Mathf.Max(1, Mathf.RoundToInt(Screen.height / (PPU * TargetVerticalUnits)));
            x = Mathf.Round(x * grid) / grid;
            y = Mathf.Round(y * grid) / grid;

            if (shakeTime > 0f)
            {
                shakeTime -= Time.deltaTime;
                float k = shakeAmp * Mathf.Clamp01(shakeTime / shakeDur);
                x += (Mathf.PerlinNoise(Time.time * 45f, 0.3f) - 0.5f) * 2f * k;
                y += (Mathf.PerlinNoise(0.7f, Time.time * 45f) - 0.5f) * 2f * k;
            }
            transform.position = new Vector3(x, y, transform.position.z);
        }

        // Phase D3: camera phoi canh nghieng — clamp theo footprint gan dung tai mat dat,
        // khong pixel-snap (phoi canh khong co luoi texel co dinh de bam).
        void ApplyDiorama()
        {
            cam.orthographic = false;
            cam.fieldOfView = Hd2dView.Fov;

            float vert = Hd2dView.Dist * Mathf.Tan(Hd2dView.Fov * 0.5f * Mathf.Deg2Rad);
            float horz = vert * cam.aspect;

            float minX = horz, maxX = MapBuilder.Width - horz;
            float minY = -MapBuilder.Height + vert, maxY = -vert;
            if (minX > maxX) { float mid = (minX + maxX) * 0.5f; minX = maxX = mid; }
            if (minY > maxY) { float mid = (minY + maxY) * 0.5f; minY = maxY = mid; }

            float x = Mathf.Clamp(target.position.x, minX, maxX);
            float y = Mathf.Clamp(target.position.y, minY, maxY);

            if (shakeTime > 0f)
            {
                shakeTime -= Time.deltaTime;
                float k = shakeAmp * Mathf.Clamp01(shakeTime / shakeDur);
                x += (Mathf.PerlinNoise(Time.time * 45f, 0.3f) - 0.5f) * 2f * k;
                y += (Mathf.PerlinNoise(0.7f, Time.time * 45f) - 0.5f) * 2f * k;
            }

            var focus = new Vector3(x, y, 0f);
            transform.rotation = Hd2dView.Rot;
            transform.position = focus - Hd2dView.Rot * Vector3.forward * Hd2dView.Dist;
        }
    }

    // Hieu ung moi truong theo map: la bay (map co cay), buom (map sang),
    // dom dom (map huyen ao) — the gioi khong con tinh lang.
    public class AmbientFX : MonoBehaviour
    {
        public int MapIndex;

        class Part
        {
            public Transform tr;
            public SpriteRenderer sr;
            public int kind;   // 0 la, 1 dom dom, 2 buom
            public float phase;
            public Vector3 target;
            public float frameT;
            public int frame;
        }

        readonly List<Part> parts = new List<Part>();

        void Start()
        {
            int leaves = MapIndex <= 1 ? 10 : 0;
            int fireflies = MapIndex >= 3 ? 14 : 0;
            int butterflies = MapIndex <= 2 ? 4 : 0;

            Color[] leafCols = { new Color(0.45f, 0.62f, 0.28f), new Color(0.75f, 0.55f, 0.25f), new Color(0.55f, 0.68f, 0.3f) };
            Color[] wingCols = { new Color(0.95f, 0.75f, 0.3f), new Color(0.85f, 0.5f, 0.75f), new Color(0.6f, 0.8f, 0.95f) };

            for (int i = 0; i < leaves; i++)
                Spawn(0, PixelArt.Leaf(leafCols[i % leafCols.Length]), RandomPos(), 3000);
            for (int i = 0; i < fireflies; i++)
                Spawn(1, PixelArt.Glow(new Color(0.75f, 0.95f, 0.5f, 0.8f), 16), RandomPos(), 3200);
            for (int i = 0; i < butterflies; i++)
                Spawn(2, PixelArt.Butterfly(0, wingCols[i % wingCols.Length]), RandomPos(), 2500);
        }

        void Spawn(int kind, Sprite sprite, Vector3 pos, int order)
        {
            var go = new GameObject("fx");
            go.transform.SetParent(transform, false);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = order;
            if (kind == 2) go.transform.localScale = Vector3.one * 0.9f;
            parts.Add(new Part { tr = go.transform, sr = sr, kind = kind, phase = Random.Range(0f, 6.28f), target = pos });
        }

        Vector3 RandomPos()
        {
            return new Vector3(Random.Range(1f, MapBuilder.Width - 1f), -Random.Range(1f, MapBuilder.Height - 1f), 0f);
        }

        void Update()
        {
            float dt = Time.deltaTime;
            foreach (var p in parts)
            {
                switch (p.kind)
                {
                    case 0: // la roi cheo, dong dua theo gio
                    {
                        var pos = p.tr.position;
                        pos.y -= (0.45f + 0.2f * Mathf.Sin(p.phase)) * dt;
                        pos.x += (Mathf.Sin(Time.time * 2f + p.phase) * 0.5f - 0.15f) * dt;
                        if (pos.y < -(MapBuilder.Height - 0.5f) || pos.x < 0.5f)
                            pos = new Vector3(Random.Range(1f, MapBuilder.Width - 1f), -0.5f, 0f);
                        p.tr.position = pos;
                        break;
                    }
                    case 1: // dom dom troi lo lung, sang toi theo nhip
                    {
                        var pos = p.tr.position;
                        pos.x += (Mathf.PerlinNoise(Time.time * 0.4f + p.phase, 0f) - 0.5f) * 1.2f * dt;
                        pos.y += (Mathf.PerlinNoise(0f, Time.time * 0.4f + p.phase) - 0.5f) * 1.2f * dt;
                        pos.x = Mathf.Clamp(pos.x, 1f, MapBuilder.Width - 1f);
                        pos.y = Mathf.Clamp(pos.y, -(MapBuilder.Height - 1f), -1f);
                        p.tr.position = pos;
                        var c = p.sr.color;
                        c.a = 0.35f + 0.45f * (0.5f + 0.5f * Mathf.Sin(Time.time * 2.5f + p.phase * 3f));
                        p.sr.color = c;
                        break;
                    }
                    case 2: // buom dap canh bay giua cac diem ngau nhien
                    {
                        p.frameT += dt;
                        if (p.frameT > 0.14f)
                        {
                            p.frameT = 0f;
                            p.frame = 1 - p.frame;
                            p.sr.sprite = PixelArt.Butterfly(p.frame, WingColor(p));
                        }
                        Vector3 d = p.target - p.tr.position;
                        if (d.magnitude < 0.15f) p.target = RandomPos();
                        else
                        {
                            var pos = p.tr.position + d.normalized * 1.1f * dt;
                            pos.y += Mathf.Sin(Time.time * 6f + p.phase) * 0.35f * dt;
                            p.tr.position = pos;
                            if (Mathf.Abs(d.x) > 0.05f) p.sr.flipX = d.x < 0f;
                        }
                        break;
                    }
                }
            }
        }

        Color WingColor(Part p)
        {
            Color[] wingCols = { new Color(0.95f, 0.75f, 0.3f), new Color(0.85f, 0.5f, 0.75f), new Color(0.6f, 0.8f, 0.95f) };
            return wingCols[parts.IndexOf(p) % wingCols.Length];
        }
    }

    // Hat crystal bay toa ra khi nhan thuong — phan hoi truc quan cho khoanh khac quan trong.
    public class SparkFly : MonoBehaviour
    {
        public Vector3 Vel;
        const float LifeMax = 0.7f;
        float life = LifeMax;
        SpriteRenderer sr;

        void Awake() { sr = GetComponent<SpriteRenderer>(); }

        void Update()
        {
            life -= Time.deltaTime;
            if (life <= 0f) { Destroy(gameObject); return; }
            transform.position += Vel * Time.deltaTime;
            Vel *= 1f - 2.5f * Time.deltaTime;
            Vel.y += 0.8f * Time.deltaTime; // hoi bay len nhu bui phep
            var c = sr.color;
            c.a = life / LifeMax;
            sr.color = c;
        }

        public static void Burst(Vector3 pos, Transform parent, int count = 10)
        {
            for (int i = 0; i < count; i++)
            {
                var go = new GameObject("spark");
                go.transform.SetParent(parent, false);
                go.transform.position = pos;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = PixelArt.Crystal(new Color(0.62f, 0.95f, 1f), 8);
                sr.sortingOrder = 6000;
                float ang = i / (float)count * Mathf.PI * 2f + Random.Range(-0.3f, 0.3f);
                go.AddComponent<SparkFly>().Vel =
                    new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0f) * Random.Range(1.6f, 2.6f);
            }
        }
    }

    // Nhap nhay quang duoc (glow child sprite + Light2D that) bang Perlin noise.
    public class TorchFlicker : MonoBehaviour
    {
        public SpriteRenderer Glow;
        public UnityEngine.Rendering.Universal.Light2D Light; // Phase B: den that nhap nhay cung glow
        float seed;
        float lightBase = -1f;

        void Awake() { seed = Random.Range(0f, 100f); }

        void Update()
        {
            float n = Mathf.PerlinNoise(Time.time * 6f + seed, 0f);
            if (Glow != null)
            {
                var c = Glow.color;
                c.a = 0.55f + n * 0.35f;
                Glow.color = c;
                float s = 0.9f + n * 0.25f;
                Glow.transform.localScale = new Vector3(s, s, 1f);
            }
            if (Light != null)
            {
                if (lightBase < 0f) lightBase = Light.intensity;
                Light.intensity = lightBase * (0.8f + n * 0.4f);
            }
        }
    }

    // Nhap nho len xuong dung cho crystal, marker NPC, vat trang tri.
    public class Bobber : MonoBehaviour
    {
        public float Amplitude = 0.08f;
        public float Speed = 2.5f;
        float phase;
        Vector3 basePos;

        void Awake()
        {
            phase = Random.Range(0f, Mathf.PI * 2f);
            basePos = transform.localPosition;
        }

        void Update()
        {
            transform.localPosition = basePos + new Vector3(0f, Mathf.Sin(Time.time * Speed + phase) * Amplitude, 0f);
        }
    }

    // NPC dung yen nhung "tho" nhe nhang (doi giua frame dung yen va frame tho),
    // va quay mat ve phia nguoi choi khi nguoi choi den gan.
    public class NpcIdle : MonoBehaviour
    {
        public Color Hair, Skin, Shirt;
        public Color Pants = new Color(0.3f, 0.28f, 0.35f);

        static PlayerController player; // player ton tai suot game nen cache dung chung

        SpriteRenderer sr;
        float phase;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            phase = Random.Range(0f, 4f);
            if (player == null) player = Object.FindObjectOfType<PlayerController>();
        }

        void Update()
        {
            int facing = 0;
            bool flip = false;
            if (player != null)
            {
                Vector3 d = player.transform.position - transform.position;
                if (d.sqrMagnitude < 9f) // trong ban kinh 3 o thi quay ve phia nguoi choi
                {
                    if (Mathf.Abs(d.x) > Mathf.Abs(d.y)) { facing = 2; flip = d.x < 0f; }
                    else facing = d.y > 0f ? 1 : 0;
                }
            }
            bool breathe = ((Time.time + phase) % 2.4f) > 1.2f;
            sr.sprite = PixelArt.Character(Hair, Skin, Shirt, Pants, facing, breathe ? 3 : 0);
            sr.flipX = flip;
        }
    }

    public enum AnimalKind { Chicken, Dog, Cow, Sheep, Pig, Cat }

    // Dan lang di lang thang trang tri (khong tuong tac/khong thoai): buoc frame 1/2 khi di,
    // tho nhe frame 0/3 khi dung, quay mat theo huong di chuyen. Tai su dung logic tim o trong
    // giong AnimalWander nhung ban kinh rong hon (dan lang di dao pho).
    public class VillagerWander : MonoBehaviour
    {
        public Color Hair, Skin, Shirt;
        public Color Pants = new Color(0.3f, 0.28f, 0.35f);

        SpriteRenderer sr;
        Vector3 target;
        const float Speed = 0.9f;
        const int Radius = 3;
        float pauseT;
        float walkFrameT;
        int walkFrame;
        int facing;
        bool flip;
        float breathePhase;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            target = transform.position;
            pauseT = Random.Range(0f, 2f);
            breathePhase = Random.Range(0f, 4f);
        }

        void Update()
        {
            Vector3 d = target - transform.position;
            bool moving = pauseT <= 0f && d.magnitude >= 0.06f;

            walkFrameT += Time.deltaTime;
            if (walkFrameT > 0.28f) { walkFrameT = 0f; walkFrame = 1 - walkFrame; }

            if (pauseT > 0f)
            {
                pauseT -= Time.deltaTime;
            }
            else if (d.magnitude < 0.06f)
            {
                pauseT = Random.Range(1.5f, 4f);
                var c = MapBuilder.WorldToCell(transform.position + new Vector3(0f, 0.3f, 0f));
                for (int i = 0; i < 8; i++)
                {
                    var nc = c + new Vector2Int(Random.Range(-Radius, Radius + 1), Random.Range(-Radius, Radius + 1));
                    if (nc != c && MapBuilder.IsWalkable(nc))
                    {
                        target = MapBuilder.CellToWorld(nc) - new Vector3(0f, 0.5f, 0f);
                        break;
                    }
                }
            }
            else
            {
                facing = Mathf.Abs(d.x) > Mathf.Abs(d.y) ? 2 : (d.y > 0f ? 1 : 0);
                if (facing == 2) flip = d.x < 0f;
                transform.position += d.normalized * Speed * Time.deltaTime;
                sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10f);
            }

            int frame = moving ? (1 + walkFrame) : (((Time.time + breathePhase) % 2.4f) > 1.2f ? 3 : 0);
            sr.sprite = PixelArt.Character(Hair, Skin, Shirt, Pants, moving ? facing : 0, frame);
            sr.flipX = moving && facing == 2 && flip;
        }
    }

    // Con vat di lang thang ngau nhien trong pham vi nho, ton trong o bi chan.
    // Moi loai co bang cau hinh rieng (frame, toc do, thoi gian dung...) de de mo rong.
    public class AnimalWander : MonoBehaviour
    {
        public AnimalKind Kind;

        struct Cfg
        {
            public int frames;
            public float frameDur;
            public float speed;
            public float pauseMin, pauseMax;
            public int radius;
            public System.Func<int, Sprite> sprite;
        }

        static readonly Dictionary<AnimalKind, Cfg> cfgs = new Dictionary<AnimalKind, Cfg>
        {
            { AnimalKind.Chicken, new Cfg { frames = 4, frameDur = 0.35f, speed = 0.7f, pauseMin = 0.6f, pauseMax = 2.8f, radius = 2, sprite = PixelArt.Chicken } },
            { AnimalKind.Dog,     new Cfg { frames = 4, frameDur = 0.16f, speed = 1.4f, pauseMin = 0.4f, pauseMax = 2.0f, radius = 3, sprite = PixelArt.Dog } },
            { AnimalKind.Cow,     new Cfg { frames = 2, frameDur = 0.6f,  speed = 0.4f, pauseMin = 1.5f, pauseMax = 4.0f, radius = 2, sprite = PixelArt.Cow } },
            { AnimalKind.Sheep,   new Cfg { frames = 2, frameDur = 0.6f,  speed = 0.45f,pauseMin = 1.2f, pauseMax = 3.5f, radius = 2, sprite = PixelArt.Sheep } },
            { AnimalKind.Pig,     new Cfg { frames = 2, frameDur = 0.5f,  speed = 0.6f, pauseMin = 1.0f, pauseMax = 3.0f, radius = 2, sprite = PixelArt.Pig } },
            { AnimalKind.Cat,     new Cfg { frames = 2, frameDur = 0.9f,  speed = 0.8f, pauseMin = 1.8f, pauseMax = 4.5f, radius = 2, sprite = PixelArt.Cat } },
        };

        SpriteRenderer sr;
        Cfg cfg;
        Vector3 target;
        float frameT;
        int frame;
        float pauseT;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            cfg = cfgs[Kind];
            target = transform.position;
            pauseT = Random.Range(0f, 2f);
        }

        void Update()
        {
            frameT += Time.deltaTime;
            if (frameT > cfg.frameDur)
            {
                frameT = 0f;
                frame = (frame + 1) % cfg.frames;
                sr.sprite = cfg.sprite(frame);
            }

            if (pauseT > 0f) { pauseT -= Time.deltaTime; return; }

            Vector3 d = target - transform.position;
            if (d.magnitude < 0.06f)
            {
                pauseT = Random.Range(cfg.pauseMin, cfg.pauseMax);
                var c = MapBuilder.WorldToCell(transform.position + new Vector3(0f, 0.3f, 0f));
                for (int i = 0; i < 8; i++)
                {
                    var nc = c + new Vector2Int(Random.Range(-cfg.radius, cfg.radius + 1), Random.Range(-cfg.radius, cfg.radius + 1));
                    if (nc != c && MapBuilder.IsWalkable(nc))
                    {
                        target = MapBuilder.CellToWorld(nc) - new Vector3(0f, 0.5f, 0f);
                        break;
                    }
                }
            }
            else
            {
                transform.position += d.normalized * cfg.speed * Time.deltaTime;
                if (Mathf.Abs(d.x) > 0.05f) sr.flipX = d.x < 0f;
                sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10f);
            }
        }
    }

    // Dung dua nhe ngon cay trong theo gio (nghieng quanh pivot day, lech pha ngau nhien).
    public class CropSway : MonoBehaviour
    {
        public float Amplitude = 4f; // do nghieng toi da (Euler Z)
        public float Speed = 1.6f;
        float phase;

        void Awake() { phase = Random.Range(0f, Mathf.PI * 2f); }

        void Update()
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * Speed + phase) * Amplitude);
        }
    }

    // Gon nhe mau tile nuoc de tao cam giac gon song.
    public class WaterAnim : MonoBehaviour
    {
        SpriteRenderer sr;
        Color baseColor;
        float phase;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            baseColor = sr.color;
            phase = Random.Range(0f, Mathf.PI * 2f);
        }

        void Update()
        {
            float t = 0.85f + 0.15f * Mathf.Sin(Time.time * 1.5f + phase);
            sr.color = new Color(baseColor.r * t, baseColor.g * t, baseColor.b * t, baseColor.a);
        }
    }
}
