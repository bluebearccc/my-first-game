# Nhật ký tiến độ HD-2D

> AI cập nhật file này sau mỗi phase. Định dạng: ngày · phase · việc đã làm · kết quả nghiệm thu · còn lại.

## Trạng thái tổng quan

| Phase | Mô tả | Trạng thái |
|-------|-------|-----------|
| A | Nền tảng URP 2D (cài package, tạo asset, gán pipeline) | ✅ Xong & đã nghiệm thu (commit `60627f7`) |
| B | Vật liệu Lit + đèn 2D (global + point light theo map) | ✅ Xong & đã nghiệm thu (commit `d9ccc7b`) |
| C | Post-processing: Bloom + Vignette + Color + DoF | ✅ Xong & đã nghiệm thu (commit `d6260da`) |
| D | (Tùy chọn) Đổ bóng / normal map / camera diorama | 🟡 D3 đã dựng thử trên `exp/hd2d-phase-d` (commit `c43f4ee`, nghiệm thu 5/5 map, FPS 154–164) — chờ người dùng duyệt để merge; D1/D2 chưa làm |

Chú thích: ⬜ chưa · 🟡 đang làm · ✅ xong & đã nghiệm thu · ⏸ tạm dừng chờ quyết định

## Mốc setup

- [x] Đã tạo branch `feature/hd2d-urp` (2026-07-18)
- [x] Đã sao lưu `GraphicsSettings.asset` + `QualitySettings.asset` vào scratchpad (2026-07-18)
- [x] Đã có bộ ảnh "before" của 5 map (chụp trên `main`, lưu scratchpad `shots/before/`)

---

## Log

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
