# Kế hoạch chi tiết: HD-2D qua URP

**Mục tiêu:** Biến Econia từ 2D phẳng thành 2D "nhìn như 3D" (HD-2D) —
ánh sáng 2D động, đổ bóng, bloom, depth of field, và (tùy chọn) camera nghiêng diorama.

**Môi trường:** Unity 2022.3.62f3 LTS · URP 2D · Windows.

---

## 0. Bối cảnh kỹ thuật (đã khảo sát qua code graph)

| Yếu tố | Hiện trạng | Ảnh hưởng khi lên URP |
|--------|-----------|------------------------|
| Render pipeline | Built-in RP, chưa có URP | Phải cài URP + tạo 2D Renderer asset + gán vào Graphics/Quality |
| Khởi tạo | Toàn bộ dựng bằng code ([Bootstrap.cs](../KinhTeGioi-Unity/Assets/Scripts/Core/Bootstrap.cs)) | Thêm đèn global + Volume + bật post trong code, không cần kéo-thả |
| Camera | Orthographic, pixel-perfect ([CameraFollow](../KinhTeGioi-Unity/Assets/Scripts/World/WorldComponents.cs)) | Giữ ortho cho Phase A-C; chỉ Phase D (tùy chọn) đụng phối cảnh |
| Sprite | `Sprite.Create` + `Sprites-Default`, `FilterMode.Point` | Vẫn render (unlit) trong URP → **không vỡ**; cần vật liệu Lit để nhận đèn |
| Sort | Theo trục Y (`-y*10`) trong [MapBuilder](../KinhTeGioi-Unity/Assets/Scripts/World/MapBuilder.cs) | Giữ nguyên |
| Prop có sẵn ánh sáng giả | Torch/Crystal/Portal/Window đã có sprite `Glow` | Thay/bổ sung bằng `Light2D` thật |

**Điểm rủi ro cao nhất:** đổi Scriptable Render Pipeline là thay đổi **toàn repo** về mặt hình ảnh
→ bắt buộc làm trên branch riêng, nghiệm thu kỹ, có đường lùi.

---

## Phase A — Nền tảng URP (bắt buộc, rủi ro: CAO)

**Kết quả mong đợi:** game chạy y hệt như cũ nhưng đã ở trên URP 2D (chưa có hiệu ứng mới).
Đây là "checkpoint an toàn" trước khi thêm đèn.

1. `git checkout -b feature/hd2d-urp`. Sao lưu `ProjectSettings/GraphicsSettings.asset`
   và `QualitySettings.asset`.
2. Cài **Universal RP** (`com.unity.render-pipelines.universal`, bản 14.0.x khớp Unity 2022.3)
   qua Package Manager → sẽ tự thêm vào `Packages/manifest.json`.
3. Tạo asset pipeline: `Assets/Create > Rendering > URP Asset (with 2D Renderer)`.
   Đặt trong `Assets/Settings/` → sinh ra 1 `UniversalRenderPipelineAsset` + 1 `Renderer2DData`.
4. Gán pipeline vào **Project Settings > Graphics** và **mọi mức Quality**.
5. Trong `Bootstrap.Init`: thêm `camGO.AddComponent<UniversalAdditionalCameraData>()`
   (URP thường tự thêm, nhưng thêm tường minh để chắc chắn cho camera tạo-bằng-code).
6. **Nghiệm thu A:** mở Unity, Play qua đủ 5 map. Sprite/UI render đúng, không hồng, không mất tile.
   Chụp màn hình từng map làm mốc so sánh "before".

> Nếu bất kỳ sprite nào hồng: gán `Sprites-Default` (URP) làm vật liệu chia sẻ cho world sprites
> tại chỗ `PixelArt.Make(...)`.

---

## Phase B — Vật liệu Lit + đèn 2D (rủi ro: TRUNG BÌNH)

**Kết quả mong đợi:** thế giới có chiều sâu ánh sáng — vũng sáng ấm quanh đuốc, crystal phát sáng thật,
mỗi map một tông ánh sáng riêng (chợ ban ngày ấm, thung lũng lạnh/tím, cung điện trang nghiêm).

1. Tạo **vật liệu Sprite-Lit chia sẻ** (`Sprite-Lit-Default`) cho **world sprites**
   (tile + prop + nhân vật). **UI giữ nguyên** (Canvas, không nhận đèn 2D).
   Gán material này trong `PixelArt` hoặc ngay tại `MapBuilder.SpawnTile/SpawnProp`.
   - Lưu ý: sprite `Glow`, hạt `SparkFly`, đom đóm nên **giữ Unlit + additive** để vẫn tự phát sáng.
2. Thêm **Global Light2D** (ánh sáng nền toàn map) trong `Bootstrap` hoặc `MapBuilder.Build`,
   màu + cường độ đọc từ bảng cấu hình mới (xem mục "Dữ liệu cấu hình" bên dưới).
3. Thêm **Point Light2D** cho các nguồn sáng:
   - `t` (đuốc): điểm sáng màu `flame`, bán kính ~3 ô, nhấp nháy.
   - `*` (crystal): điểm sáng lạnh khi đã thu thập.
   - `H` cửa sổ nhà: điểm sáng vàng ấm nhỏ.
   - `<`/`>` (portal): điểm sáng theo màu portal.
   - `o`/`L` (nấm/sparkle huyền ảo ở valley): điểm sáng nhẹ.
4. Sửa [`TorchFlicker`](../KinhTeGioi-Unity/Assets/Scripts/World/WorldComponents.cs) để nhấp nháy
   **cường độ `Light2D`** (giữ cả glow sprite hoặc thay). Tạo `LightFlicker` nếu cần tách bạch.
5. **Nghiệm thu B:** 5 map, kiểm tra tông sáng khác biệt rõ giữa các vùng, đuốc tạo vũng sáng,
   pixel-perfect còn nguyên (không nhòe/nhấp nháy khi di chuyển).

---

## Phase C — Post-processing: Bloom + Vignette + Color + DoF (rủi ro: TRUNG BÌNH)

**Kết quả mong đợi:** đây là bước tạo "chất điện ảnh" HD-2D rõ rệt nhất.

1. Tạo **Global Volume** + **Volume Profile** (đặt trong `Assets/Settings/`), thêm override:
   - **Bloom** — làm crystal/đuốc/portal/glow "rực" lên (cần bật HDR trên pipeline asset;
     có thể phải đẩy màu glow lên >1 / dùng HDR color để bloom bắt sáng).
   - **Vignette** — tối 4 góc, dồn chú ý vào giữa (thay thế `PixelArt.Vignette` cũ — xem bước 2).
   - **Tonemapping** (Neutral hoặc ACES) + **Color Adjustments** — chỉnh tông màu tổng thể theo map.
   - **Depth of Field** (Gaussian/Bokeh, mức nhẹ) — tạo cảm giác tilt-shift/diorama.
     Với camera ortho hiệu ứng tinh tế; là mấu chốt của cảm giác "3D thu nhỏ".
     Nhòe của DoF là **nhòe có chủ đích** (xem định nghĩa pixel-perfect trong `02-ai-rules.md` A.3)
     — mức độ phải được người dùng duyệt khi nghiệm thu.
2. **Gỡ vignette UI cũ:** vignette hiện tại là một `Image` vẽ bằng `PixelArt.Vignette()`
   trong `UIManager` (canvas `ScreenSpaceOverlay` render SAU post-processing, nên URP Vignette
   không thể đè lên nó → nếu không gỡ sẽ bị 2 lớp vignette chồng nhau). Tắt/xóa Image này khi
   bật URP Vignette. Chạy `impact` trên hàm dựng HUD của `UIManager` trước khi sửa.
3. Bật `renderPostProcessing = true` trên `UniversalAdditionalCameraData` trong `Bootstrap`.
4. Cân nhắc **per-map Volume Profile** hoặc override runtime để mỗi vùng một không khí riêng.
5. **Nghiệm thu C:** so sánh before/after từng map. Kiểm tra hiệu năng (FPS) — bloom + DoF tốn GPU.
   - Chữ UI: **đã xác nhận qua code** — canvas dùng `RenderMode.ScreenSpaceOverlay`
     (`UIFactory.CreateCanvas`) nên UI render sau toàn bộ post, bloom/DoF/vignette của URP
     **không bao giờ** làm mờ UI. Chỉ cần kiểm tra nhanh bằng mắt, không cần điều tra thêm.
   - Xác nhận không còn vignette chồng lớp (bước 2 đã gỡ bản cũ).

---

## Phase D — (TÙY CHỌN, STRETCH) Khối 3D & camera diorama (rủi ro: CAO)

Chỉ làm sau khi A-C ổn định và bạn muốn đẩy xa hơn. Có thể làm độc lập, từng phần:

- **D1 — Đổ bóng 2D thật:** thêm `ShadowCaster2D` cho prop/nhân vật + `Light2D` bật đổ bóng
  → bóng động theo nguồn sáng (đắt về hiệu năng, cân nhắc số lượng).
- **D2 — Normal map procedural:** sinh normal map từ sprite (Sobel trên kênh height/alpha) trong
  `PixelArt` để đèn tạo khối sáng-tối trên bề mặt sprite. Phức tạp nhất, hiệu quả "khối 3D" cao nhất.
- **D3 — Camera phối cảnh + billboard:** đổi camera sang perspective nghiêng nhẹ, sprite billboard
  → hiệu ứng mô hình diorama đúng chất Octopath.
  **⚠ Rủi ro cao:** phá vỡ toán pixel-perfect và giả định Y-sort trong `CameraFollow` + `MapBuilder`.
  Phải viết lại logic clamp/snap camera. Cân nhắc kỹ trước khi chọn.

---

## Dữ liệu cấu hình ánh sáng (thêm mới)

Thêm cấu hình đèn theo map, song song với `GameContent.TorchColor(mapIndex)` đã có:

- Mở rộng `MapDef` trong [`DataModels.cs`](../KinhTeGioi-Unity/Assets/Scripts/Data/DataModels.cs)
  với: `Color AmbientColor`, `float AmbientIntensity` (và tùy chọn tham số DoF/bloom/tint).
- Hoặc thêm bảng `MapLighting` trong [`GameContent.cs`](../KinhTeGioi-Unity/Assets/Scripts/Data/GameContent.cs)
  giống pattern `TorchColor`. **Ưu tiên cách này** để không phình `MapDef` và giữ mọi dữ liệu ở một nơi.

Gợi ý tông theo map: `market` ấm sáng · `guild` trung tính hơi ám khói · `bank` lạnh xám-lam ·
`valley` tím/huyền ảo tối · `palace` lạnh trang nghiêm, tương phản cao.

---

## Danh sách file dự kiến chạm

| File | Thay đổi | Ghi chú GitNexus (chạy impact trước) |
|------|----------|--------------------------------------|
| `Packages/manifest.json` | + package URP | không phải symbol |
| `ProjectSettings/GraphicsSettings.asset`, `QualitySettings.asset` | gán URP asset | không phải symbol |
| `Assets/Settings/*` (MỚI) | URP asset, Renderer2D, Volume Profile, material Lit | asset mới |
| [`Core/Bootstrap.cs`](../KinhTeGioi-Unity/Assets/Scripts/Core/Bootstrap.cs) | camera data, global light, volume, bật post | `impact Bootstrap.Init` |
| [`Core/PixelArt.cs`](../KinhTeGioi-Unity/Assets/Scripts/Core/PixelArt.cs) | gán material Lit; (D2) normal map | `impact PixelArt.Make` |
| [`World/MapBuilder.cs`](../KinhTeGioi-Unity/Assets/Scripts/World/MapBuilder.cs) | thêm Light2D vào prop; ambient theo map | `impact MapBuilder.Build`, `SpawnProp`, `SpawnTile` |
| [`World/WorldComponents.cs`](../KinhTeGioi-Unity/Assets/Scripts/World/WorldComponents.cs) | TorchFlicker → Light2D; (D3) CameraFollow | `impact TorchFlicker.Update`, `CameraFollow.Apply` |
| [`UI/UIManager.cs`](../KinhTeGioi-Unity/Assets/Scripts/UI/UIManager.cs) | (Phase C) gỡ Image vignette cũ (`PixelArt.Vignette`) khi bật URP Vignette | `impact` hàm dựng HUD chứa vignette |
| [`Data/GameContent.cs`](../KinhTeGioi-Unity/Assets/Scripts/Data/GameContent.cs) | bảng `MapLighting` | `impact TorchColor` (thêm cạnh nó) |
| [`Data/DataModels.cs`](../KinhTeGioi-Unity/Assets/Scripts/Data/DataModels.cs) | (nếu chọn) field ánh sáng trong `MapDef` | `impact MapDef` |

---

## Nghiệm thu tổng thể (Definition of Done)

- [ ] Game chạy trên URP 2D, đủ 5 map, không lỗi render, không hồng sprite.
- [ ] Mỗi map có tông ánh sáng riêng; đuốc/crystal/portal phát sáng thật (Light2D).
- [ ] Bloom + Vignette + DoF bật, tạo chất HD-2D; UI vẫn sắc nét (UI ở `ScreenSpaceOverlay`,
      nằm ngoài post — chỉ kiểm tra nhanh); vignette UI cũ đã gỡ, không chồng 2 lớp.
- [ ] Pixel-perfect còn nguyên: không nhòe/nhấp nháy **do snapping sai** khi camera di chuyển.
      Nhòe có chủ đích của DoF không tính là vi phạm, nhưng mức độ phải được người dùng duyệt.
- [ ] Hiệu năng: **≥ 60 FPS** trên máy đang dev ở cả 5 map (đo FPS trước/sau để so sánh).
      Nếu không đạt → giảm/tắt DoF trước, bloom sau, và báo người dùng quyết định đánh đổi.
- [ ] Có bộ ảnh before/after cho cả 5 map.
- [ ] `gitnexus_detect_changes()` xác nhận phạm vi thay đổi đúng dự kiến trước mỗi commit.

## Rollback

- Mọi thay đổi nằm trên `feature/hd2d-urp`. Lùi bằng cách bỏ branch.
- Điểm lùi an toàn từng phase: commit riêng sau mỗi Phase (A, B, C) đã nghiệm thu.
- Nếu URP gây sự cố không mong muốn: revert `GraphicsSettings.asset` + `manifest.json` về Built-in RP.
