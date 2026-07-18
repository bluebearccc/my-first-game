# Kinh Tế Giới

Game phiêu lưu — kể chuyện 2D top-down. Đồ án môn Kinh tế chính trị Mác - Lênin.

Người chơi vào vai một sinh viên lạc vào Econia — thế giới kỳ ảo mô phỏng nền
kinh tế thị trường — khám phá **5 vùng đất**, thu thập đủ **5 Economic Crystal**
để tìm đường về nhà. Cả game xoay quanh một nguyên lý duy nhất: **cạnh tranh
sinh ra độc quyền, nhưng độc quyền không thủ tiêu cạnh tranh mà làm nó đa dạng
và gay gắt hơn** (theo lý luận của V.I. Lênin).

Toàn bộ game được sinh **hoàn toàn bằng code** — mọi sprite pixel art, UI, hiệu
ứng âm thanh và nhạc nền đều được tạo lúc chạy, không cần asset ngoài. Project
đã được compile và build thành công với **Unity 2022.3.62f3**.

Điểm nhấn hình ảnh: nhân vật có **animation đi bộ 4 nhịp theo 3 hướng nhìn**
(xuống / lên / ngang), NPC có animation thở và **tự quay mặt về phía người
chơi** khi lại gần; các map được trang trí bằng thùng gỗ, hòm hàng, giếng nước,
bụi cây, tảng đá, biển chỉ dẫn cùng hoa và cỏ rải tự nhiên trên nền; màn chơi
có vignette và menu chính có 5 viên crystal bay lơ lửng.

## Chơi ngay (bản build sẵn)

Chạy `Build/Windows/KinhTeGioi.exe` — không cần cài gì thêm.

## Chạy trong Unity Editor

1. Mở project bằng **Unity 2022.3 LTS** (Unity Hub → Add → chọn thư mục `KinhTeGioi-Unity`).
2. Mở scene `Assets/Scenes/Main.unity` (tự tạo khi mở project lần đầu).
3. Bấm **Play** — `Bootstrap.cs` tự dựng camera, thế giới, UI và vào Main Menu.

## Build lại file .exe

- Trong Editor: menu **KTG → Build Windows** → xuất ra `Build/Windows/KinhTeGioi.exe`.
- Hoặc dòng lệnh (không cần mở Editor):

```
Unity.exe -batchmode -quit -projectPath <thư mục project> ^
  -executeMethod KTG.EditorTools.BuildGame.BuildWindows -logFile build.log
```

## Điều khiển

| Phím | Chức năng |
|---|---|
| WASD / Mũi tên | Di chuyển |
| E hoặc Space | Tương tác / tiếp tục hội thoại |
| I | Túi đồ |
| J | Nhật ký kiến thức |
| Esc | Tạm dừng |

## Vòng lặp gameplay

Khám phá bản đồ → nói chuyện NPC → trả lời câu hỏi → giải puzzle tại bệ đá →
nhận Crystal → mở cổng sang vùng đất tiếp theo. Chọn sai không bị Game Over —
NPC giải thích vì sao sai rồi cho chọn lại. Tiến trình tự lưu sau mỗi bước,
tắt game mở lại bấm **Tiếp Tục** để chơi tiếp.

**5 vùng đất, 5 mức độ tư duy:**

1. **Market of Many Hands** — nhận biết cạnh tranh & độc quyền (puzzle phân loại)
2. **Guild Town** — hiểu cạnh tranh đẻ ra độc quyền (puzzle sắp xếp trình tự)
3. **Bank of Interests** — phân loại biểu hiện độc quyền: tư bản tài chính, xuất khẩu tư bản, phân chia thị trường
4. **Valley of Market Illusions** — bắt lỗi các ngộ nhận về thị trường
5. **Palace of Regulation** — đối chất Boss Solandor bằng lý luận + thực tiễn Việt Nam

## Cấu trúc code

```
Assets/
 ├── Editor/
 │   ├── ProjectSetup.cs      # Tu tao scene Main.unity lan dau mo project
 │   └── BuildGame.cs         # Build Windows .exe (menu KTG hoac batchmode)
 └── Scripts/
     ├── Core/
     │   ├── Bootstrap.cs     # Diem khoi dau, dung camera/UI/GameManager
     │   ├── PixelArt.cs      # Sinh toan bo sprite bang code
     │   ├── AudioSynth.cs    # Hieu ung am thanh tong hop (sine/square)
     │   └── MusicSynth.cs    # Nhac nen mo mang (pad + arpeggio, loop)
     ├── Data/
     │   ├── DataModels.cs    # Cac class du lieu (Dialogue, Puzzle, MapDef...)
     │   └── GameContent.cs   # Toan bo noi dung: 5 map, NPC, hoi thoai,
     │                        # puzzle, journal, item, boss
     ├── World/
     │   ├── MapBuilder.cs        # Doc ASCII map, sinh tile/prop/NPC
     │   └── WorldComponents.cs   # CameraFollow, TorchFlicker, Bobber, WaterAnim
     ├── Player/
     │   └── PlayerController.cs  # Di chuyen, va cham, tuong tac
     ├── Systems/
     │   └── GameManager.cs   # State machine, save/load, dieu phoi toan bo game
     └── UI/
         ├── UIFactory.cs     # Ham dung Canvas/Panel/Button/Text
         ├── UIManager.cs     # Menu, HUD, hoi thoai, journal, inventory, pause
         ├── PuzzleUI.cs      # Puzzle: Classify / Order / Quiz
         └── BossUI.cs        # Tran doi chat voi boss cuoi
```

Muốn chỉnh nội dung học thuật (hội thoại, câu hỏi, journal)? Tất cả nằm trong
một file duy nhất: `Assets/Scripts/Data/GameContent.cs`.

Chi tiết về nội dung giáo dục và tech stack: xem
`../Triet-Gioi-MoTa-Game-va-TechStack.md` và `../Kinh_Te_Gioi_Mo_Ta_UI_Nang_Cap.md`.
