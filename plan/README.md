# Plan: Nâng cấp đồ họa HD-2D (URP) cho Econia

Thư mục này chứa kế hoạch và quy tắc cho việc nâng cấp game **KinhTeGioi ("Econia")**
từ đồ họa 2D top-down phẳng (Built-in Render Pipeline) lên phong cách **HD-2D**
(2D nhìn như 3D) theo hướng của *Octopath Traveler / Triangle Strategy*:
sprite pixel-art đặt trong không gian có **ánh sáng 2D động, đổ bóng, bloom, depth of field**
và (tùy chọn) **camera phối cảnh nghiêng kiểu diorama**.

> Đây chính là "Mức 3" trong buổi trao đổi ban đầu — mức đẹp nhất, đáng đầu tư nhất,
> đánh đổi là phải **migrate project sang URP** (một lần).

## Mục lục

| File | Nội dung |
|------|----------|
| [`01-hd2d-urp-migration-plan.md`](01-hd2d-urp-migration-plan.md) | Kế hoạch chi tiết theo từng phase, file bị ảnh hưởng, tiêu chí nghiệm thu, rollback |
| [`02-ai-rules.md`](02-ai-rules.md) | **Rule bắt buộc cho mọi AI** làm việc trong workspace này khi thực thi kế hoạch |
| [`03-progress.md`](03-progress.md) | Nhật ký tiến độ — AI cập nhật sau mỗi phase |

## Nguyên tắc cốt lõi (đọc trước khi làm bất cứ gì)

1. **Triết lý gốc của game phải giữ:** *100% procedural, không asset ngoài.*
   Đồ họa sinh bằng code trong [`PixelArt.cs`](../KinhTeGioi-Unity/Assets/Scripts/Core/PixelArt.cs).
   URP/đèn/bloom là lớp **rendering** phủ lên, không thay thế cách sinh sprite.
2. **Không trộn "đổi pipeline" với "đổi nội dung"** trong cùng một commit.
3. **Pixel-perfect là bất khả xâm phạm** — logic pixel-snapping trong
   [`CameraFollow`](../KinhTeGioi-Unity/Assets/Scripts/World/WorldComponents.cs) phải còn nguyên tác dụng.
4. **Nghiệm thu trên cả 5 map** (market → guild → bank → valley → palace), không chỉ 1 map.
5. Tuân thủ **GitNexus workflow** (impact analysis trước khi sửa symbol) — xem `02-ai-rules.md`.

## Trạng thái hiện tại

- Branch làm việc dự kiến: `feature/hd2d-urp` (CHƯA tạo — bước 1 của Phase A sẽ tạo)
- Pipeline hiện tại: **Built-in RP** (`ProjectSettings/GraphicsSettings.asset` → `m_CustomRenderPipeline: {fileID: 0}`)
- Package URP: **chưa cài**
