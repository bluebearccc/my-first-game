using UnityEngine;

namespace KTG
{
    public class PlayerController : MonoBehaviour
    {
        public static readonly Color Hair = new Color(0.35f, 0.22f, 0.12f);
        public static readonly Color Skin = new Color(0.87f, 0.68f, 0.55f);
        public static readonly Color Shirt = new Color(0.25f, 0.45f, 0.55f);
        public static readonly Color Pants = new Color(0.2f, 0.22f, 0.3f);

        public float Speed = 3.2f;
        public Vector2Int Facing = new Vector2Int(0, -1);

        SpriteRenderer body;
        Transform bodyT;
        float walkT;
        float interactCooldown;

        void Awake()
        {
            var bodyGO = new GameObject("Body");
            bodyGO.transform.SetParent(transform, false);
            bodyT = bodyGO.transform;
            body = bodyGO.AddComponent<SpriteRenderer>();
            body.sprite = PixelArt.Character(Hair, Skin, Shirt, Pants);
            Lighting2D.MakeLit(body); // nguoi choi nhan anh sang 2D nhu the gioi (Phase B)
            Hd2dView.StandUp(bodyT);  // D3: nhan vat "dung day" trong goc nhin diorama

            var shadowGO = new GameObject("Shadow");
            shadowGO.transform.SetParent(transform, false);
            var shadowSr = shadowGO.AddComponent<SpriteRenderer>();
            shadowSr.sprite = PixelArt.Shadow();
            shadowSr.sortingOrder = -1;
            body.sortingOrder = 0;
        }

        void Update()
        {
            interactCooldown -= Time.deltaTime;
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Explore) return;

            float ix = 0f, iy = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) ix -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) ix += 1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) iy += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) iy -= 1f;

            Vector2 dir = new Vector2(ix, iy);
            bool moving = dir.sqrMagnitude > 0.01f;
            if (moving)
            {
                dir.Normalize();
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) Facing = new Vector2Int(dir.x < 0f ? -1 : 1, 0);
                else if (Mathf.Abs(dir.y) > 0.01f) Facing = new Vector2Int(0, dir.y < 0f ? -1 : 1);

                MoveAxis(new Vector3(dir.x * Speed * Time.deltaTime, 0f, 0f));
                MoveAxis(new Vector3(0f, dir.y * Speed * Time.deltaTime, 0f));

                walkT += Time.deltaTime;
            }
            else
            {
                walkT = 0f;
            }
            UpdateSprite(moving);

            body.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10f) + 500;

            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)) && interactCooldown <= 0f)
            {
                TryInteract();
                interactCooldown = 0.3f;
            }
        }

        // Chu ky di bo 4 nhip [buoc trai, dung, buoc phai, dung]; dung yen thi tho nhe nhang.
        void UpdateSprite(bool moving)
        {
            int facingIdx = Facing.y < 0 ? 0 : (Facing.y > 0 ? 1 : 2);
            int frame;
            if (moving)
            {
                int step = (int)(walkT * 8f) % 4;
                frame = step == 0 ? 1 : (step == 2 ? 2 : 0);
            }
            else
            {
                frame = (Time.time % 2.4f) > 1.2f ? 3 : 0;
            }
            body.sprite = PixelArt.Character(Hair, Skin, Shirt, Pants, facingIdx, frame);
            body.flipX = facingIdx == 2 && Facing.x < 0;
        }

        void MoveAxis(Vector3 delta)
        {
            if (delta.sqrMagnitude < 0.0000001f) return;
            Vector3 target = transform.position + delta;
            if (CanStand(target)) transform.position = target;
        }

        bool CanStand(Vector3 pos)
        {
            const float half = 0.28f;
            Vector2[] corners =
            {
                new Vector2(pos.x - half, pos.y + 0.08f),
                new Vector2(pos.x + half, pos.y + 0.08f),
                new Vector2(pos.x - half, pos.y + 0.55f),
                new Vector2(pos.x + half, pos.y + 0.55f)
            };
            foreach (var c in corners)
                if (!MapBuilder.IsWalkable(MapBuilder.WorldToCell(c))) return false;
            return true;
        }

        void TryInteract()
        {
            Vector3 cellCenter = transform.position + new Vector3(0f, 0.5f, 0f);
            Vector3 checkPos = cellCenter + new Vector3(Facing.x, Facing.y, 0f);
            var cell = MapBuilder.WorldToCell(checkPos);
            if (MapBuilder.Interactables.TryGetValue(cell, out var info))
                GameManager.Instance.Interact(info.Kind, info.Code);
        }

        // pos = vi tri chan (canh duoi cua o), giong quy uoc dat prop trong MapBuilder.
        public void Warp(Vector3 feetWorldPos)
        {
            transform.position = feetWorldPos;
            walkT = 0f;
        }
    }
}
