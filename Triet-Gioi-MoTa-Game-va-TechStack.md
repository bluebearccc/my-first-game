# Kinh Tế Giới — Mô tả Game & Tech Stack

*Tài liệu tổng hợp định hướng sản phẩm. Đồ án môn Kinh tế chính trị Mác - Lênin.*

---

## PHẦN 1 — MÔ TẢ GAME

### Đây là game gì?

Kinh Tế Giới là một **game phiêu lưu — kể chuyện 2D**, nhìn từ trên xuống (top-down) theo phong cách Stardew Valley, chạy trên trình duyệt web. Người chơi vào vai một sinh viên vô tình lạc vào một thế giới kỳ ảo mang hình hài một nền kinh tế thị trường thu nhỏ, và phải khám phá năm vùng đất để thu thập đủ năm viên Economic Crystal và tìm đường về nhà.

Cả năm vùng đất cùng xoay quanh **một nguyên lý kinh tế chính trị duy nhất — Cạnh tranh sinh ra độc quyền, nhưng độc quyền không thủ tiêu cạnh tranh mà làm nó trở nên đa dạng và gay gắt hơn** (quy luật thống nhất và đấu tranh giữa cạnh tranh và độc quyền trong nền kinh tế thị trường, dựa theo lý luận của V.I. Lênin về các đặc điểm kinh tế của độc quyền). Thay vì mỗi map dạy một khái niệm rời rạc, game đào sâu dần vào cùng một nguyên lý: vùng sau không giới thiệu kiến thức mới, mà buộc người chơi vận dụng nó ở mức độ tư duy cao hơn — từ nhận biết hai hiện tượng, đến hiểu quan hệ biện chứng giữa chúng, đến vận dụng, phản biện và cuối cùng là tổng hợp bằng thực tiễn.

Điểm cốt lõi: **người chơi học kinh tế chính trị bằng cách chơi, không phải bằng cách đọc.** Mỗi khái niệm được "chơi ra" qua hội thoại, puzzle và lựa chọn, chứ không trình bày như một bài giảng. Người chơi luôn có cảm giác đang phiêu lưu, không phải đang làm bài kiểm tra.

### Cảm giác của game (tone)

Ấm áp, mơ màng, yên bình — như một giấc mơ đẹp. Vì nhân vật chính "lạc vào thế giới kinh tế trong lúc ngủ gật", cả thế giới nên mang không khí siêu thực nhẹ nhàng: những phiên chợ nắng vàng, những thị trấn buôn bán ấm áp, âm nhạc êm dịu. Đây không phải game hành động căng thẳng; đây là một hành trình suy tư về thị trường được khoác lớp áo cổ tích.

### Lối chơi (gameplay loop)

Vòng lặp cốt lõi lặp lại ở mỗi vùng đất:

Khám phá bản đồ → nói chuyện NPC → nhận nhiệm vụ → giải puzzle → đưa ra lựa chọn → học một khái niệm kinh tế chính trị → nhận Crystal → mở khóa vùng đất tiếp theo.

Nguyên tắc thiết kế quan trọng: **chọn sai không bị Game Over.** Khi người chơi trả lời sai, NPC không phạt mà *giải thích tại sao sai* rồi gợi ý — biến cả cái sai thành một phần của bài học. Học được kể cả khi sai thì mới đáng chơi.

### Năm vùng đất — một nguyên lý, năm mức độ tư duy

Mỗi vùng đất tương ứng với một mức độ tư duy tăng dần (nhận biết → hiểu → vận dụng → phân tích → tổng hợp), nhưng luôn xoay quanh cùng một nguyên lý: **cạnh tranh sinh ra độc quyền, và độc quyền không thủ tiêu mà làm cạnh tranh trở nên đa dạng, gay gắt hơn**.

**Map 1 — Market of Many Hands (Nhận biết cạnh tranh & độc quyền).** Một phiên chợ làng nơi có hai góc chợ đối lập nhau: một dãy hàng rau với hàng chục người bán tranh nhau mời khách (cạnh tranh), và một góc chợ chỉ có duy nhất một thương nhân được cấp phép bán muối, không ai được bán cạnh (độc quyền). Người chơi làm quen với hai khái niệm qua quan sát trực quan nhất: nhiều người bán giành khách, đối lập với một người bán không ai tranh được — chưa cần giải thích quan hệ giữa chúng, chỉ cần nhận ra cả hai hiện tượng đều tồn tại trong thị trường.

**Map 2 — Guild Town (Hiểu thống nhất & đấu tranh).** Một thị trấn buôn bán nơi nhiều phường hội nhỏ cạnh tranh khốc liệt để giành khách, rồi dần dần sáp nhập, liên kết thành những liên minh buôn bán lớn khống chế cả thị trấn. Người chơi hiểu ra: chính cạnh tranh gay gắt là thứ **đẻ ra** độc quyền (do tích tụ, tập trung sản xuất), nhưng khi độc quyền hình thành, cạnh tranh không hề biến mất — nó chỉ **chuyển sang những hình thức mới**: cạnh tranh giữa các liên minh độc quyền với nhau, giữa liên minh độc quyền với những người bán còn lại bên ngoài, và cả cạnh tranh ngầm trong nội bộ một liên minh.

**Map 3 — Bank of Interests (Vận dụng: phân loại các biểu hiện của độc quyền).** Một thành phố tài chính phức tạp hơn, nơi độc quyền không còn đơn giản mà biểu hiện dưới nhiều dạng: tích tụ và tập trung vốn giữa các thương hội, ngân hàng thâu tóm và chi phối các xưởng sản xuất (tư bản tài chính), thương nhân mang vốn đi đầu tư ở vùng đất khác (xuất khẩu tư bản), các phe phái ngầm thỏa thuận chia nhau thị trường và địa bàn buôn bán. Người chơi tự phân loại từng sự kiện trong thành phố thuộc dạng biểu hiện nào của độc quyền, qua các tình huống hoàn toàn mới chưa từng gặp.

**Map 4 — Valley of Market Illusions (Phân tích & phản biện ngộ nhận).** Một thung lũng phủ đầy ảo ảnh, nơi cư dân tin vào những ngộ nhận sai lầm về thị trường: "độc quyền lúc nào cũng xấu, phải xóa bỏ hoàn toàn", "cứ để thị trường cạnh tranh tự do tuyệt đối, không cần ai can thiệp, thì mọi thứ sẽ tự tốt lên", "một khi đã có độc quyền thì cạnh tranh coi như đã chết hẳn". Người chơi phải chỉ ra lỗi lập luận trong từng ngộ nhận — đây là mức khó nhất, chuẩn bị "đạn" lý lẽ cho trận Boss.

**Map 5 — Palace of Regulation (Tổng hợp & thực tiễn).** Cung điện chứa Boss cuối — một đại thương nhân độc quyền liên tục lặp lại chính những ngộ nhận ở Map 4 để ngụy biện, bảo vệ đặc quyền của mình. Người chơi đánh bại Boss không bằng vũ khí, mà bằng cách phản biện bằng ví dụ thực tiễn (vai trò điều tiết của Nhà nước trong nền kinh tế thị trường định hướng xã hội chủ nghĩa ở Việt Nam: kiểm soát độc quyền, bảo vệ cạnh tranh lành mạnh, hài hòa lợi ích giữa doanh nghiệp và xã hội), chứng minh mâu thuẫn cạnh tranh — độc quyền không bị xóa bỏ mà được điều tiết để thúc đẩy phát triển.

### Các hệ thống trong game

**Hội thoại (Dialogue).** Giao diện kiểu visual novel: chân dung NPC ở góc, hộp thoại chạy chữ, người chơi chọn đáp án A/B/C. Đây là xương sống kể chuyện của game.

**Nhiệm vụ (Quest).** Nhiều loại: thu thập vật phẩm, nói chuyện với NPC, giải puzzle, điều tra tìm nguyên nhân thật, và ra quyết định. Mỗi quest hoàn thành sẽ dẫn tới một bài học và một phần thưởng.

**Puzzle.** Mỗi vùng đất có một cơ chế tương tác riêng phản ánh đúng mức độ tư duy của map đó (ví dụ Map 1 ghép cặp hiện tượng — sạp hàng đông người bán với sạp hàng độc quyền; Map 4 "bắt lỗi" trong các đoạn hội thoại chứa ngụy biện về thị trường). Người chơi thể hiện hiểu biết bằng thao tác, không phải bằng cách chọn trắc nghiệm.

**Túi đồ (Inventory).** Chứa Seed, Book, Crystal, Letter, Map, Key, Potion... Nhiều vật phẩm mang ý nghĩa gắn với câu chuyện.

**Nhật ký kiến thức (Journal).** Đây là hệ thống quan trọng nhất về mặt giáo dục — giống một cuốn Wikipedia trong game. Mỗi khi học xong một khái niệm, người chơi mở khóa một trang gồm: định nghĩa, ví dụ, minh họa, và liên hệ đời sống thực. Đây là nơi kiến thức kinh tế chính trị được ghi lại một cách có hệ thống.

**Phần thưởng (Reward).** Không thưởng bằng điểm số. Thưởng bằng Crystal, thành tựu (achievement), kỹ năng mới, khu vực mới, và mẩu cốt truyện (lore) — những thứ tạo động lực khám phá thay vì cảm giác thi cử.

**Boss fight.** Boss cuối không đánh bằng kiếm mà liên tục lặp lại các ngộ nhận về cạnh tranh — độc quyền đã gặp ở Map 4 (ví dụ "độc quyền lúc nào cũng xấu, phải xóa bỏ hoàn toàn"); người chơi phản biện đúng bằng lý luận + ví dụ thực tiễn thì Boss mất máu, sai thì Boss phản biện lại. Có animation, gần với một trận đấu trí kiểu visual novel.

---

## PHẦN 2 — TECH STACK (giải thích chi tiết)

Toàn bộ game được xây dựng trên **Unity**. So với một engine web nhẹ, Unity nặng hơn và cần cài Unity Editor để build, nhưng đổi lại có bộ công cụ 2D, animation, lighting, âm thanh và quản lý scene mạnh hơn hẳn, tài liệu/tutorial phong phú, và là kỹ năng công nghiệp thực tế nếu nhóm muốn học sâu hơn về làm game thay vì chỉ làm cho xong đồ án. Không cần backend vì đây là game chơi một người, mọi dữ liệu lưu ngay trên máy người chơi.

### Tổng quan các thành phần

| Thành phần | Công nghệ | Vai trò |
|---|---|---|
| Game engine | Unity (2D URP) | Chạy toàn bộ thế giới game: scene, nhân vật, va chạm, animation |
| Ngôn ngữ | C# | Viết toàn bộ logic gameplay và hệ thống |
| Bản đồ | Unity Tilemap + Tile Palette | Ghép bản đồ từ các ô vuông pixel art, đánh dấu va chạm |
| Giao diện (UI) | Unity UI Toolkit (hoặc uGUI) | Menu, hội thoại, túi đồ, nhật ký, quest UI |
| Camera & chuyển cảnh | Cinemachine | Camera theo nhân vật, hiệu ứng chuyển cảnh giữa các vùng |
| Animation & cutscene | Animator + Timeline | Animation nhân vật/NPC, hiệu ứng chữ chạy, cutscene ngắn |
| Ánh sáng | URP 2D Lighting + Bloom | Ánh sáng mềm, hiệu ứng Bloom, chu kỳ ngày – đêm |
| Hiệu ứng | Particle System (Shuriken) | Mưa, nắng, lá rơi, hiệu ứng phép thuật |
| Dữ liệu game | ScriptableObject | Định nghĩa Quest, Dialogue, Item, Journal Entry dưới dạng asset, dễ chỉnh không cần sửa code |
| Âm thanh | Unity Audio Mixer | Nhạc nền, hiệu ứng âm thanh, crossfade theo vùng |
| Lưu dữ liệu | JSON + File I/O (Application.persistentDataPath) | Lưu tiến trình chơi (map, item, Crystal, khái niệm đã học) |
| Build & triển khai | Unity Build Windows/Mac hoặc WebGL | Xuất file .exe để nộp bài, hoặc build WebGL để có link demo chạy trên trình duyệt |
| Version control | Git + Git LFS | Quản lý code và asset nhị phân (sprite, audio) dung lượng lớn |
| Asset đồ họa | Kenney / itch.io / OpenGameArt (import vào Unity) | Sprite, tile pixel art miễn phí |
| Âm thanh | Freesound / Pixabay | Nhạc nền và hiệu ứng miễn phí |

### Giải thích từng thành phần

**Unity (2D URP) — trái tim của game.** Unity là engine game phổ biến nhất hiện nay, hỗ trợ 2D đầy đủ: quản lý "Scene" (mỗi vùng đất là một Scene riêng), Tilemap dựng bản đồ kiểu Stardew, hệ thống animation, va chạm (Collider/Rigidbody2D), và Universal Render Pipeline (URP) cho ánh sáng 2D + Bloom. Unity lo toàn bộ phần "thế giới game": những gì diễn ra bên trong màn chơi.

**C# — ngôn ngữ viết logic.** Là ngôn ngữ chính thức của Unity, có kiểm tra kiểu dữ liệu chặt chẽ. Với một game nhiều hệ thống (quest, dialogue, inventory, journal), C# giúp bắt lỗi ngay khi biên dịch thay vì để lỗi âm thầm chạy sai lúc demo, và MonoBehaviour + ScriptableObject giúp chia code thành các module gọn gàng, nhiều người trong nhóm cùng làm không giẫm chân nhau.

**Unity Tilemap — công cụ vẽ bản đồ.** Để làm bản đồ kiểu Stardew, người ta không vẽ cả bản đồ thành một ảnh khổng lồ, mà ghép nó từ hàng trăm ô vuông nhỏ (tile) như xếp gạch. Unity có sẵn Tilemap + Tile Palette ngay trong Editor: kéo-thả tile để "xây" ngôi làng, khu rừng, thành phố; dùng Tilemap Collider 2D để đánh dấu chỗ nào đi được, chỗ nào là tường; đặt vị trí NPC và vật phẩm bằng Prefab. Không cần công cụ ngoài như Tiled vì mọi thứ nằm trong cùng một Editor.

**Unity UI Toolkit / uGUI — làm giao diện.** Đây là điểm phân vai rõ ràng: **thế giới game (Tilemap, nhân vật, va chạm) và các bảng giao diện (menu, túi đồ, nhật ký kiến thức, cài đặt, lưu/tải) đều dựng trong cùng một Editor nhưng khác layer.** UI Toolkit (hoặc Canvas/uGUI cho ai quen thao tác kéo-thả hơn) cho phép dựng nút bấm, danh sách, bố cục nhanh bằng công cụ kéo-thả có sẵn, không cần viết HTML/CSS riêng.

**Cinemachine — camera & chuyển cảnh.** Gói này của Unity xử lý camera theo dõi nhân vật mượt mà, và tạo hiệu ứng chuyển cảnh (fade, pan) giữa các vùng đất mà không cần tự viết logic camera từ đầu.

**ScriptableObject — dữ liệu game.** Thay vì hard-code nội dung Quest/Dialogue/Item/Journal trong script, ScriptableObject cho phép định nghĩa chúng như các file asset riêng trong Unity Editor. Người phụ trách nội dung (kịch bản, câu hỏi kinh tế) có thể chỉnh sửa trực tiếp trên Inspector mà không cần đụng vào code.

**JSON + File I/O — lưu tiến trình.** Vì game chơi một người và không có backend, mọi dữ liệu (đang ở map nào, đã có bao nhiêu Crystal, đã học khái niệm gì) được serialize ra JSON và lưu vào `Application.persistentDataPath` trên máy người chơi. Người chơi tắt đi mở lại vẫn tiếp tục được. Không cần máy chủ, không cần tài khoản, không tốn chi phí.

**Build & triển khai.** Unity cho phép build ra file .exe (Windows) hoặc .app (Mac) để nộp bài trực tiếp, hoặc build sang WebGL nếu muốn có một đường link demo gửi giảng viên và cả lớp mà không cần cài đặt gì.

**Asset đồ họa và âm thanh — miễn phí.** Kenney, itch.io, OpenGameArt cung cấp hàng nghìn bộ sprite và tile pixel art phong cách Stardew, import thẳng vào Unity qua Sprite Editor. Freesound và Pixabay Audio cung cấp nhạc nền và hiệu ứng âm thanh miễn phí. Nhóm gần như không phải tự vẽ hay tự làm nhạc — chỉ cần chọn một bộ asset nhất quán và bám theo nó cho cả game.

### Cấu trúc thư mục đề xuất (Unity Project)

```
Assets/
 ├── Scenes/                # Mỗi vùng đất là một Scene Unity
 │   ├── Boot.unity               # Nạp asset ban đầu
 │   ├── MainMenu.unity           # Màn hình chính
 │   ├── Map1_Market.unity        # Market of Many Hands (nhận biết cạnh tranh & độc quyền)
 │   ├── Map2_GuildTown.unity     # Guild Town (thống nhất & đấu tranh)
 │   ├── Map3_Bank.unity          # Bank of Interests (phân loại biểu hiện độc quyền)
 │   ├── Map4_Valley.unity        # Valley of Market Illusions (ngộ nhận & phản biện)
 │   ├── Map5_Palace.unity        # Palace of Regulation (tổng hợp & thực tiễn)
 │   └── BossFight.unity          # Boss cuối
 ├── Scripts/
 │   ├── Entities/                # Player.cs, NPC.cs, Item.cs, Boss.cs
 │   ├── Systems/                 # QuestSystem.cs, DialogueSystem.cs, InventorySystem.cs,
 │   │                             # SaveSystem.cs, JournalSystem.cs, PuzzleSystem.cs
 │   ├── UI/                      # MenuUI.cs, DialogueUI.cs, InventoryUI.cs, JournalUI.cs, QuestUI.cs
 │   └── Data/                    # Định nghĩa các class ScriptableObject (QuestData, DialogueData...)
 ├── Prefabs/               # Player, NPC, Item, UI panel dùng lại được
 ├── ScriptableObjects/     # Asset dữ liệu: Quest, Dialogue, Item, Journal Entry
 ├── Tilemaps/              # Tile palette và tilemap từng vùng đất
 ├── Sprites/               # Sprite nhân vật, NPC, vật phẩm, tile
 ├── Animations/            # Animator Controller, animation clip
 └── Audio/                 # Nhạc nền, hiệu ứng âm thanh
```

### Tóm tắt: ai lo việc gì

Cách dễ nhớ nhất về kiến trúc này:

**Unity Scene + Tilemap** lo mọi thứ *bên trong* thế giới game — đi bộ, bản đồ, va chạm, animation, ánh sáng. **UI Toolkit/uGUI** lo mọi thứ *phủ lên trên* — menu, túi đồ, nhật ký. **C#** là ngôn ngữ dán tất cả lại với nhau. **ScriptableObject** là nơi lưu nội dung Quest/Dialogue/Item mà không cần sửa code. **Cinemachine** lo camera và chuyển cảnh. **JSON + File I/O** nhớ tiến trình. **Unity Build** đóng gói ra .exe hoặc WebGL. Và **asset miễn phí** cho phần nhìn và nghe.

Toàn bộ stack này chạy trên Unity Editor, không cần backend — phù hợp cho một đồ án môn học nhưng cũng đủ nền tảng nếu nhóm muốn phát triển tiếp sau này.
