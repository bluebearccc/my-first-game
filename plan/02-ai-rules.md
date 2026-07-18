# RULE cho AI khi làm việc trong workspace này

> **Bắt buộc đọc trước khi sửa bất cứ file nào.** Rule này áp dụng cho mọi trợ lý AI
> (Claude Code, Cursor, v.v.) khi thực thi kế hoạch HD-2D trong [`01-hd2d-urp-migration-plan.md`](01-hd2d-urp-migration-plan.md).
> Rule ở đây **bổ sung**, không thay thế, quy tắc GitNexus trong `CLAUDE.md`/`AGENTS.md` gốc.

## A. Nguyên tắc dự án (không được vi phạm)

1. **Procedural-only.** Game sinh 100% đồ họa/âm thanh bằng code, **không import asset ngoài**
   (ảnh, model, sprite sheet, âm thanh tải về). URP, đèn, material, Volume Profile là **cấu hình rendering**
   — được phép. Nhưng KHÔNG được thay sprite procedural bằng file ảnh.
2. **Không trộn loại thay đổi.** Một commit chỉ làm một việc: hoặc đổi pipeline, hoặc thêm đèn,
   hoặc thêm post, hoặc sửa nội dung. Không gộp "đổi graphics" với "sửa hội thoại/câu đố".
3. **Pixel-perfect bất khả xâm phạm.** Không phá logic pixel-snapping trong `CameraFollow`
   (pixel-perfect làm thủ công qua `orthographicSize = Screen.height / (2*PPU*zoom)`,
   KHÔNG dùng Pixel Perfect Camera component). Sau mỗi thay đổi hình ảnh phải kiểm tra
   sprite không nhòe/nhấp nháy khi di chuyển.
   **Định nghĩa rõ:** "pixel-perfect" = không nhòe/nhấp nháy **do snapping/scaling sai**.
   Nhòe **có chủ đích** của Depth of Field (Phase C) không tính là vi phạm rule này —
   nhưng mức độ DoF phải được người dùng duyệt khi nghiệm thu, không tự quyết.
4. **Giữ tiếng Việt trong nội dung & comment.** Code comment và text game bằng tiếng Việt như hiện có.
5. **Không đổi cân bằng gameplay/nội dung học thuật** khi đang làm task đồ họa. Nội dung kinh tế chính trị
   là phần cốt lõi — chỉ sửa khi có yêu cầu riêng.

## B. Quy trình an toàn khi sửa code (kết hợp GitNexus)

1. **Branch trước.** Mọi việc HD-2D làm trên `feature/hd2d-urp`, KHÔNG commit thẳng lên `main`.
2. **Impact analysis trước khi sửa mọi symbol** (theo `CLAUDE.md`):
   chạy `gitnexus_impact({target, direction:"upstream"})`, báo blast radius cho người dùng.
   **Cảnh báo nếu risk HIGH/CRITICAL** và chờ xác nhận trước khi sửa.
   Các symbol trọng yếu của task này: `Bootstrap.Init`, `PixelArt.Make`, `MapBuilder.Build`,
   `MapBuilder.SpawnProp`, `MapBuilder.SpawnTile`, `CameraFollow.Apply`, `TorchFlicker.Update`, `TorchColor`,
   và (Phase C) hàm dựng HUD trong `UIManager` chứa Image vignette cũ.
3. **Không find-and-replace để đổi tên symbol** — dùng `gitnexus_rename`.
4. **Trước mỗi commit:** chạy `gitnexus_detect_changes()`, xác nhận chỉ chạm đúng symbol/luồng dự kiến.
5. **Commit theo phase.** Sau mỗi Phase (A/B/C) đã nghiệm thu → 1 commit rõ ràng làm điểm lùi.
6. Nếu GitNexus báo index stale: chạy `npx gitnexus analyze` trước.

## C. Nghiệm thu bắt buộc (không được bỏ qua)

1. **Test trên cả 5 map**: market → guild → bank → valley → palace. Không kết luận "xong" từ 1 map.
2. **Chụp before/after** mỗi map để so sánh, lưu vào nơi tạm (scratchpad), không commit ảnh vào repo trừ khi được yêu cầu.
3. **Kiểm tra bằng cách chạy thật** (skill `run` / mở Unity Play mode), không chỉ compile.
   Kiểm: render đúng, không hồng sprite, UI sắc nét, pixel-perfect còn, FPS chấp nhận được.
4. **Cập nhật [`03-progress.md`](03-progress.md)** sau mỗi phase: đã làm gì, kết quả nghiệm thu, việc còn lại.

## D. Ranh giới quyết định — khi nào PHẢI hỏi người dùng

- Trước khi **cài package mới** hoặc **đổi Scriptable Render Pipeline** của project (Phase A).
- Trước khi bắt đầu **Phase D** (camera phối cảnh/normal map/shadow) — rủi ro cao, đổi kiến trúc.
- Khi impact analysis trả về **HIGH/CRITICAL**.
- Khi phải đánh đổi rõ rệt (ví dụ hiệu năng giảm mạnh vì bloom/DoF).
- Khi một thay đổi làm hỏng pixel-perfect và không có cách giữ.

## E. Điều KHÔNG được làm

- ❌ Commit thẳng lên `main`.
- ❌ Sửa symbol mà chưa chạy `gitnexus_impact`.
- ❌ Import asset ảnh/âm thanh ngoài để "cho nhanh".
- ❌ Gộp nhiều phase vào một commit lớn khó lùi.
- ❌ Tự ý bắt đầu Phase D khi chưa hỏi.
- ❌ Kết luận hoàn thành khi mới test 1 map hoặc chỉ compile mà chưa chạy.
- ❌ Đổi nội dung học thuật/hội thoại/câu đố trong lúc làm task đồ họa.
