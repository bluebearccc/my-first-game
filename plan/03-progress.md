# Nhật ký tiến độ HD-2D

> AI cập nhật file này sau mỗi phase. Định dạng: ngày · phase · việc đã làm · kết quả nghiệm thu · còn lại.

## Trạng thái tổng quan

| Phase | Mô tả | Trạng thái |
|-------|-------|-----------|
| A | Nền tảng URP 2D (cài package, tạo asset, gán pipeline) | ✅ Xong & đã nghiệm thu (commit `60627f7`) |
| B | Vật liệu Lit + đèn 2D (global + point light theo map) | ✅ Xong & đã nghiệm thu (commit `d9ccc7b`) |
| C | Post-processing: Bloom + Vignette + Color + DoF | ✅ Xong & đã nghiệm thu (commit `d6260da`) |
| D | (Tùy chọn) Đổ bóng / normal map / camera diorama | 🟡 D3 đã dựng thử trên `exp/hd2d-phase-d` (commit `c43f4ee`, nghiệm thu 5/5 map, FPS 154–164) — chờ người dùng duyệt để merge; D1/D2 chưa làm |
| NPC/đồng ruộng/con vật (kế hoạch `04-npc-dongruong-convat-plan.md`) | Nhóm A ✅, Nhóm B ✅, Nhóm C (NPC) ✅ — cả 3 nhóm xong & đã nghiệm thu |

Chú thích: ⬜ chưa · 🟡 đang làm · ✅ xong & đã nghiệm thu · ⏸ tạm dừng chờ quyết định

## Mốc setup

- [x] Đã tạo branch `feature/hd2d-urp` (2026-07-18)
- [x] Đã sao lưu `GraphicsSettings.asset` + `QualitySettings.asset` vào scratchpad (2026-07-18)
- [x] Đã có bộ ảnh "before" của 5 map (chụp trên `main`, lưu scratchpad `shots/before/`)

---

## Log

### 2026-07-19 · Nhóm C (kế hoạch `04-npc-dongruong-convat-plan.md`) · Dân làng đi lại + NPC flavor (XONG, nghiệm thu 5/5 map)

**Đã làm:**
- `WorldComponents.cs`: thêm `VillagerWander` (component mới, không có caller cũ nên không cần impact
  chặn) — dùng `PixelArt.Character` có sẵn, frame 1/2 khi đi, frame 0/3 (thở) khi dừng, quay mặt theo
  hướng di chuyển, tái dùng logic tìm-ô-trống của `AnimalWander` với bán kính rộng hơn (3 ô).
- `MapBuilder.Build`: thêm `case 'V'` — spawn `Character` (màu tóc/da/áo random theo bảng palette,
  seed theo ô để ổn định qua các lần build) + `VillagerWander` + shadow; **KHÔNG** thêm vào
  `Interactables` (không thoại), ô vẫn đi được.
- NPC flavor (`D`) — **không cần sửa `MapBuilder`**: nhánh `default` (xử lý `map.Npcs`) đã tổng quát
  sẵn từ trước, chỉ cần thêm dữ liệu ở `GameContent.cs`.
- `GameContent.cs`: thêm `m.Npcs['D']` + `FlavorDialogues[Key(mapIndex,'D')]` (1-2 câu tán gẫu tiếng
  Việt) cho map0-3 (market/guild/bank/valley); rải 1 `V` (dân làng) + 1 `D` (NPC flavor) mỗi map ở ô
  cỏ trống. **Bỏ qua map4 (palace)** — phòng ngai vàng là khu vực boss/trang nghiêm, dân làng/tán gẫu
  không hợp bối cảnh (quyết định phạm vi, không đụng `Flow`/`Objectives`).
  Xác nhận `D` không nằm trong bất kỳ `Flow` nào → `GetDialogue` luôn rơi về `FlavorDialogues`, không
  bao giờ kích hoạt `MainDialogues`/puzzle/crystal.

**Nghiệm thu (Unity 2022.3.62f3, Play thật qua harness `HD2DTestRunner`, `-hd2dLabel groupC-after`):**
- Biên dịch sạch. `gitnexus_detect_changes()`: đúng 3 file dự kiến (`GameContent.cs`, `MapBuilder.cs`,
  `WorldComponents.cs`) — `PixelArt.cs` không đổi (tái dùng `Character` có sẵn).
- 5/5 map render đúng, không hồng/lủng tile: map0=162.7fps, map1=164.0fps, map2=164.8fps,
  map3=163.4fps, map4=165.2fps (chuẩn ≥60 ĐẠT).
- Quan sát ảnh: xuất hiện thêm dân làng đi lại + NPC flavor ở map0/1/2/3 so với ảnh nghiệm thu Nhóm
  A/B trước đó, không kẹt/không đè hình.
- Ảnh: scratchpad `shots/groupC-after/map0..4.png` + `summary.txt` (không commit vào repo).

**Còn lại:** chưa bấm nói chuyện tay với NPC `D` để nghe thoại thực tế (logic `GetDialogue` đã xác
minh qua code, rủi ro thấp vì `D` không có trong `Flow`). Animation phụ tuỳ chọn (chớp mắt `NpcIdle`,
`FarmerChore`, khói bếp `Chimney`) trong mục 3 của Nhóm C **chưa làm** — đánh giá là đủ với thời lượng
hiện tại, có thể làm thêm nếu người dùng muốn.

### 2026-07-19 · Nhóm B (kế hoạch `04-npc-dongruong-convat-plan.md`) · Đồng ruộng trang trí (XONG, nghiệm thu 5/5 map)

**Đã làm:**
- Impact trước khi sửa: `MapBuilder.Build` = LOW; `MapBuilder.SpawnTile`/`SpawnProp` = **HIGH** (đúng mẫu rủi ro
  plan đã lường trước, giống Phase B cũ và Nhóm A vừa làm) — tiếp tục với cùng mitigation (nghiệm thu 5 map).
- `PixelArt.cs`: thêm `TilledSoil(seed)` (tile 16×16, luống cày), `Crop(type, seed)` (prop 16×20, 3-4
  nhánh cây, type 0=rau/1=lúa/2=ngô theo `mapIndex % 3`), `Scarecrow()` (16×22), `Haystack(seed)` (16×14).
- `WorldComponents.cs`: thêm `CropSway` (nghiêng nhẹ quanh pivot đáy theo Perlin/sin, lệch pha ngẫu nhiên).
- `MapBuilder.Build`: thêm nhánh tile `case '='` (TilledSoil, không đi được) và case prop `i` (Crop +
  `CropSway`), `j` (Scarecrow + `Shadow2D.AddCaster`), `h` (Haystack + `Shadow2D.AddCaster`) — tất cả
  `walkable = false`, nhận đèn 2D qua `SpawnTile/SpawnProp` có sẵn (không cần code riêng).
- `GameContent.cs`: rải ruộng trang trí cạnh hàng rào có sẵn ở map0 (market, góc dưới trái/phải) và
  map1 (guild, cạnh nhà đầu map) — thuần trang trí, không đụng `Flow`/`Objectives`.

**Nghiệm thu (Unity 2022.3.62f3, Play thật qua harness `HD2DTestRunner`, `-hd2dLabel groupB-after`):**
- Biên dịch sạch. `gitnexus_detect_changes()`: đúng 4 file dự kiến (`PixelArt.cs`, `MapBuilder.cs`,
  `WorldComponents.cs`, `GameContent.cs`).
- 5/5 map render đúng, không hồng/lủng tile: map0=162.8fps, map1=164.1fps, map2=164.1fps,
  map3=164.1fps, map4=164.8fps (chuẩn ≥60 ĐẠT).
- Quan sát ảnh: luống cày (nâu, có rãnh) tách biệt rõ với cỏ xanh; bù nhìn rơm + đống rơm hiển thị
  đúng vị trí góc map0/map1; cây trồng có bóng đổ qua `Shadow2D`.
- Ảnh: scratchpad `shots/groupB-after/map0..4.png` + `summary.txt` (không commit vào repo).

**Còn lại:** chưa test bằng di chuyển thật xem người chơi có bị chặn đúng ở luống/prop mới hay không
(logic `walkable = false` đồng nhất với các prop khác nên tin cậy cao, nhưng chưa click-test tay).
Nhóm C (NPC/animation phụ) chưa làm.

### 2026-07-19 · Nhóm A (kế hoạch `04-npc-dongruong-convat-plan.md`) · Con vật nét hơn + thêm loài (XONG, nghiệm thu 5/5 map)

**Đã làm:**
- Impact trước khi sửa: `PixelArt.Chicken`/`PixelArt.Dog` = **HIGH** (gọi xuyên `MapBuilder.Build`/`AnimalWander.Update`,
  lan tới `ContinueGame`/`GoToMapRoutine`) — đã báo người dùng, người dùng xác nhận tiếp tục theo đúng
  mitigation đã dùng ở Phase B (nghiệm thu đủ 5 map). `AnimalWander` (class) = LOW. Không đụng `PixelArt.Make`.
- `PixelArt.Chicken`: 12×12 → **16×16**, thêm mào 3px, wattle, cánh lớp riêng, viền `OutlineTex`,
  4 frame (ngẩng/trung gian/cúi mổ/vỗ cánh).
- `PixelArt.Dog`: 16×12 → **20×14**, 4 chân tách rõ (chu kỳ trot), tai cụp, mõm, viền `OutlineTex`,
  4 frame (trot cycle + vẫy đuôi).
- Thêm `PixelArt.Cow/Sheep/Pig/Cat` (2 frame mỗi loài, pivot đáy, `OutlineTex`).
- `AnimalWander` (`WorldComponents.cs`): đổi `Kind` từ `int` cứng sang `enum AnimalKind` + bảng cấu hình
  `Cfg{frames, frameDur, speed, pauseMin/Max, radius, spriteFn}` dùng chung cho 6 loài.
- `MapBuilder.Build`: gộp case `k/d/m/e/g/y` dùng chung 1 khối spawn (switch chọn sprite/kind).
- `GameContent.cs`: rải thêm 4 con vật mới (`m/e/g/y`) vào map0 (market) và map1 (guild) để nghiệm thu
  thực tế — thuần trang trí, không đụng `Flow`/`Objectives`.

**Nghiệm thu (Unity 2022.3.62f3, Play thật qua harness `HD2DTestRunner`, `-hd2dLabel groupA-after`):**
- Biên dịch sạch (chỉ 2 warning cũ có sẵn, không liên quan thay đổi).
- `gitnexus_detect_changes()`: chỉ chạm đúng 3 file dự kiến (`PixelArt.cs`, `MapBuilder.cs`, `WorldComponents.cs`).
- 5/5 map render đúng, không hồng/lủng tile: map0=161.3fps, map1=155.5fps, map2=164.1fps,
  map3=153.4fps, map4=154.8fps (chuẩn ≥60 ĐẠT).
- Ảnh: scratchpad `shots/groupA-after/map0..4.png` + `summary.txt` (không commit vào repo).

**Còn lại:** Nhóm B (đồng ruộng) và Nhóm C (NPC/animation phụ) của kế hoạch `04-...` chưa làm.

### 2026-07-19 · UI sharpness (Hướng A) · Tinh chỉnh legacy UI cho chữ nét hơn (XONG, nghiệm thu 5/5 map)

**Vấn đề:** người dùng thấy UI chưa nét. Nguyên nhân ở cấu hình Canvas, không phải nội dung panel.

**Đã làm:**
- Impact `UIFactory.CreateCanvas` = **LOW** (chỉ `UIManager.Build` gọi); `UIFactory.CreateText` = **CRITICAL**
  (17 caller) → thiết kế né hoàn toàn, KHÔNG đụng `CreateText`.
- `UIFactory.CreateCanvas`: thêm `scaler.dynamicPixelsPerUnit = 2f` — tăng gấp đôi độ phân giải raster
  của glyph `Text` và hình 9-slice `Wood9`. Đây là toàn bộ thay đổi (1 dòng), procedural, không asset ngoài.
- Giữ nguyên `pixelPerfect`, `referenceResolution`, `matchWidthOrHeight`.

**Nghiệm thu (harness `HD2DTestRunner`, Play thật, before/after cùng điều kiện):**
- Chụp before/after main menu + 5 map. So sánh zoom 4×: chữ tiêu đề 56pt, HUD objective và dòng help
  chữ nhỏ đều **viền sắc gọn hơn, ít quầng nhòe**; dấu tiếng Việt đủ, không cắt chữ.
- Sprite thế giới vẫn pixel-perfect (dPU chỉ ảnh hưởng canvas UI, không đụng camera thế giới → rule A3 an toàn).
- 5 map render đúng tông, không hồng/mất tile; bloom còn. FPS after 161–165 (≥60 ĐẠT).
- Ảnh: scratchpad `shots/{ui-before,ui-after}/{menu,map0..4}.png` + `cmp_*` (không commit vào repo).

**Còn lại:** nếu người dùng thấy vẫn chưa đủ nét ở màn hình phân giải cao → cân nhắc Hướng B
(TextMeshPro/SDF), cần xin phép cài package theo ranh giới D.

### 2026-07-18 · Phase B + C · Đèn 2D + post-processing (XONG, đã nghiệm thu 5/5 map)

**Phase B (commit `d9ccc7b`):**
- `Lighting2D` (mới): material Sprite-Lit chia sẻ + tạo Global/Point Light2D bằng code.
- `GameContent`: `MapAmbientColor/Intensity` cạnh `TorchColor` — 5 tông riêng (chợ ấm sáng,
  guild trung tính, bank lạnh lam, valley tím tối, palace lạnh trang nghiêm).
- `MapBuilder`: tile/prop/NPC nhận đèn; glow + portal giữ unlit; point light cho đuốc (nhấp nháy
  qua `TorchFlicker.Light`), crystal đã thu thập, cửa sổ nhà, portal, nấm valley, sparkle.
- `PlayerController`: player nhận đèn. Impact đã chạy: SpawnProp/SpawnTile HIGH (thay đổi thuần
  cộng thêm, nghiệm thu đủ 5 map để bù rủi ro), còn lại LOW.

**Phase C (commit `d6260da`):**
- `Bootstrap`: Volume + profile dựng bằng code — Bloom (0.85, threshold 0.85), Vignette 0.28,
  Tonemapping Neutral, ColorAdjustments (sat +6, contrast +4), DoF Gaussian start 11
  (thế giới ở depth 10 → vẫn sắc nét; DoF "thật" cần Phase D3). `renderPostProcessing = true`.
- `UIManager.BuildHud`: đã gỡ Image vignette cũ — hết nguy cơ chồng 2 lớp vignette.

**Nghiệm thu (harness tự động `HD2DTestRunner`, Play thật qua 5 map):**
- before (main): ~163 FPS · after-A: hình y hệt, không hồng/mất tile · after-B: 5 tông sáng
  rõ rệt, đuốc có vũng sáng nhấp nháy · after-C: bloom rực glow/đom đóm, UI sắc nét.
- FPS after-C: 142–165 trên cả 5 map (chuẩn ≥60 ĐẠT). Lần đo 15 FPS ở after-A là artifact
  throttle của Editor khi mất focus — đã vá harness (InteractionMode No Throttling).
- Ảnh before/after: scratchpad `shots/{before,after-A,after-B,after-C}/map0..4.png`.

**Còn lại:** Phase D chờ quyết định của người dùng (không tự làm theo rule D).

### 2026-07-18 · Phase A · Cài URP + tạo asset + gán pipeline (XONG — nghiệm thu đạt, xem mục trên)

**Đã làm:**
- Tạo branch `feature/hd2d-urp`; sao lưu `GraphicsSettings.asset` + `QualitySettings.asset` (scratchpad).
- Impact `Bootstrap.Init` (GitNexus): risk **LOW**, 0 caller trực tiếp (entry point) → sửa an toàn.
- `Packages/manifest.json`: + `com.unity.render-pipelines.universal@14.0.12`.
- Viết `Assets/Editor/HD2DUrpSetup.cs` (menu `Tools/HD2D/Setup URP 2D`, idempotent, chạy được batchmode).
- Chạy Unity batchmode: package cài OK, **không lỗi compile**, script setup chạy thành công.
- Sinh `Assets/Settings/Renderer2D.asset` + `URP-HD2D.asset`; đã xác minh GUID pipeline gán đúng vào
  `GraphicsSettings.asset` và cả 6 mức Quality; pipeline asset tham chiếu đúng Renderer2D.
- `Bootstrap.cs`: thêm `UniversalAdditionalCameraData` tường minh cho camera tạo-bằng-code.

**Còn lại (nghiệm thu A — chưa làm):**
- [ ] Bộ ảnh "before" 5 map: chụp bằng cách checkout `main` (pipeline đổi chỉ nằm trên branch này).
- [ ] Mở Unity, Play qua đủ 5 map: sprite/UI đúng, không hồng, không mất tile; chụp ảnh "after Phase A".
- [ ] Sau nghiệm thu: `gitnexus_detect_changes()` rồi commit Phase A làm điểm lùi.
