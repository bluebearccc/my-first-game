# Nhật ký tiến độ HD-2D

> AI cập nhật file này sau mỗi phase. Định dạng: ngày · phase · việc đã làm · kết quả nghiệm thu · còn lại.

## Trạng thái tổng quan

| Phase | Mô tả | Trạng thái |
|-------|-------|-----------|
| A | Nền tảng URP 2D (cài package, tạo asset, gán pipeline) | 🟡 Đang làm |
| B | Vật liệu Lit + đèn 2D (global + point light theo map) | ⬜ Chưa bắt đầu |
| C | Post-processing: Bloom + Vignette + Color + DoF | ⬜ Chưa bắt đầu |
| D | (Tùy chọn) Đổ bóng / normal map / camera diorama | ⬜ Chưa quyết định |

Chú thích: ⬜ chưa · 🟡 đang làm · ✅ xong & đã nghiệm thu · ⏸ tạm dừng chờ quyết định

## Mốc setup

- [x] Đã tạo branch `feature/hd2d-urp` (2026-07-18)
- [x] Đã sao lưu `GraphicsSettings.asset` + `QualitySettings.asset` vào scratchpad (2026-07-18)
- [ ] Đã có bộ ảnh "before" của 5 map

---

## Log

### 2026-07-18 · Phase A · Cài URP + tạo asset + gán pipeline (xong phần code, CHỜ nghiệm thu)

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
