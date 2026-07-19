# Kế hoạch: NPC & animation phụ · Đồng ruộng · Con vật nét hơn

**Mục tiêu:** Làm thế giới Econia sống động và "đầy đặn" hơn — thêm dân làng đi lại + NPC tán
gẫu, rải đồng ruộng vào các map có sẵn, và vẽ lại/mở rộng đàn con vật cho sắc nét, mượt hơn.

**Nguyên tắc bất di bất dịch (kế thừa [`02-ai-rules.md`](02-ai-rules.md)):**
- **Procedural-only** — mọi sprite vẽ 100% bằng code trong `PixelArt`, KHÔNG import asset ngoài.
- Làm trên branch `feature/hd2d-urp`, **không** commit thẳng `main`.
- **Không trộn loại thay đổi trong 1 commit** → chia commit theo từng nhóm (A/B/C bên dưới).
- **Pixel-perfect**: giữ 16 PPU cho world sprite. "Nét hơn" = vẽ nhiều texel hơn ở cùng 16 PPU
  (con vật to hơn chút, không phá lưới pixel) — KHÔNG đổi PPU, KHÔNG đụng `CameraFollow`.
- **KHÔNG đổi nội dung học thuật** (hội thoại kinh tế chính, puzzle, boss). NPC/thoại thêm mới
  chỉ là flavor vô thưởng vô phạt, KHÔNG vào `Flow`, KHÔNG thêm puzzle.
- **Impact analysis trước khi sửa mọi symbol**; nghiệm thu đủ **5 map**; cập nhật
  [`03-progress.md`](03-progress.md) sau mỗi nhóm.

**Quyết định phạm vi (đã chốt với người dùng 2026-07-19):**
- NPC: **cả hai** — dân làng trang trí (đi lại, không thoại) + vài NPC flavor (thoại ngắn).
- Đồng ruộng: **rải mảnh ruộng vào 5 map có sẵn** (thuần trang trí). *Khu nông trại riêng = việc
  tương lai, KHÔNG làm trong plan này* (đụng Flow/Objectives → cần duyệt riêng).
- Con vật: **vẽ lại chi tiết hơn (giữ pixel-perfect) + thêm frame mượt + thêm loài mới**.

---

## 0. Bối cảnh kỹ thuật (đã khảo sát qua code)

| Yếu tố | Hiện trạng |
|--------|-----------|
| Sinh sprite | [`PixelArt.cs`](../KinhTeGioi-Unity/Assets/Scripts/Core/PixelArt.cs) — hàm tĩnh, cache theo key, `NewTex/Rect/P/OutlineTex/Make` |
| Dựng map | [`MapBuilder.Build`](../KinhTeGioi-Unity/Assets/Scripts/World/MapBuilder.cs): đọc lưới ASCII, `switch(ch)` → `SpawnTile`/`SpawnProp` |
| Con vật | `PixelArt.Chicken` (12×12, 2 frame), `Dog` (16×12, 2 frame); ký tự `k`/`d`; hành vi [`AnimalWander`](../KinhTeGioi-Unity/Assets/Scripts/World/WorldComponents.cs) |
| NPC | `NpcDef{Hair,Skin,Shirt}` → `PixelArt.Character` (16×24, facing×frame); [`NpcIdle`](../KinhTeGioi-Unity/Assets/Scripts/World/WorldComponents.cs) thở + quay mặt; gắn `Interactables` → hội thoại |
| Animation phụ | `AmbientFX` (lá/đom đóm/bướm), `Bobber`, `TorchFlicker`, `WaterAnim`, `SparkFly` |
| Hội thoại | `GameContent.GetDialogue` → nếu NPC không thuộc `Flow` (không "active") thì tự rơi về `FlavorDialogues` → NPC flavor **không cần** đụng puzzle/main dialogue |

**Bảng ký tự đang dùng** (để tránh trùng khi thêm mới):
`#  ,  .  ~  T  t  *  s  o  b  c  r  u  w  n  H  F  k  d  L  X  <  >  P` và NPC hoa `A B C X`.

**Ký tự MỚI đề xuất** (chưa trùng — case-sensitive nên hoa/thường khác nhau):

| Ký tự | Ý nghĩa | Nhóm |
|-------|---------|------|
| `=` | Đất ruộng đã cày (luống) — loại **tile** | B |
| `i` | Luống cây trồng (rau/lúa/ngô theo map) — **prop** | B |
| `j` | Bù nhìn rơm (scarecrow) | B |
| `h` | Đống rơm (haystack) | B |
| `m` | Bò (cow) | A |
| `e` | Cừu (sheep) | A |
| `g` | Lợn (pig) | A |
| `y` | Mèo (cat) | A |
| `V` | Dân làng đi lại (ambient, không thoại) | C |
| `D` `E` `G` | NPC flavor (thoại ngắn, không vào Flow) | C |

---

## Nhóm A — Con vật: nét hơn + mượt hơn + thêm loài (rủi ro: TRUNG BÌNH)

**Kết quả mong đợi:** gà/chó chi tiết & mượt hơn hẳn; có thêm bò, cừu, lợn, mèo lang thang
làm map "có hồn".

1. **Vẽ lại `PixelArt.Chicken`**: 12×12 → **16×16**; thêm mào 3 pixel, cánh có lớp lông,
   viền `OutlineTex` cho nổi khối, mắt + bóng dưới bụng. Nâng **2 → 4 frame**: mổ-xuống,
   trung gian, ngẩng, thi thoảng vỗ cánh.
2. **Vẽ lại `PixelArt.Dog`**: 16×12 → **20×14**; tách rõ 4 chân (chu kỳ chạy), tai cụp, mõm,
   đổ bóng thân. Nâng **2 → 4 frame** (trot cycle + vẫy đuôi).
3. **Thêm sprite loài mới trong `PixelArt`** (mỗi loài ≥2 frame, pivot đáy, có `OutlineTex`):
   `Cow(frame)`, `Sheep(frame)`, `Pig(frame)`, `Cat(frame)` (mèo hay ngồi cuộn — idle nhẹ).
4. **Tổng quát hoá `AnimalWander`**: đổi `Kind` (int cứng 0/1) sang **enum/bảng cấu hình**
   `{spriteFn, frameCount, frameDur, speed, pauseRange, wanderRadius}` để mọi loài dùng chung.
   Bò/cừu chậm gặm cỏ; mèo hay dừng; gà mổ thóc; chó nhanh. Giữ né ô bị chặn như hiện tại.
5. **`MapBuilder`**: thêm `case 'm'/'e'/'g'/'y'` → spawn prop + `AnimalWander` tương ứng
   (giống `case 'k'/'d'` sẵn có). Ô vẫn đi được (con vật tự tránh đường).

**⚠ Impact bắt buộc trước khi sửa:** `PixelArt.Chicken`, `PixelArt.Dog` (đổi kích thước sprite),
`AnimalWander.Update`/`AnimalWander.Awake` (đổi chữ ký Kind), `PixelArt.Make` (dùng chung).
Báo blast radius; nếu HIGH/CRITICAL → dừng hỏi.

**Nghiệm thu A:** 5 map — gà/chó rõ nét & mượt, loài mới đi lại tự nhiên, không kẹt tường,
không nhòe/nhấp nháy khi di chuyển (pixel-perfect còn). Sortingorder theo Y đúng (không đè sai).

---

## Nhóm B — Đồng ruộng rải vào map có sẵn (rủi ro: TRUNG BÌNH)

**Kết quả mong đợi:** rìa các map (chợ, guild — nền cỏ) có luống cày, cây trồng đung đưa,
bù nhìn, đống rơm → cảm giác vùng quê canh tác.

1. **Sprite mới trong `PixelArt`:**
   - `TilledSoil(seed)` — 16×16 tile: nền đất nâu + vằn luống ngang (giống `Ground` nhưng có rãnh cày).
   - `Crop(type, seed)` — prop 16×20 pivot đáy: luống 3–4 nhánh cây; `type` chọn theo map
     (rau xanh / lúa vàng / ngô). Tùy chọn: tham số "độ chín" cho đa dạng.
   - `Scarecrow()` — 16×22: cọc + áo rơm + mũ, tay dang ngang.
   - `Haystack(seed)` — 16×14: đống rơm vàng bó tròn.
2. **`MapBuilder`:**
   - `case '='` → `SpawnTile(TilledSoil)` (tile, walkable=false hoặc true tùy ý — mặc định
     **không đi được** để giống ruộng thật; chốt khi nghiệm thu).
   - `case 'i'` → `SpawnProp(Crop)` + component `CropSway` (mục 3), walkable=false.
   - `case 'j'` → `SpawnProp(Scarecrow)` + `Shadow2D.AddCaster`, walkable=false.
   - `case 'h'` → `SpawnProp(Haystack)` + `Shadow2D.AddCaster`, walkable=false.
   - Cây trồng/đất ruộng **nhận đèn 2D** như prop thường (qua `Lighting2D.MakeLit`, đã có sẵn
     trong `SpawnProp/SpawnTile`) → ăn khớp tông sáng từng map (Phase B HD-2D).
3. **Animation phụ `CropSway`** (component mới): đung đưa nhẹ ngọn cây theo gió
   (nghiêng/scale nhẹ quanh pivot đáy, lệch pha ngẫu nhiên) — như `Bobber` nhưng lắc ngang.
4. **Đặt ruộng vào lưới ASCII** trong [`GameContent`](../KinhTeGioi-Unity/Assets/Scripts/Data/GameContent.cs)
   `BuildMap0/1` (rìa cỏ trống, tránh đè đường `.`/prop/đường đi bắt buộc). Ví dụ market:
   khoảnh cỏ góc dưới-trái/dưới-phải (đã có hàng rào `F`) → thêm 2–3 luống `=`/`i` + 1 `j`, 1 `h`.
   Có thể điểm thêm cho valley/palace nếu hợp bối cảnh (tùy chọn, chốt khi nghiệm thu).

**⚠ Impact bắt buộc:** `MapBuilder.Build` (thêm case — thuần cộng thêm, từng ghi nhận HIGH ở
Phase B), `MapBuilder.SpawnTile`/`SpawnProp`, `PixelArt.Make`. Chạy `gitnexus_detect_changes()`
trước commit để chắc chỉ chạm đúng luồng dựng map.

**Nghiệm thu B:** map có ruộng render đúng (không hồng, không lủng tile), walkability đúng
(không kẹt người chơi trong luống), cây đung đưa tự nhiên, bóng bù nhìn/đống rơm đổ đúng,
4 map còn lại **không đổi** ngoài dự kiến.

---

## Nhóm C — NPC: dân làng trang trí + NPC flavor + animation phụ (rủi ro: TRUNG BÌNH)

**Kết quả mong đợi:** phố xá có người qua lại; vài người bấm nói được câu tán gẫu; nông dân
cuốc ruộng — thế giới bớt "đứng hình".

1. **Dân làng đi lại (ambient, không tương tác) — ký tự `V`:**
   - Component mới `VillagerWander`: dùng `PixelArt.Character` với **frame 1/2 (bước đi)** khi
     di chuyển, frame 0/3 khi dừng; lang thang trong bán kính nhỏ, né ô chặn (tái dùng logic
     `AnimalWander`). Màu tóc/da/áo random từ một bảng palette để mỗi người một vẻ.
   - `MapBuilder case 'V'` → spawn Character + `VillagerWander` + shadow; **KHÔNG** thêm vào
     `Interactables` (không thoại). Ô đi được.
2. **NPC flavor (thoại ngắn) — ký tự `D`/`E`/`G`:**
   - Thêm `NpcDef` cho từng ký tự trong `map.Npcs` (như A/B/C) NHƯNG **không đưa vào `Flow`**.
     → `GetDialogue` tự rơi về `FlavorDialogues` (main không bao giờ active) → an toàn, không
     đụng tiến trình puzzle/crystal.
   - Thêm `FlavorDialogues[Key(map, 'D')] = ...` 1–2 câu vô thưởng vô phạt (đúng không khí từng
     map, tiếng Việt). Reuse case `default` sẵn có trong `MapBuilder` (đã handle mọi NPC letter).
   - Đặt 1–2 NPC flavor mỗi map ở chỗ hợp lý (gần sạp, giếng, ruộng...).
3. **Animation phụ cho NPC (tùy chọn, làm nếu còn thời gian):**
   - `NpcIdle`: thêm **chớp mắt** thi thoảng (đổi frame 1 tick).
   - `FarmerChore`: nông dân đứng cạnh ruộng vung cuốc theo nhịp (biến thể của `NpcIdle`).
   - Khói bếp từ nhà `H`: component `Chimney` nhả hạt khói mờ bay lên (thêm loài hạt vào
     `AmbientFX` hoặc component riêng). → làm nhà "có người ở".

**⚠ Impact bắt buộc:** `NpcIdle.Update` (nếu thêm chớp mắt), `MapBuilder.Build` (case `V`),
`PixelArt.Character` (nếu tinh chỉnh frame đi bộ — HIGH vì nhiều caller), `GameContent` builders
(thêm NpcDef/Flavor — không đụng Main/Flow/Puzzle). **Không** dùng find-and-replace đổi tên.

**Nghiệm thu C:** dân làng đi lại không kẹt/không giật; NPC flavor bấm ra đúng câu tán gẫu,
KHÔNG kích hoạt puzzle/crystal; tiến trình game 5 map **không đổi**; dấu tiếng Việt hiển thị đủ.

---

## Danh sách file dự kiến chạm

| File | Nhóm | Thay đổi |
|------|------|----------|
| [`Core/PixelArt.cs`](../KinhTeGioi-Unity/Assets/Scripts/Core/PixelArt.cs) | A,B,C | Vẽ lại Chicken/Dog; +Cow/Sheep/Pig/Cat; +TilledSoil/Crop/Scarecrow/Haystack |
| [`World/WorldComponents.cs`](../KinhTeGioi-Unity/Assets/Scripts/World/WorldComponents.cs) | A,B,C | Tổng quát hoá `AnimalWander`; +`CropSway`, +`VillagerWander`, (+`FarmerChore`/`Chimney`, chớp mắt `NpcIdle`) |
| [`World/MapBuilder.cs`](../KinhTeGioi-Unity/Assets/Scripts/World/MapBuilder.cs) | A,B,C | +case `m/e/g/y` (vật), `=/i/j/h` (ruộng), `V` (dân làng) |
| [`Data/GameContent.cs`](../KinhTeGioi-Unity/Assets/Scripts/Data/GameContent.cs) | B,C | Rải ruộng vào lưới map0/1; +NpcDef & FlavorDialogues cho `D/E/G` |

*Không đụng:* `Bootstrap`, `CameraFollow`, `Lighting2D`, `UIManager`, pipeline/URP asset, nội
dung Main dialogue/Puzzle/Boss/Journal.

---

## Thứ tự thực hiện & commit (mỗi nhóm = 1+ commit riêng, có điểm lùi)

1. **A** (con vật) — độc lập, rủi ro gọn nhất, thấy kết quả ngay. → commit `feat: con vật nét hơn + loài mới`.
2. **B** (đồng ruộng) — dựa trên PixelArt/MapBuilder đã quen tay ở A. → commit `feat: đồng ruộng trang trí`.
3. **C** (NPC + animation phụ) — cuối, vì đụng GameContent nhiều nhất. → commit(s) riêng cho
   dân làng, cho NPC flavor, cho animation phụ (không gộp).

Trước **mỗi** commit: `gitnexus_detect_changes()` xác nhận phạm vi đúng.

---

## Nghiệm thu tổng thể (Definition of Done)

- [ ] Chạy thật (Unity Play / harness `HD2DTestRunner`), **đủ 5 map**, không chỉ compile.
- [ ] Con vật (cũ + mới) nét, mượt, đi lại tự nhiên, không kẹt tường, Y-sort đúng.
- [ ] Đồng ruộng render đúng, walkability đúng, cây đung đưa, bóng đổ đúng; 4 map không liên quan không đổi.
- [ ] Dân làng đi lại mượt; NPC flavor ra thoại tán gẫu, KHÔNG kích hoạt puzzle/crystal; tiến trình game nguyên vẹn.
- [ ] **Pixel-perfect còn nguyên** (không nhòe/nhấp nháy do snapping); 16 PPU không đổi.
- [ ] **FPS ≥ 60** cả 5 map (đo trước/sau; con vật/NPC đông có thể tốn CPU → giới hạn số lượng nếu tụt).
- [ ] Ảnh before/after (scratchpad, không commit vào repo trừ khi được yêu cầu).
- [ ] Cập nhật [`03-progress.md`](03-progress.md) sau mỗi nhóm.

## Ranh giới — PHẢI hỏi người dùng trước khi

- Tạo **khu/map nông trại mới** (đụng Flow/Objectives/cổng) — ngoài phạm vi plan này.
- Thêm **puzzle/nội dung học thuật** cho NPC mới (plan này chỉ flavor).
- Cài package mới / đổi PPU / đụng camera.
- Khi impact trả về **HIGH/CRITICAL**, hoặc phải đánh đổi FPS rõ rệt.

## Rollback

- Mọi thay đổi trên `feature/hd2d-urp`. Mỗi nhóm A/B/C là 1 commit → lùi từng phần dễ dàng.
- Sprite/animation thuần cộng thêm; gỡ case trong `MapBuilder` + ký tự trong `GameContent` là hoàn tác sạch.
</content>
</invoke>
