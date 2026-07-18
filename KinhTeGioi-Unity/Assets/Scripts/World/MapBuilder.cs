using System.Collections.Generic;
using UnityEngine;

namespace KTG
{
    // Doc luoi ASCII cua MapDef, sinh toan bo tile/prop/NPC bang PixelArt, va dung bang walkability.
    public static class MapBuilder
    {
        public static int Width, Height;
        public static bool[,] Blocked;
        public static readonly Dictionary<Vector2Int, InteractableInfo> Interactables = new Dictionary<Vector2Int, InteractableInfo>();
        public static Vector2Int SpawnCell;

        public static Vector3 CellToWorld(Vector2Int c) => new Vector3(c.x + 0.5f, -(c.y + 0.5f), 0f);
        public static Vector2Int WorldToCell(Vector2 pos) => new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(-pos.y));

        public static bool IsWalkable(Vector2Int c)
        {
            if (c.x < 0 || c.x >= Width || c.y < 0 || c.y >= Height) return false;
            return !Blocked[c.x, c.y];
        }

        static readonly List<Vector2Int> extraBlocked = new List<Vector2Int>();

        public static void Build(int mapIndex, Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
                Object.Destroy(root.GetChild(i).gameObject);
            Interactables.Clear();
            extraBlocked.Clear();

            var map = GameContent.Maps[mapIndex];
            var rows = map.Rows;
            Height = rows.Length;
            Width = rows[0].Length;
            Blocked = new bool[Width, Height];
            SpawnCell = new Vector2Int(Width / 2, Height / 2);

            var flame = GameContent.TorchColor(mapIndex);
            bool crystalHere = GameManager.Instance != null && GameManager.Instance.Save.crystal[mapIndex];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char ch = rows[y][x];
                    var cell = new Vector2Int(x, y);
                    Vector3 pos = CellToWorld(cell);
                    bool walkable = true;

                    if (ch == '#')
                    {
                        SpawnTile(root, pos, PixelArt.Wall(new Color(0.25f, 0.24f, 0.3f), new Color(0.15f, 0.14f, 0.2f)), -10000);
                        walkable = false;
                    }
                    else if (ch == '~')
                    {
                        var w = SpawnTile(root, pos, PixelArt.Water(map.Water, Color.Lerp(map.Water, Color.white, 0.3f), x * 13 + y * 7), -9000);
                        w.gameObject.AddComponent<WaterAnim>();
                        walkable = false;
                    }
                    else
                    {
                        var groundColor = ch == ',' ? map.Grass : map.Ground;
                        SpawnTile(root, pos, PixelArt.Ground(groundColor, Color.Lerp(groundColor, Color.black, 0.25f), x * 31 + y * 17), -10000);

                        // Rai trang tri ngau nhien (deterministic theo o + map):
                        // co xanh: hoa + cum co day dac; duong dat: soi da + co thua
                        int h = (x * 73856093 ^ y * 19349663 ^ mapIndex * 83492791) & 0x7fffffff;
                        int roll = h % 100;
                        if (ch == ',')
                        {
                            if (roll < 10)
                                SpawnTile(root, pos, PixelArt.Flora(h), -9500);
                            else if (roll < 28)
                                SpawnTile(root, pos, PixelArt.Tuft(h, Color.Lerp(map.Grass, Color.black, 0.18f)), -9500);
                        }
                        else if (ch == '.')
                        {
                            if (roll < 6)
                                SpawnTile(root, pos, PixelArt.Pebble(h), -9500);
                            else if (roll < 12)
                                SpawnTile(root, pos, PixelArt.Tuft(h, Color.Lerp(map.Grass, map.Ground, 0.4f)), -9500);
                        }
                    }

                    switch (ch)
                    {
                        case 'T':
                            SpawnProp(root, pos, PixelArt.Tree(new Color(0.25f, 0.5f, 0.25f), new Color(0.4f, 0.28f, 0.18f)));
                            walkable = false;
                            break;

                        case 't':
                        {
                            var torch = SpawnProp(root, pos, PixelArt.Torch(flame));
                            var glowGO = new GameObject("Glow");
                            glowGO.transform.SetParent(torch.transform, false);
                            glowGO.transform.localPosition = new Vector3(0f, 0.5f, -0.01f);
                            glowGO.transform.localScale = Vector3.one * 1.6f;
                            var glowSr = glowGO.AddComponent<SpriteRenderer>();
                            glowSr.sprite = PixelArt.Glow(flame, 64);
                            glowSr.sortingOrder = torch.sortingOrder - 1;
                            torch.gameObject.AddComponent<TorchFlicker>().Glow = glowSr;
                            walkable = false;
                            break;
                        }

                        case '*':
                        {
                            var pedestal = SpawnProp(root, pos, PixelArt.Pedestal());
                            var crystalGO = new GameObject("CrystalGlow");
                            crystalGO.transform.SetParent(root, false);
                            crystalGO.transform.position = pedestal.transform.position + new Vector3(0f, 0.9f, 0f);
                            var csr = crystalGO.AddComponent<SpriteRenderer>();
                            csr.sprite = PixelArt.Crystal(crystalHere ? new Color(0.5f, 0.9f, 1f) : new Color(0.45f, 0.45f, 0.55f, 0.6f));
                            csr.sortingOrder = pedestal.sortingOrder + 1;
                            crystalGO.AddComponent<Bobber>().Amplitude = 0.06f;
                            Interactables[cell] = new InteractableInfo(InteractableKind.Pedestal, '*');
                            walkable = false;
                            break;
                        }

                        case 's':
                            SpawnProp(root, pos, PixelArt.Stall(new Color(0.7f, 0.3f, 0.25f)));
                            walkable = false;
                            break;

                        case 'o':
                            SpawnProp(root, pos, PixelArt.Coral(new Color(0.6f, 0.3f, 0.8f), x * 7 + y * 11));
                            walkable = false;
                            break;

                        case 'b':
                            SpawnProp(root, pos, PixelArt.Barrel());
                            walkable = false;
                            break;

                        case 'c':
                            SpawnProp(root, pos, PixelArt.Crate());
                            walkable = false;
                            break;

                        case 'r':
                            SpawnProp(root, pos, PixelArt.Rock(x * 5 + y * 13));
                            walkable = false;
                            break;

                        case 'u':
                            SpawnProp(root, pos, PixelArt.Bush(Color.Lerp(map.Grass, new Color(0.2f, 0.5f, 0.25f), 0.5f), x * 3 + y * 19));
                            walkable = false;
                            break;

                        case 'w':
                            SpawnProp(root, pos, PixelArt.Well());
                            walkable = false;
                            break;

                        case 'n':
                            SpawnProp(root, pos, PixelArt.Sign());
                            walkable = false;
                            break;

                        case 'H':
                        {
                            // Nha rong 3 o, cao 2 o: chan them cac o xung quanh o neo
                            var roofC = Color.Lerp(map.Ground, new Color(0.72f, 0.28f, 0.22f), 0.65f);
                            SpawnProp(root, pos, PixelArt.House(new Color(0.87f, 0.80f, 0.64f), roofC));
                            walkable = false;
                            extraBlocked.Add(new Vector2Int(x - 1, y));
                            extraBlocked.Add(new Vector2Int(x + 1, y));
                            extraBlocked.Add(new Vector2Int(x - 1, y - 1));
                            extraBlocked.Add(new Vector2Int(x, y - 1));
                            extraBlocked.Add(new Vector2Int(x + 1, y - 1));
                            break;
                        }

                        case 'F':
                            SpawnProp(root, pos, PixelArt.Fence());
                            walkable = false;
                            break;

                        case 'k':
                        case 'd':
                        {
                            var animal = SpawnProp(root, pos, ch == 'k' ? PixelArt.Chicken(0) : PixelArt.Dog(0));
                            animal.gameObject.AddComponent<AnimalWander>().Kind = ch == 'k' ? 0 : 1;
                            break; // o van di duoc — con vat tu tranh duong
                        }

                        case 'L':
                        {
                            var spark = SpawnProp(root, pos, PixelArt.Sparkle());
                            var glowGO = new GameObject("Glow");
                            glowGO.transform.SetParent(spark.transform, false);
                            glowGO.transform.localPosition = new Vector3(0f, 0.35f, 0f);
                            var glowSr = glowGO.AddComponent<SpriteRenderer>();
                            glowSr.sprite = PixelArt.Glow(new Color(0.5f, 0.9f, 1f, 0.45f), 64);
                            glowSr.sortingOrder = spark.sortingOrder - 1;
                            spark.gameObject.AddComponent<TorchFlicker>().Glow = glowSr;
                            Interactables[cell] = new InteractableInfo(InteractableKind.Lore, 'L');
                            walkable = false;
                            break;
                        }

                        case 'X':
                        {
                            var boss = SpawnProp(root, pos, PixelArt.Character(new Color(0.15f, 0.1f, 0.15f), new Color(0.85f, 0.7f, 0.6f), new Color(0.5f, 0.15f, 0.55f), new Color(0.25f, 0.1f, 0.3f)));
                            boss.transform.localScale = Vector3.one * 1.6f;
                            SpawnShadowAt(root, boss.transform.position, boss.sortingOrder);
                            var bossIdle = boss.gameObject.AddComponent<NpcIdle>();
                            bossIdle.Hair = new Color(0.15f, 0.1f, 0.15f);
                            bossIdle.Skin = new Color(0.85f, 0.7f, 0.6f);
                            bossIdle.Shirt = new Color(0.5f, 0.15f, 0.55f);
                            bossIdle.Pants = new Color(0.25f, 0.1f, 0.3f);
                            Interactables[cell] = new InteractableInfo(InteractableKind.Boss, 'X');
                            walkable = false;
                            break;
                        }

                        case '<':
                            Interactables[cell] = new InteractableInfo(InteractableKind.PortalBack, '<');
                            SpawnTile(root, pos, PixelArt.Glow(new Color(0.9f, 0.8f, 0.3f, 0.55f), 48), -8000);
                            break;

                        case '>':
                            Interactables[cell] = new InteractableInfo(InteractableKind.PortalForward, '>');
                            SpawnTile(root, pos, PixelArt.Glow(new Color(0.3f, 0.85f, 0.9f, 0.55f), 48), -8000);
                            break;

                        case 'P':
                            SpawnCell = cell;
                            break;

                        default:
                            if (map.Npcs.TryGetValue(ch, out var npc))
                            {
                                var body = SpawnProp(root, pos, PixelArt.Character(npc.Hair, npc.Skin, npc.Shirt, new Color(0.3f, 0.28f, 0.35f)));
                                SpawnShadowAt(root, body.transform.position, body.sortingOrder);
                                var idle = body.gameObject.AddComponent<NpcIdle>();
                                idle.Hair = npc.Hair; idle.Skin = npc.Skin; idle.Shirt = npc.Shirt;
                                Interactables[cell] = new InteractableInfo(InteractableKind.Npc, ch);
                                walkable = false;
                            }
                            break;
                    }

                    Blocked[x, y] = !walkable;
                }
            }

            // Ap dung cac o bi chan them (than nha chiem nhieu o)
            foreach (var c in extraBlocked)
                if (c.x >= 0 && c.x < Width && c.y >= 0 && c.y < Height)
                    Blocked[c.x, c.y] = true;

            // Hieu ung moi truong (la bay / buom / dom dom) — con cua root, tu huy khi doi map
            var fxGO = new GameObject("AmbientFX");
            fxGO.transform.SetParent(root, false);
            fxGO.AddComponent<AmbientFX>().MapIndex = mapIndex;
        }

        static SpriteRenderer SpawnTile(Transform root, Vector3 pos, Sprite sprite, int order)
        {
            var go = new GameObject("tile");
            go.transform.SetParent(root, false);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = order;
            return sr;
        }

        // Sprite pivot-bottom: neo tai canh duoi cua o (pos la tam o).
        static SpriteRenderer SpawnProp(Transform root, Vector3 pos, Sprite sprite)
        {
            var go = new GameObject("prop");
            go.transform.SetParent(root, false);
            go.transform.position = pos - new Vector3(0f, 0.5f, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = Mathf.RoundToInt(-go.transform.position.y * 10f);
            return sr;
        }

        static void SpawnShadowAt(Transform root, Vector3 pos, int aboveOrder)
        {
            var go = new GameObject("Shadow");
            go.transform.SetParent(root, false);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PixelArt.Shadow();
            sr.sortingOrder = aboveOrder - 1;
        }
    }
}
