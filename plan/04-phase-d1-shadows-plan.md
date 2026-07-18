# Kế hoạch Phase D1 — Đổ bóng động 2D (trên nền D3 diorama)

**Bối cảnh:** Người dùng đã duyệt D3 (camera diorama, commit `c43f4ee` trên `exp/hd2d-phase-d`)
và chọn "bước 2": giữ D3 + làm thêm **D1 — bóng đổ động**. Kế hoạch này nối tiếp
[`01-hd2d-urp-migration-plan.md`](01-hd2d-urp-migration-plan.md) mục Phase D; mọi rule trong
[`02-ai-rules.md`](02-ai-rules.md) vẫn áp dụng nguyên vẹn.

**Mục tiêu:** đứng cạnh đuốc, cây/nhà/thùng/NPC/người chơi đổ **bóng dài ngược hướng nguồn sáng**,
bóng **rung nhẹ theo nhịp lửa** (đuốc đã nhấp nháy cường độ qua `TorchFlicker.Light`).
Trên nền diorama D3, bóng động là mảnh ghép làm khung cảnh "có khối" rõ nhất.

---

## 0. Khảo sát kỹ thuật (đã xác minh trực tiếp trong URP 14.0.12 đã cài)

| Yếu tố | Kết luận đã xác minh | Hệ quả |
|--------|----------------------|--------|
| `Light2D.shadowIntensity` / `shadowsEnabled` / `ShadowCaster2D.selfShadows` | **API public, set được runtime** (`Light2D.cs:215,220`, `ShadowCaster2D.cs:100`) | Bật bóng bằng code thuần, không hack |
| `ShadowCaster2D.m_ShapePath` | private serialized; `shapePath` chỉ đọc; **nhưng khi rỗng sẽ tự khởi tạo hình vuông đơn vị** (`ShadowCaster2D.cs:176`) | Phương án chính: dùng shape mặc định + **transform scale** để chỉnh cỡ bóng. Reflection (`m_ShapePath` + `m_ShapePathHash`) chỉ là dự phòng nếu shape mặc định không đạt |
| Global Light2D không có hướng | Bóng chỉ có nghĩa với **point light** | Chỉ bật shadow trên đuốc/sparkle/crystal; KHÔNG bật trên global light |
| Chi phí: mỗi đèn-có-bóng × mỗi caster trong bán kính = 1 pass | Map nhiều đuốc (market/guild) là điểm nóng hiệu năng | Đo FPS từng map; có công tắc tắt nhanh |
| Sprite "đứng" (billboard D3) nhưng ShadowCaster2D tính trong mặt XY | Bóng là hình chiếu phẳng trên đất | Chấp nhận — bóng phẳng dưới chân hợp mắt diorama (kiểu Octopath) |

## 1. Trình tự thực hiện

### D1.0 — Chốt D3 (điều kiện tiên quyết)
1. Merge `exp/hd2d-phase-d` → `feature/hd2d-urp` (fast-forward, D3 đã nghiệm thu 5/5 map).
2. Làm D1 tiếp trên `feature/hd2d-urp`; giữ `exp/hd2d-phase-d` đến khi D1 xong rồi xóa.

### D1.1 — Helper `Shadow2D` (file mới, rủi ro THẤP)
- `Assets/Scripts/World/Shadow2D.cs`: static class, công tắc `public const bool Enabled`.
- `AddCaster(GameObject go, float width, float height)`: thêm `ShadowCaster2D`,
  `selfShadows = false`, cỡ bóng qua localScale của GO con "ShadowShape" (shape mặc định vuông đơn vị).
- **Smoke test ngay bước này:** 1 caster + 1 đuốc trên map 0, xác nhận bóng hiện đúng
  trước khi lan ra mọi prop. Nếu shape mặc định không sinh bóng → chuyển dự phòng reflection.

### D1.2 — Bật bóng trên nguồn sáng (sửa `Lighting2D`, rủi ro THẤP)
- `Lighting2D.AddPoint` thêm tham số `bool castShadows = false`;
  đuốc/sparkle/crystal gọi với `true`, `shadowIntensity ≈ 0.75`.
- Cửa sổ nhà/portal/nấm: **không** bật (đèn trang trí, tiết kiệm hiệu năng).

### D1.3 — Gắn caster cho vật thể (sửa `MapBuilder` + `PlayerController`, rủi ro TB)
- Prop khối lớn: Tree, House, Stall, Barrel, Crate, Rock, Well, Sign, Fence → caster theo cỡ prop.
- Nhân vật: NPC, boss, player → caster hẹp (~0.5 ô) dưới chân; giữ blob shadow cũ (contact shadow).
- Trang trí nhỏ (Flora/Tuft/Pebble/con vật) → KHÔNG caster.
- Impact đã có từ trước: `SpawnProp` HIGH (thay đổi cộng thêm), `PlayerController.Awake` LOW.
  Chạy lại `gitnexus_impact` cho symbol nào chưa từng phân tích trước khi sửa.

### D1.4 — Nghiệm thu & tuning (bắt buộc đủ 5 map)
- Harness `HD2DTestRunner` chụp `after-D1`, so với `after-D3`.
- Kiểm: bóng đổ ngược hướng đuốc và rung theo lửa; không artifact (bóng cắt sprite, viền răng cưa lạ);
  UI không ảnh hưởng; **FPS ≥ 60 cả 5 map** (baseline hiện tại 154–164).
- Nếu FPS tụt < 60: giảm caster (chỉ prop trong bán kính ~1.2× outer radius của đèn),
  rồi mới cân nhắc giảm số đèn có bóng. Báo người dùng nếu phải đánh đổi.
- Commit 1 lần sau nghiệm thu (chạy `gitnexus_detect_changes()` trước), cập nhật `03-progress.md`.

## 2. Danh sách file dự kiến chạm

| File | Thay đổi | Impact cần chạy |
|------|----------|-----------------|
| `World/Shadow2D.cs` (MỚI) | helper caster + công tắc | file mới, không cần |
| `World/Lighting2D.cs` | `AddPoint` thêm `castShadows` | `impact AddPoint` |
| `World/MapBuilder.cs` | caster cho prop; bật bóng đèn đuốc/sparkle | đã có (HIGH — cộng thêm) |
| `Player/PlayerController.cs` | caster cho player | đã có (LOW) |
| `plan/03-progress.md` | nhật ký | docs |

## 3. Rủi ro & đường lùi

- **Shape mặc định không hoạt động runtime** → dự phòng reflection `m_ShapePath`/`m_ShapePathHash`
  (đã xác định đúng tên field trong 14.0.12; pin version trong manifest nên không sợ lệch).
- **FPS tụt mạnh** → thang giảm: bớt caster → bớt đèn có bóng → `Shadow2D.Enabled = false`.
- **Bóng xấu/rối trên diorama** → chỉnh `shadowIntensity` (0.5–0.8) từng map; tệ nhất tắt công tắc.
- Rollback tổng: mọi thứ nằm sau `Shadow2D.Enabled` + 1 commit riêng → revert 1 phát là sạch.

## 4. Definition of Done (D1)

- [ ] Bóng động đổ đúng hướng từ đuốc/sparkle/crystal, rung theo flicker, trên cả 5 map.
- [ ] Không artifact; UI sắc nét; blob shadow cũ và bóng động không "đánh nhau" khó chịu.
- [ ] FPS ≥ 60 cả 5 map (so baseline after-D3 154–164, ghi số cụ thể).
- [ ] Công tắc `Shadow2D.Enabled = false` tắt sạch toàn bộ trong 1 dòng.
- [ ] Ảnh so sánh after-D3 vs after-D1 đủ 5 map; `03-progress.md` cập nhật.
