# Kinh Tế Giới --- Mô tả Game (Phiên bản UI nâng cấp)

## Nền tảng phát triển

Game được xây dựng bằng **Unity** (2D URP). Toàn bộ hiệu ứng hình ảnh mô
tả trong tài liệu này (ánh sáng mềm, Bloom, thời tiết, chu kỳ ngày --
đêm, chuyển cảnh) đều dùng các tính năng có sẵn của Unity: URP 2D
Lighting cho ánh sáng và Bloom, Particle System cho mưa/nắng/lá rơi,
Cinemachine cho chuyển cảnh giữa các vùng, và UI Toolkit/uGUI cho toàn
bộ giao diện (menu, hội thoại, inventory, journal, quest). Chi tiết đầy
đủ về tech stack nằm trong tài liệu
*Kinh Tế Giới — Mô tả Game & Tech Stack*.

## Giới thiệu

**Kinh Tế Giới** là một game phiêu lưu kể chuyện 2D góc nhìn từ trên
xuống (top-down), lấy cảm hứng từ những tựa game indie hiện đại như
**Stardew Valley**, **Eastward** và **Sea of Stars**. Gameplay, cốt
truyện và nội dung giáo dục **được giữ nguyên**, nhưng toàn bộ phần giao
diện và trải nghiệm hình ảnh được thiết kế lại theo hướng hiện đại, sinh
động và hấp dẫn hơn.

Người chơi vào vai một sinh viên vô tình lạc vào thế giới Econia -- một
thế giới kỳ ảo mô phỏng nền kinh tế thị trường. Để trở về thế giới thực,
người chơi phải khám phá năm vùng đất, hoàn thành nhiệm vụ, giải các câu
đố và thu thập đủ năm **Economic Crystal**.

Điểm cốt lõi của trò chơi vẫn là: **học kinh tế chính trị thông qua trải
nghiệm**, thay vì đọc lý thuyết.

------------------------------------------------------------------------

# Phong cách hình ảnh

## Visual Style

-   Modern Stylized Pixel Art
-   Ánh sáng 2D mềm, hiệu ứng Bloom nhẹ
-   Màu sắc tươi sáng, nhiều chi tiết môi trường
-   Animation mượt cho nhân vật và NPC
-   Hiệu ứng thời tiết (mưa, nắng, lá rơi)
-   Chu kỳ ngày -- đêm
-   Hiệu ứng chuyển cảnh giữa các khu vực

Mỗi vùng đất vẫn giữ nguyên nội dung nhưng được nâng cấp về hình ảnh để
tạo cảm giác như đang khám phá một thế giới fantasy sống động.

------------------------------------------------------------------------

# Giao diện (UI)

## Menu chính

-   Background động
-   Logo phát sáng
-   Hiệu ứng Fade khi chuyển màn hình
-   Nút Start / Continue / Settings hiện đại

## Hội thoại

-   Chân dung NPC lớn
-   Khung hội thoại phong cách fantasy
-   Hiệu ứng chữ chạy
-   Lựa chọn A/B/C hiển thị dưới dạng nút đẹp

## Inventory

-   Hiển thị dạng lưới icon
-   Tooltip mô tả vật phẩm
-   Phân loại vật phẩm

## Journal

Thiết kế như một cuốn sách mở: - Trang trái: danh mục kiến thức - Trang
phải: nội dung, hình minh họa và ví dụ

## Quest UI

Danh sách nhiệm vụ hiển thị gọn ở góc màn hình với trạng thái hoàn thành
rõ ràng.

------------------------------------------------------------------------

# Gameplay (Giữ nguyên)

Gameplay vẫn giữ nguyên:

Khám phá bản đồ → Nói chuyện NPC → Nhận nhiệm vụ → Giải Puzzle → Đưa ra
lựa chọn → Học khái niệm kinh tế → Nhận Economic Crystal → Mở khóa vùng
tiếp theo.

Không thay đổi: - 5 vùng đất - Nội dung kinh tế chính trị - Quest -
Dialogue - Puzzle - Boss Debate - Journal - Reward

------------------------------------------------------------------------

# Trải nghiệm

Người chơi sẽ có cảm giác đang tham gia một cuộc phiêu lưu trong thế
giới fantasy thay vì một bài học trên lớp. Kiến thức được lồng ghép tự
nhiên vào nhiệm vụ, đối thoại và các câu đố, trong khi giao diện mới
mang lại trải nghiệm trực quan, hiện đại và dễ tiếp cận hơn.

Mục tiêu của phiên bản này là **nâng cao trải nghiệm người dùng mà không
thay đổi gameplay hay giá trị giáo dục của sản phẩm**.
