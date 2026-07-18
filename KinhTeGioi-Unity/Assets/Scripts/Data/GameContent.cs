using System.Collections.Generic;
using UnityEngine;

namespace KTG
{
    // Kho du lieu tinh cho toan bo game: 5 map, NPC, hoi thoai, puzzle, nhat ky, boss.
    public static class GameContent
    {
        public static readonly string BossName = "Solandor — Vua Độc Quyền";

        public static readonly string[] CrystalNames =
        {
            "Viên Ngọc Chợ Búa", "Viên Ngọc Guild Town", "Viên Ngọc Ngân Thành",
            "Viên Ngọc Ảo Ảnh", "Viên Ngọc Điều Tiết"
        };

        public static readonly MapDef[] Maps = { BuildMap0(), BuildMap1(), BuildMap2(), BuildMap3(), BuildMap4() };

        // ---------------------------------------------------------------- MAP 0
        static MapDef BuildMap0()
        {
            var g = new MapGrid(28, 16, '#');
            g.Rect(1, 1, 26, 14, ',');            // nen co xanh
            g.Rect(1, 8, 26, 1, '.');             // duong dat ngang qua cho
            g.Rect(13, 1, 1, 13, '.');            // duong doc
            g.Rect(10, 3, 7, 3, '.');             // san cho trung tam
            g.Rect(4, 7, 5, 3, '.');              // khu sap rau
            g.Rect(19, 7, 5, 3, '.');             // khu sap muoi
            g.Rect(11, 11, 5, 3, '.');            // san be da
            // nha dan hai ben cho
            g.Set(6, 3, 'H'); g.Set(21, 3, 'H');
            // NPC + sap hang
            g.Set(13, 4, 'C');
            g.Set(6, 8, 'A'); g.Set(6, 9, 's');
            g.Set(20, 8, 'B'); g.Set(20, 9, 's');
            // cay, duoc, gieng
            g.Set(3, 5, 'T'); g.Set(24, 5, 'T'); g.Set(3, 11, 'T'); g.Set(24, 11, 'T');
            g.Set(13, 11, 't');
            g.Set(9, 4, 'w');
            // hang rao vuon rau hai goc
            g.Set(3, 13, 'F'); g.Set(4, 13, 'F'); g.Set(5, 13, 'F');
            g.Set(22, 13, 'F'); g.Set(23, 13, 'F'); g.Set(24, 13, 'F');
            // thung hang canh sap
            g.Set(4, 9, 'b'); g.Set(5, 9, 'c'); g.Set(22, 9, 'c'); g.Set(23, 9, 'b');
            // bui cay + bien chi duong
            g.Set(10, 2, 'u'); g.Set(16, 2, 'u'); g.Set(26, 7, 'n');
            // dong vat trong cho
            g.Set(10, 6, 'k'); g.Set(16, 6, 'k'); g.Set(13, 9, 'd');
            // cau chuyen an giau o goc cho
            g.Set(2, 14, 'L');
            // be da, spawn, cong
            g.Set(13, 12, '*');
            g.Set(13, 8, 'P');
            g.Set(27, 8, '>');

            var m = new MapDef
            {
                Id = "market",
                Title = "Market of Many Hands",
                Rows = g.Rows(),
                Ground = new Color(0.8f, 0.64f, 0.42f),
                Grass = new Color(0.48f, 0.7f, 0.32f),
                Water = new Color(0.2f, 0.45f, 0.6f),
                Flow = new[] { 'C', 'A', 'B', '*' },
                Objectives = new[]
                {
                    "Hãy nói chuyện với Trưởng Chợ",
                    "Hỏi chuyện Dì Ba ở sạp rau",
                    "Hỏi chuyện Ông Muối Trắng ở sạp muối",
                    "Đến bệ đá giữa chợ để giải đố",
                    "Đi tới cổng phía Đông để rời khỏi chợ"
                },
                RewardAction = "crystal;journal:canh_tranh;journal:doc_quyen;item:muoi_phep;toast:Bạn nhận được Viên Ngọc Chợ Búa!"
            };
            m.Npcs['C'] = new NpcDef { Code = 'C', Name = "Trưởng Chợ", Hair = new Color(0.5f, 0.35f, 0.2f), Skin = new Color(0.85f, 0.65f, 0.5f), Shirt = new Color(0.7f, 0.45f, 0.15f) };
            m.Npcs['A'] = new NpcDef { Code = 'A', Name = "Dì Ba", Hair = new Color(0.15f, 0.1f, 0.1f), Skin = new Color(0.9f, 0.7f, 0.55f), Shirt = new Color(0.3f, 0.6f, 0.35f) };
            m.Npcs['B'] = new NpcDef { Code = 'B', Name = "Ông Muối Trắng", Hair = new Color(0.85f, 0.85f, 0.85f), Skin = new Color(0.8f, 0.62f, 0.5f), Shirt = new Color(0.85f, 0.85f, 0.95f) };
            return m;
        }

        // ---------------------------------------------------------------- MAP 1
        static MapDef BuildMap1()
        {
            var g = new MapGrid(28, 16, '#');
            g.Rect(1, 1, 26, 14, ',');            // nen co
            g.Rect(1, 8, 26, 1, '.');             // duong chinh
            g.Rect(13, 1, 1, 13, '.');
            g.Rect(5, 5, 4, 3, '.');              // san truoc xuong det
            g.Rect(19, 5, 4, 3, '.');
            g.Rect(11, 10, 6, 4, '.');            // quang truong quan tro
            // xuong det va hoi quan lien minh
            g.Set(8, 3, 'H'); g.Set(19, 3, 'H');
            // NPC
            g.Set(6, 6, 'A'); g.Set(20, 6, 'B'); g.Set(13, 11, 'C');
            // cay, duoc, gieng
            g.Set(4, 12, 'T'); g.Set(23, 12, 'T');
            g.Set(13, 4, 't');
            g.Set(9, 12, 'w');
            // hang hoa xuong det
            g.Set(4, 7, 'b'); g.Set(5, 7, 'c'); g.Set(21, 7, 'c'); g.Set(22, 7, 'b');
            // hang rao truoc nha
            g.Set(3, 4, 'F'); g.Set(4, 4, 'F'); g.Set(23, 4, 'F'); g.Set(24, 4, 'F');
            // bui cay + bien
            g.Set(12, 2, 'u'); g.Set(15, 2, 'u'); g.Set(26, 7, 'n');
            // dong vat thi tran
            g.Set(11, 8, 'k'); g.Set(16, 9, 'd');
            // cau chuyen an
            g.Set(25, 14, 'L');
            g.Set(13, 12, '*');
            g.Set(13, 8, 'P');
            g.Set(0, 8, '<'); g.Set(27, 8, '>');

            var m = new MapDef
            {
                Id = "guild",
                Title = "Guild Town",
                Rows = g.Rows(),
                Ground = new Color(0.64f, 0.48f, 0.35f),
                Grass = new Color(0.42f, 0.62f, 0.3f),
                Water = new Color(0.2f, 0.4f, 0.55f),
                Flow = new[] { 'A', 'B', 'C', '*' },
                Objectives = new[]
                {
                    "Hãy nói chuyện với thợ dệt Lina",
                    "Hỏi chuyện Vance, người đứng đầu Liên Minh Dệt",
                    "Hỏi chuyện chủ quán trọ",
                    "Đến bệ đá để sắp xếp lại chuỗi sự kiện",
                    "Rời Guild Town qua cổng phía Đông"
                },
                RewardAction = "crystal;journal:tich_tu_tap_trung;journal:hinh_thuc_moi;item:soi_det;toast:Bạn nhận được Viên Ngọc Guild Town!"
            };
            m.Npcs['A'] = new NpcDef { Code = 'A', Name = "Lina", Hair = new Color(0.6f, 0.3f, 0.15f), Skin = new Color(0.88f, 0.68f, 0.53f), Shirt = new Color(0.5f, 0.2f, 0.25f) };
            m.Npcs['B'] = new NpcDef { Code = 'B', Name = "Vance", Hair = new Color(0.1f, 0.1f, 0.12f), Skin = new Color(0.82f, 0.6f, 0.48f), Shirt = new Color(0.15f, 0.25f, 0.45f) };
            m.Npcs['C'] = new NpcDef { Code = 'C', Name = "Chủ Quán Rowan", Hair = new Color(0.4f, 0.4f, 0.4f), Skin = new Color(0.86f, 0.66f, 0.5f), Shirt = new Color(0.4f, 0.3f, 0.2f) };
            return m;
        }

        // ---------------------------------------------------------------- MAP 2
        static MapDef BuildMap2()
        {
            var g = new MapGrid(28, 16, '#');
            g.Rect(1, 1, 26, 14, ',');            // nen co cong vien
            g.Rect(1, 8, 26, 1, '.');             // dai lo lat da
            g.Rect(13, 1, 1, 13, '.');
            g.Rect(5, 5, 5, 3, '.');              // san truoc ngan hang
            g.Rect(18, 5, 5, 3, '.');
            g.Rect(10, 10, 8, 4, '.');            // quang truong giao dich
            // toa ngan hang va thuong hoi
            g.Set(8, 3, 'H'); g.Set(19, 3, 'H');
            // ho nuoc trang tri hai goc
            g.Set(2, 2, '~'); g.Set(3, 2, '~'); g.Set(24, 2, '~'); g.Set(25, 2, '~');
            // NPC
            g.Set(6, 6, 'A'); g.Set(20, 6, 'B'); g.Set(13, 11, 'C');
            g.Set(13, 4, 't');
            // cot da do thi
            g.Set(5, 4, 'r'); g.Set(22, 4, 'r');
            // kien hang cho xuat khau
            g.Set(3, 9, 'c'); g.Set(4, 9, 'c'); g.Set(23, 9, 'b'); g.Set(24, 9, 'b');
            // hang rao ben cang
            g.Set(3, 13, 'F'); g.Set(4, 13, 'F'); g.Set(23, 13, 'F'); g.Set(24, 13, 'F');
            g.Set(26, 7, 'n');
            // cho canh gac ngan hang
            g.Set(16, 9, 'd');
            // cau chuyen an
            g.Set(2, 14, 'L');
            g.Set(13, 12, '*');
            g.Set(13, 8, 'P');
            g.Set(0, 8, '<'); g.Set(27, 8, '>');

            var m = new MapDef
            {
                Id = "bank",
                Title = "Ngân Thành — Bank of Interests",
                Rows = g.Rows(),
                Ground = new Color(0.62f, 0.64f, 0.7f),
                Grass = new Color(0.4f, 0.58f, 0.42f),
                Water = new Color(0.25f, 0.45f, 0.68f),
                Flow = new[] { 'A', 'B', 'C', '*' },
                Objectives = new[]
                {
                    "Hãy nói chuyện với chủ ngân hàng Aurel",
                    "Hỏi chuyện thương nhân viễn xứ Sable",
                    "Hỏi chuyện ký lục viên Piu",
                    "Đến bệ đá để phân loại các biểu hiện của độc quyền",
                    "Rời Ngân Thành qua cổng phía Đông"
                },
                RewardAction = "crystal;journal:tu_ban_tai_chinh;journal:xuat_khau_tu_ban;journal:phan_chia_thi_truong;item:so_ghi_chep;toast:Bạn nhận được Viên Ngọc Ngân Thành!"
            };
            m.Npcs['A'] = new NpcDef { Code = 'A', Name = "Aurel", Hair = new Color(0.2f, 0.2f, 0.22f), Skin = new Color(0.8f, 0.6f, 0.48f), Shirt = new Color(0.15f, 0.35f, 0.15f) };
            m.Npcs['B'] = new NpcDef { Code = 'B', Name = "Sable", Hair = new Color(0.3f, 0.15f, 0.05f), Skin = new Color(0.6f, 0.42f, 0.3f), Shirt = new Color(0.5f, 0.35f, 0.1f) };
            m.Npcs['C'] = new NpcDef { Code = 'C', Name = "Piu", Hair = new Color(0.15f, 0.15f, 0.15f), Skin = new Color(0.85f, 0.65f, 0.52f), Shirt = new Color(0.3f, 0.3f, 0.35f) };
            return m;
        }

        // ---------------------------------------------------------------- MAP 3
        static MapDef BuildMap3()
        {
            var g = new MapGrid(28, 16, '#');
            g.Rect(1, 1, 26, 14, ',');            // tham co tim huyen ao
            g.Rect(1, 8, 26, 1, '.');             // loi mon xuyen suong
            g.Rect(13, 1, 1, 13, '.');
            g.Rect(11, 11, 5, 3, '.');            // khoang trong quanh be da
            // NPC
            g.Set(13, 4, 'A'); g.Set(6, 10, 'B'); g.Set(20, 10, 'C');
            // nam phat sang bon goc
            g.Set(4, 4, 'o'); g.Set(23, 4, 'o'); g.Set(4, 12, 'o'); g.Set(23, 12, 'o');
            // da tang huyen ao
            g.Set(8, 3, 'r'); g.Set(18, 3, 'r'); g.Set(3, 8, 'r'); g.Set(24, 8, 'r');
            // bui cay
            g.Set(10, 12, 'u'); g.Set(16, 12, 'u'); g.Set(2, 5, 'u'); g.Set(25, 5, 'u');
            // hang rao do nat cua khu dinh cu cu
            g.Set(6, 6, 'F'); g.Set(7, 6, 'F'); g.Set(20, 6, 'F'); g.Set(21, 6, 'F');
            // cau chuyen an giau sau Nha Hien Triet
            g.Set(13, 2, 'L');
            g.Set(13, 12, '*');
            g.Set(13, 8, 'P');
            g.Set(0, 8, '<'); g.Set(27, 8, '>');

            var m = new MapDef
            {
                Id = "valley",
                Title = "Valley of Market Illusions",
                Rows = g.Rows(),
                Ground = new Color(0.56f, 0.44f, 0.68f),
                Grass = new Color(0.44f, 0.38f, 0.62f),
                Water = new Color(0.3f, 0.25f, 0.55f),
                Flow = new[] { 'A', 'B', 'C', '*' },
                Objectives = new[]
                {
                    "Hãy nói chuyện với Nhà Hiền Triết Mù",
                    "Nghe Mira kể về ngộ nhận của cô",
                    "Nghe Talos kể về ngộ nhận của anh ta",
                    "Đến bệ đá để bắt lỗi các ngộ nhận",
                    "Rời thung lũng qua cổng phía Đông, tiến vào Cung Điện"
                },
                RewardAction = "crystal;journal:dieu_tiet_nha_nuoc;journal:ngo_nhan_thi_truong;item:kinh_soi;toast:Bạn nhận được Viên Ngọc Ảo Ảnh!"
            };
            m.Npcs['A'] = new NpcDef { Code = 'A', Name = "Nhà Hiền Triết Mù", Hair = new Color(0.9f, 0.9f, 0.9f), Skin = new Color(0.75f, 0.58f, 0.48f), Shirt = new Color(0.6f, 0.5f, 0.7f) };
            m.Npcs['B'] = new NpcDef { Code = 'B', Name = "Mira", Hair = new Color(0.5f, 0.2f, 0.5f), Skin = new Color(0.85f, 0.68f, 0.6f), Shirt = new Color(0.4f, 0.2f, 0.5f) };
            m.Npcs['C'] = new NpcDef { Code = 'C', Name = "Talos", Hair = new Color(0.25f, 0.2f, 0.3f), Skin = new Color(0.7f, 0.55f, 0.5f), Shirt = new Color(0.3f, 0.25f, 0.45f) };
            return m;
        }

        // ---------------------------------------------------------------- MAP 4
        static MapDef BuildMap4()
        {
            var g = new MapGrid(28, 16, '#');
            g.Rect(1, 1, 26, 14, ',');            // tham san cung dien
            g.Rect(9, 1, 9, 14, '.');             // long duong da giua dien
            g.Rect(1, 12, 26, 1, '.');
            g.Set(13, 3, 'X');
            g.Set(8, 3, 't'); g.Set(18, 3, 't');
            // hai hang cot da dan len ngai vang
            g.Set(6, 5, 'r'); g.Set(20, 5, 'r');
            g.Set(6, 8, 'r'); g.Set(20, 8, 'r');
            // lan can go truoc san chau
            g.Set(9, 11, 'F'); g.Set(10, 11, 'F'); g.Set(16, 11, 'F'); g.Set(17, 11, 'F');
            // thung hang cong nap
            g.Set(4, 11, 'b'); g.Set(22, 11, 'b');
            // linh gac
            g.Set(13, 10, 'A');
            // cau chuyen an trong goc cung dien
            g.Set(25, 14, 'L');
            g.Set(13, 12, 'P');
            g.Set(0, 12, '<');

            var m = new MapDef
            {
                Id = "palace",
                Title = "Palace of Regulation",
                Rows = g.Rows(),
                Ground = new Color(0.44f, 0.42f, 0.56f),
                Grass = new Color(0.36f, 0.34f, 0.48f),
                Water = new Color(0.2f, 0.2f, 0.4f),
                Flow = new[] { 'A', 'X' },
                Objectives = new[]
                {
                    "Hãy nói chuyện với lính gác trước cửa",
                    "Bước lên ngai vàng và đối chất với Solandor",
                    "Đã hoàn thành"
                },
                RewardAction = "crystal;journal:tong_hop_quy_luat;item:huy_hieu_dieu_tiet;win"
            };
            m.Npcs['A'] = new NpcDef { Code = 'A', Name = "Lính Gác", Hair = new Color(0.2f, 0.2f, 0.2f), Skin = new Color(0.8f, 0.6f, 0.48f), Shirt = new Color(0.25f, 0.25f, 0.3f) };
            return m;
        }

        // Per-map torch flame tint (dùng bởi MapBuilder)
        public static Color TorchColor(int mapIndex)
        {
            switch (mapIndex)
            {
                case 0: return new Color(1f, 0.55f, 0.2f);
                case 1: return new Color(1f, 0.5f, 0.15f);
                case 2: return new Color(0.3f, 0.9f, 0.9f);
                case 3: return new Color(0.8f, 0.4f, 0.9f);
                default: return new Color(0.95f, 0.8f, 0.3f);
            }
        }

        // Per-map ambient light — Phase B HD-2D (dùng bởi Lighting2D.AddGlobal):
        // market ấm sáng · guild trung tính ám khói · bank lạnh xám-lam ·
        // valley tím huyền ảo tối · palace lạnh trang nghiêm
        public static Color MapAmbientColor(int mapIndex)
        {
            switch (mapIndex)
            {
                case 0: return new Color(1f, 0.96f, 0.88f);
                case 1: return new Color(0.93f, 0.90f, 0.85f);
                case 2: return new Color(0.80f, 0.86f, 1f);
                case 3: return new Color(0.62f, 0.55f, 0.85f);
                default: return new Color(0.78f, 0.82f, 1f);
            }
        }

        public static float MapAmbientIntensity(int mapIndex)
        {
            switch (mapIndex)
            {
                case 0: return 1.0f;
                case 1: return 0.9f;
                case 2: return 0.85f;
                case 3: return 0.7f;
                default: return 0.78f;
            }
        }

        // ================================================================ DIALOGUE
        static readonly Dictionary<string, DialogueDef> MainDialogues = new Dictionary<string, DialogueDef>();
        static readonly Dictionary<string, DialogueDef> FlavorDialogues = new Dictionary<string, DialogueDef>();

        static string Key(int mapIndex, char npc) => mapIndex + "_" + npc;

        static GameContent()
        {
            BuildMap0Dialogues();
            BuildMap1Dialogues();
            BuildMap2Dialogues();
            BuildMap3Dialogues();
            BuildMap4Dialogues();
        }

        public static DialogueDef GetDialogue(int mapIndex, char npc, int stage)
        {
            var map = Maps[mapIndex];
            int flowIdx = System.Array.IndexOf(map.Flow, npc);
            bool isActiveNow = flowIdx >= 0 && flowIdx == stage;
            var key = Key(mapIndex, npc);
            if (isActiveNow && MainDialogues.TryGetValue(key, out var main)) return main;
            if (FlavorDialogues.TryGetValue(key, out var flavor)) return flavor;
            return new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine(npc.ToString(), "...") },
                Action = null
            };
        }

        static void BuildMap0Dialogues()
        {
            MainDialogues[Key(0, 'C')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("!", "Bạn tỉnh dậy giữa một phiên chợ nhộn nhịp, không nhớ vì sao mình lại ở đây."),
                    new DialogueLine("C", "Chào mừng đến với Econia, người lạ! Đây là một thế giới vận hành hoàn toàn bằng quy luật kinh tế."),
                    new DialogueLine("C", "Muốn về nhà, cháu phải thu thập đủ năm Economic Crystal rải rác khắp năm vùng đất."),
                    new DialogueLine("C", "Viên đầu tiên nằm đâu đó trong chợ này. Hãy ra sạp rau phía Tây và sạp muối phía Đông trước đã.")
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(0, 'C')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("C", "Cứ từ từ khám phá khu chợ đi, đừng vội.") }
            };

            MainDialogues[Key(0, 'A')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("A", "Rau nhà tôi tươi lắm! Nhưng dạo này tôi phải bán rẻ hơn cả tháng trước."),
                    new DialogueLine("A", "Nhìn quanh xem — cả chục sạp rau khác cũng đang giành khách với tôi."),
                    new DialogueLine("A", "Cháu có biết vì sao tôi phải liên tục giảm giá và cải thiện chất lượng không?",
                        new List<DialogueChoice>
                        {
                            new DialogueChoice("Vì nhiều sạp rau khác cũng đang tranh giành khách hàng", true,
                                "Đúng vậy! Càng nhiều người bán cùng một mặt hàng, ai cũng phải cạnh tranh để không mất khách."),
                            new DialogueChoice("Vì Dì Ba thích bán rẻ cho vui", false,
                                "Không phải đâu cháu, có ai muốn bán lỗ bao giờ — phải có lý do buộc dì làm vậy."),
                            new DialogueChoice("Vì có người ép Dì Ba phải bán rẻ", false,
                                "Không ai ép cả, đây là chuyện tự nhiên xảy ra khi có nhiều người bán giống nhau.")
                        })
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(0, 'A')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("A", "Mua giúp dì bó rau đi cháu, tươi lắm!") }
            };

            MainDialogues[Key(0, 'B')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("B", "Ta là người duy nhất được lãnh chúa cấp phép bán muối trong vùng này."),
                    new DialogueLine("B", "Ta không cần giảm giá, không cần chào mời ai cả. Khách vẫn phải mua."),
                    new DialogueLine("B", "Cháu đoán xem, vì sao ta chẳng bao giờ phải cạnh tranh như mấy sạp rau kia?",
                        new List<DialogueChoice>
                        {
                            new DialogueChoice("Vì không ai khác được phép bán muối, khách không có lựa chọn nào khác", true,
                                "Chính xác. Khi chỉ có một người bán duy nhất, người đó không còn áp lực phải cạnh tranh nữa."),
                            new DialogueChoice("Vì muối của ông ngon hơn hẳn", false,
                                "Ha! Muối ở đâu chẳng giống nhau, vấn đề không nằm ở chất lượng đâu cháu."),
                            new DialogueChoice("Vì ông quen biết rộng nên không ai dám cạnh tranh", false,
                                "Không phải vậy, đây là vì quyền bán bị giới hạn hợp pháp, chứ không phải quen biết.")
                        })
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(0, 'B')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("B", "Muối trắng tinh khiết đây, không đâu bán được đâu.") }
            };
        }

        static void BuildMap1Dialogues()
        {
            MainDialogues[Key(1, 'A')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("A", "Chào cháu, ta là Lina, thợ dệt. Trước đây khu này có hàng chục xưởng dệt nhỏ."),
                    new DialogueLine("A", "Chúng ta cạnh tranh khốc liệt — ai cải tiến khung dệt nhanh hơn, ai giảm giá sâu hơn thì sống sót."),
                    new DialogueLine("A", "Rồi dần dần, các xưởng thắng thế gom hết vốn, mua lại xưởng thua. Giờ chỉ còn Liên Minh Dệt của Vance."),
                    new DialogueLine("A", "Cháu có biết điều gì đã khiến các xưởng nhỏ bị sáp nhập như vậy không?",
                        new List<DialogueChoice>
                        {
                            new DialogueChoice("Chính cạnh tranh khốc liệt khiến xưởng yếu bị xưởng mạnh thâu tóm", true,
                                "Đúng! Cạnh tranh dữ dội thúc đẩy tích tụ và tập trung vốn, và cuối cùng sinh ra độc quyền."),
                            new DialogueChoice("Chỉ đơn giản là Vance may mắn hơn người khác", false,
                                "Không hẳn là may mắn — đó là kết quả tất yếu của một quá trình cạnh tranh kéo dài."),
                            new DialogueChoice("Nhà vua ra lệnh sáp nhập các xưởng", false,
                                "Không ai ra lệnh cả, thị trường tự vận động theo quy luật của nó.")
                        })
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(1, 'A')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("A", "Giờ ta chỉ còn dệt thuê cho Liên Minh thôi.") }
            };

            MainDialogues[Key(1, 'B')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("B", "Ta là Vance, chủ Liên Minh Dệt. Ta kiểm soát gần như toàn bộ giá vải trong vùng."),
                    new DialogueLine("B", "Cạnh tranh? Ta đã xóa sổ nó từ lâu rồi, không còn ai dám đối đầu với ta nữa!"),
                    new DialogueLine("B", "Cháu nghĩ cạnh tranh đã thực sự biến mất chưa?",
                        new List<DialogueChoice>
                        {
                            new DialogueChoice("Chưa — nó chuyển sang hình thức mới, giữa các liên minh và cả bên trong liên minh", true,
                                "(Vance cau mày) Hình thức mới ư? Đừng ảo tưởng, cháu."),
                            new DialogueChoice("Đúng vậy, độc quyền thì cạnh tranh coi như đã chết hẳn", false,
                                "Không đúng đâu — nếu vậy thì tại sao các liên minh khác vẫn đang tìm cách lấn sân?"),
                            new DialogueChoice("Cạnh tranh chỉ biến mất tạm thời rồi sẽ quay lại y như cũ", false,
                                "Gần đúng nhưng chưa đủ — nó không quay lại y như cũ, mà biến đổi sang dạng khác hẳn.")
                        })
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(1, 'B')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("B", "Đừng làm phiền ta, ta đang bận đếm lợi nhuận.") }
            };

            MainDialogues[Key(1, 'C')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("C", "Ta là chủ quán trọ ở đây, chứng kiến hết mọi chuyện trong thị trấn này."),
                    new DialogueLine("C", "Vance nói đã xóa sổ cạnh tranh, nhưng ta thấy các liên minh khác vẫn ngấm ngầm tranh khách với hắn."),
                    new DialogueLine("C", "Ngay cả những người trong Liên Minh Dệt cũng ngầm ganh đua để giành phần chia lớn hơn."),
                    new DialogueLine("!", "Bạn hiểu ra: độc quyền không thủ tiêu cạnh tranh, mà chỉ khiến nó trở nên đa dạng và gay gắt hơn.")
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(1, 'C')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("C", "Quán trọ vẫn còn phòng trống nếu cháu cần nghỉ chân.") }
            };
        }

        static void BuildMap2Dialogues()
        {
            MainDialogues[Key(2, 'A')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("A", "Ta là Aurel, chủ ngân hàng lớn nhất Ngân Thành."),
                    new DialogueLine("A", "Ngân hàng của ta không chỉ cho vay — ta còn nắm cổ phần và cử người vào hội đồng quản trị của nhiều xưởng sản xuất."),
                    new DialogueLine("A", "Khi vốn ngân hàng và vốn công nghiệp hòa làm một, người ta gọi đó là gì, cháu biết không?",
                        new List<DialogueChoice>
                        {
                            new DialogueChoice("Tư bản tài chính — sự kết hợp giữa vốn ngân hàng và vốn sản xuất", true,
                                "Chính xác! Đó là một trong những biểu hiện quan trọng nhất của độc quyền."),
                            new DialogueChoice("Đơn giản chỉ là ngân hàng cho vay tiền", false,
                                "Không chỉ vậy đâu — nắm cổ phần và ghế hội đồng quản trị là mức độ kiểm soát sâu hơn nhiều."),
                            new DialogueChoice("Đó là hình thức từ thiện của ngân hàng", false,
                                "(Aurel cười lớn) Từ thiện? Đây là quyền lực kinh tế thực sự, cháu ạ.")
                        })
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(2, 'A')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("A", "Ngân hàng luôn mở cửa cho những ai cần vốn.") }
            };

            MainDialogues[Key(2, 'B')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("B", "Ta là Sable, thương nhân viễn xứ. Ta vừa chuyển một khoản vốn lớn từ đây sang Quần Đảo Gió."),
                    new DialogueLine("B", "Đầu tư xưởng đóng tàu ở đó, lợi nhuận cao gấp ba lần so với đầu tư trong nước."),
                    new DialogueLine("B", "Cháu có biết hành động này của ta gọi là gì không?",
                        new List<DialogueChoice>
                        {
                            new DialogueChoice("Xuất khẩu tư bản — mang vốn ra nước ngoài để tìm lợi nhuận cao hơn", true,
                                "Đúng vậy! Khi vốn trong nước đã dư thừa, tư bản độc quyền tìm nơi khác để sinh lời nhiều hơn."),
                            new DialogueChoice("Chỉ là buôn bán hàng hóa bình thường", false,
                                "Không phải hàng hóa đâu, ta mang chính đồng vốn đi đầu tư ở nơi khác."),
                            new DialogueChoice("Ta đang đi du lịch kết hợp công tác", false,
                                "(Sable cười) Không hề, đây là một quyết định đầu tư tính toán kỹ lưỡng.")
                        })
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(2, 'B')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("B", "Ta sắp lên đường sang Quần Đảo Gió rồi.") }
            };

            MainDialogues[Key(2, 'C')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("C", "Ta là Piu, ký lục viên ở đây. Ta vừa ghi chép một cuộc họp kín tối qua."),
                    new DialogueLine("C", "Bốn thương hội lớn nhất vùng đã ngồi lại, chia nhau từng vùng biển để buôn bán."),
                    new DialogueLine("C", "Họ cam kết không cạnh tranh trên địa bàn của nhau. Cháu gọi đây là hành vi gì?",
                        new List<DialogueChoice>
                        {
                            new DialogueChoice("Phân chia thị trường giữa các tổ chức độc quyền", true,
                                "Đúng rồi! Đây là cách các thế lực độc quyền phối hợp để tránh cạnh tranh lẫn nhau, chia nhau lợi nhuận."),
                            new DialogueChoice("Chỉ là một buổi họp mặt bạn bè bình thường", false,
                                "Không hề bình thường — họ đang phân chia quyền lực kinh tế trên bản đồ."),
                            new DialogueChoice("Đó là một hình thức từ thiện chung tay", false,
                                "Hoàn toàn không, đây là thỏa thuận nhằm bảo vệ lợi nhuận của chính họ.")
                        })
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(2, 'C')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("C", "Ta còn phải ghi chép thêm nhiều sổ sách nữa.") }
            };
        }

        static void BuildMap3Dialogues()
        {
            MainDialogues[Key(3, 'A')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("A", "Ta là một nhà hiền triết mù, nhưng ta 'thấy' rõ hơn ai hết những ngộ nhận trong thung lũng này."),
                    new DialogueLine("A", "Cư dân ở đây tin vào những điều sai lầm về thị trường, tưởng như là chân lý."),
                    new DialogueLine("A", "Hãy đi gặp Mira và Talos, nghe họ nói, rồi tự mình phán xét đúng sai.")
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(3, 'A')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("A", "Hãy giữ đầu óc tỉnh táo giữa sương mù này.") }
            };

            MainDialogues[Key(3, 'B')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("B", "Ta là Mira. Ta tin rằng độc quyền lúc nào cũng xấu xa, phải bị xóa bỏ hoàn toàn!"),
                    new DialogueLine("B", "Không được để bất kỳ ai nắm thế độc quyền, dù chỉ một ngày!",
                        new List<DialogueChoice>
                        {
                            new DialogueChoice("Độc quyền là kết quả tất yếu của cạnh tranh phát triển cao, nên cần điều tiết chứ không thể xóa bỏ hoàn toàn", true,
                                "(Mira khựng lại) Điều tiết... chứ không xóa bỏ hoàn toàn ư? Ta chưa nghĩ tới điều đó."),
                            new DialogueChoice("Cô nói đúng, phải xóa bỏ độc quyền bằng mọi giá", false,
                                "Nhưng độc quyền sinh ra từ chính quy luật cạnh tranh — xóa bỏ hoàn toàn là điều không tưởng."),
                            new DialogueChoice("Độc quyền hoàn toàn vô hại, không cần lo lắng", false,
                                "Không đúng — độc quyền vẫn có thể gây hại nếu không được kiểm soát.")
                        })
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(3, 'B')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("B", "Sương mù này khiến ta chẳng thấy rõ điều gì cả...") }
            };

            MainDialogues[Key(3, 'C')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("C", "Ta là Talos. Ta tin thị trường nên được tự do tuyệt đối, không ai được can thiệp!"),
                    new DialogueLine("C", "Cứ để mặc cạnh tranh diễn ra tự nhiên, mọi thứ rồi sẽ tự tốt lên thôi!",
                        new List<DialogueChoice>
                        {
                            new DialogueChoice("Tự do tuyệt đối sẽ khiến kẻ mạnh chèn ép kẻ yếu, cần vai trò điều tiết của Nhà nước", true,
                                "(Talos im lặng) Nếu không ai điều tiết... kẻ mạnh nhất sẽ nuốt chửng tất cả sao?"),
                            new DialogueChoice("Đúng vậy, không cần Nhà nước can thiệp gì cả", false,
                                "Nhưng nếu không ai điều tiết, độc quyền mới sẽ liên tục hình thành và chèn ép người yếu thế."),
                            new DialogueChoice("Nhà nước nên kiểm soát toàn bộ giá cả mọi mặt hàng", false,
                                "Đó lại là một thái cực khác — điều tiết không có nghĩa là kiểm soát tuyệt đối mọi thứ.")
                        })
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(3, 'C')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("C", "Sương mù ở đây thật kỳ lạ, phải không?") }
            };
        }

        static void BuildMap4Dialogues()
        {
            MainDialogues[Key(4, 'A')] = new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("A", "Đứng lại! Bên trong là Solandor, Vua Độc Quyền — hắn ta rất giỏi ngụy biện."),
                    new DialogueLine("A", "Hắn sẽ lặp lại đúng những ngộ nhận cháu đã gặp ở Thung Lũng Ảo Ảnh để bảo vệ đặc quyền của mình."),
                    new DialogueLine("A", "Bốn viên Crystal cháu mang theo chính là vũ khí lý luận. Hãy dùng chúng để phản biện!")
                },
                Action = "stage+"
            };
            FlavorDialogues[Key(4, 'A')] = new DialogueDef
            {
                Lines = new List<DialogueLine> { new DialogueLine("A", "Hãy cẩn thận với lý lẽ của hắn.") }
            };
        }

        // ================================================================ HIDDEN LORE
        // Moi map giau mot diem lap lanh — tuong tac de mo khoa cau chuyen an + trang nhat ky.
        static DialogueDef LoreDialogue(string l1, string l2, string l3, string journalId, string toast)
        {
            return new DialogueDef
            {
                Lines = new List<DialogueLine>
                {
                    new DialogueLine("!", l1),
                    new DialogueLine("!", l2),
                    new DialogueLine("!", l3)
                },
                Action = "journal:" + journalId + ";toast:" + toast
            };
        }

        public static DialogueDef GetLore(int mapIndex)
        {
            switch (mapIndex)
            {
                case 0:
                    return LoreDialogue(
                        "Sau đống thùng gỗ cũ, bạn tìm thấy một tấm bia đá mờ chữ...",
                        "\"Thuở xưa, chợ này chỉ có trao đổi hàng lấy hàng. Người có rau muốn cá, người có cá lại muốn vải — ai cũng khổ vì chẳng mấy khi khớp nhau.\"",
                        "\"Rồi một thương nhân già nghĩ ra việc dùng vỏ sò làm vật trung gian được mọi người tin nhận. Đồng tiền đầu tiên của Econia ra đời — và từ đó cạnh tranh mới có một thước đo chung.\"",
                        "lore_market", "Mở khóa câu chuyện ẩn: Đồng tiền đầu tiên!");
                case 1:
                    return LoreDialogue(
                        "Trong góc khuất thị trấn, bạn nhặt được một con thoi dệt gãy có khắc tên \"Mette\"...",
                        "\"Mette là người thợ dệt cuối cùng từ chối gia nhập Liên Minh. Ai cũng bảo bà sẽ chết đói — nhưng bà chuyển sang dệt hoa văn cổ mà máy móc của Liên Minh không làm nổi.\"",
                        "\"Khách quý tìm đến tận nhà bà đặt hàng. Hóa ra ngay dưới bóng độc quyền, cạnh tranh vẫn tìm được khe hở để sống — bằng sự khác biệt.\"",
                        "lore_guild", "Mở khóa câu chuyện ẩn: Người thợ dệt cuối cùng!");
                case 2:
                    return LoreDialogue(
                        "Dưới chân tường Ngân Thành, bạn đào được một cuốn sổ cái cháy dở...",
                        "\"Đây là sổ của Ngân hàng Vàng Ròng — từng lớn nhất Econia. Họ đổ tiền cho vay vào những chuyến buôn viễn dương đầy rủi ro để kiếm lời gấp mười.\"",
                        "\"Một mùa bão đánh chìm cả đội thuyền, ngân hàng sụp đổ kéo theo hàng nghìn xưởng thợ. Từ đó dân Ngân Thành hiểu: quyền lực của tư bản tài chính lớn đến đâu, rủi ro nó gieo xuống xã hội lớn đến đó — nếu không ai giám sát.\"",
                        "lore_bank", "Mở khóa câu chuyện ẩn: Nhà băng sụp đổ!");
                case 3:
                    return LoreDialogue(
                        "Giữa sương mù, bạn chạm vào một mảnh gương vỡ còn vang vọng tiếng nói...",
                        "\"Kẻ tạo ra thung lũng ảo ảnh này không phải phù thủy — mà là một thương nhân xảo quyệt. Hắn gieo rắc những ngộ nhận về thị trường để dân chúng tự trói tay mình.\"",
                        "\"Người tin 'độc quyền phải xóa sạch' thì đập phá lẫn nhau; người tin 'thị trường tự lo được hết' thì để mặc hắn thâu tóm. Ảo ảnh nguy hiểm nhất không nằm trong sương — mà nằm trong đầu người không chịu suy xét.\"",
                        "lore_valley", "Mở khóa câu chuyện ẩn: Kẻ gieo ảo ảnh!");
                default:
                    return LoreDialogue(
                        "Trong góc tối cung điện, bạn tìm thấy một bức chân dung phủ bụi vẽ một cậu bé bán muối...",
                        "\"Đó là Solandor thuở hàn vi. Gia đình cậu từng bị một thương hội độc quyền chèn ép đến phá sản; cha cậu mất trong nghèo khó.\"",
                        "\"Cậu bé thề sẽ trở nên lớn mạnh hơn kẻ đã nghiền nát nhà mình — và rồi trở thành chính thứ mình từng căm ghét. Độc quyền không được điều tiết là một vòng lặp: nạn nhân hôm nay có thể là bạo chúa ngày mai.\"",
                        "lore_palace", "Mở khóa câu chuyện ẩn: Quá khứ của Solandor!");
            }
        }

        // ================================================================ PUZZLES
        public static PuzzleDef GetPuzzle(int mapIndex)
        {
            switch (mapIndex)
            {
                case 0:
                    return new PuzzleDef
                    {
                        Type = PuzzleType.Classify,
                        Title = "Phân loại: Cạnh tranh hay Độc quyền?",
                        Instructions = "Xếp mỗi tình huống vào đúng nhóm.",
                        Categories = new List<string> { "Cạnh tranh", "Độc quyền" },
                        Items = new List<PuzzleItem>
                        {
                            new PuzzleItem("Hàng chục người bán rau tranh nhau mời khách", "Cạnh tranh"),
                            new PuzzleItem("Chỉ một thương nhân được cấp phép bán muối", "Độc quyền"),
                            new PuzzleItem("Các sạp rau liên tục giảm giá để hút khách", "Cạnh tranh"),
                            new PuzzleItem("Không ai khác được phép mở sạp bán muối", "Độc quyền"),
                            new PuzzleItem("Người bán phải cải thiện chất lượng để không mất khách", "Cạnh tranh"),
                            new PuzzleItem("Khách hàng không có lựa chọn nào khác ngoài mua muối của Ông Trắng", "Độc quyền")
                        }
                    };
                case 1:
                    return new PuzzleDef
                    {
                        Type = PuzzleType.Order,
                        Title = "Sắp xếp: Từ cạnh tranh đến độc quyền",
                        Instructions = "Sắp xếp các bước theo đúng trình tự diễn ra ở Guild Town.",
                        Steps = new List<string>
                        {
                            "Nhiều xưởng dệt nhỏ cạnh tranh khốc liệt bằng giá và chất lượng",
                            "Xưởng thắng thế tích lũy vốn và mở rộng quy mô",
                            "Các xưởng yếu dần bị thâu tóm hoặc sáp nhập",
                            "Liên minh dệt lớn hình thành, kiểm soát phần lớn thị trường",
                            "Cạnh tranh không mất đi mà chuyển sang hình thức mới giữa các liên minh"
                        }
                    };
                case 2:
                    return new PuzzleDef
                    {
                        Type = PuzzleType.Classify,
                        Title = "Phân loại biểu hiện của độc quyền",
                        Instructions = "Xếp mỗi sự kiện vào đúng biểu hiện của độc quyền.",
                        Categories = new List<string> { "Tư bản tài chính", "Xuất khẩu tư bản", "Phân chia thị trường" },
                        Items = new List<PuzzleItem>
                        {
                            new PuzzleItem("Ngân hàng nắm cổ phần và cử người vào hội đồng quản trị xưởng sản xuất", "Tư bản tài chính"),
                            new PuzzleItem("Thương nhân mang vốn sang Quần Đảo Gió đầu tư xưởng đóng tàu", "Xuất khẩu tư bản"),
                            new PuzzleItem("Bốn thương hội họp kín, chia nhau từng vùng biển buôn bán", "Phân chia thị trường"),
                            new PuzzleItem("Vốn ngân hàng và vốn sản xuất kết hợp thành một khối quyền lực", "Tư bản tài chính"),
                            new PuzzleItem("Lợi nhuận đầu tư ở vùng đất khác cao hơn hẳn trong nước", "Xuất khẩu tư bản"),
                            new PuzzleItem("Các bên cam kết không cạnh tranh trên địa bàn của nhau", "Phân chia thị trường")
                        }
                    };
                case 3:
                    return new PuzzleDef
                    {
                        Type = PuzzleType.Quiz,
                        Title = "Bắt lỗi ngộ nhận thị trường",
                        Instructions = "Chọn phản biện đúng cho mỗi ngộ nhận.",
                        Quiz = new List<QuizQuestion>
                        {
                            new QuizQuestion
                            {
                                Statement = "\"Độc quyền lúc nào cũng xấu, phải xóa bỏ hoàn toàn.\"",
                                Options = new List<string>
                                {
                                    "Độc quyền là kết quả tất yếu của cạnh tranh phát triển cao, cần điều tiết chứ không thể xóa bỏ hoàn toàn",
                                    "Đúng, phải xóa bỏ độc quyền bằng mọi giá",
                                    "Độc quyền hoàn toàn vô hại"
                                },
                                CorrectIndex = 0,
                                Feedback = new List<string>
                                {
                                    "Chính xác — độc quyền sinh ra từ cạnh tranh, chỉ có thể điều tiết chứ không xóa bỏ hoàn toàn.",
                                    "Sai — xóa bỏ hoàn toàn là điều không tưởng vì nó sinh ra từ chính quy luật cạnh tranh.",
                                    "Sai — độc quyền vẫn có thể gây hại nếu không được kiểm soát."
                                }
                            },
                            new QuizQuestion
                            {
                                Statement = "\"Cứ để thị trường tự do tuyệt đối, không cần ai can thiệp, mọi thứ sẽ tự tốt lên.\"",
                                Options = new List<string>
                                {
                                    "Đúng, không cần Nhà nước can thiệp gì cả",
                                    "Tự do tuyệt đối sẽ khiến kẻ mạnh chèn ép kẻ yếu, cần vai trò điều tiết của Nhà nước",
                                    "Nhà nước nên kiểm soát toàn bộ giá cả mọi mặt hàng"
                                },
                                CorrectIndex = 1,
                                Feedback = new List<string>
                                {
                                    "Sai — không điều tiết, độc quyền mới sẽ liên tục hình thành và chèn ép người yếu thế.",
                                    "Chính xác — cần vai trò điều tiết của Nhà nước để bảo vệ cạnh tranh lành mạnh.",
                                    "Sai — điều tiết không đồng nghĩa với kiểm soát tuyệt đối mọi thứ."
                                }
                            },
                            new QuizQuestion
                            {
                                Statement = "\"Một khi đã có độc quyền thì cạnh tranh coi như đã chết hẳn.\"",
                                Options = new List<string>
                                {
                                    "Đúng, độc quyền hình thành thì cạnh tranh chấm dứt",
                                    "Cạnh tranh chỉ tạm ngừng rồi sẽ quay lại y như cũ",
                                    "Cạnh tranh không mất đi mà chuyển sang hình thức mới, kể cả trong lòng độc quyền"
                                },
                                CorrectIndex = 2,
                                Feedback = new List<string>
                                {
                                    "Sai — các tổ chức độc quyền vẫn cạnh tranh với nhau và cạnh tranh ngầm bên trong.",
                                    "Chưa đủ — nó không quay lại y như cũ mà biến đổi sang dạng khác hẳn.",
                                    "Chính xác — độc quyền không thủ tiêu cạnh tranh mà làm nó đa dạng, gay gắt hơn."
                                }
                            }
                        }
                    };
                default:
                    return null;
            }
        }

        // ================================================================ BOSS
        public static readonly List<BossRound> BossRounds = new List<BossRound>
        {
            new BossRound
            {
                Statement = "\"Độc quyền là đỉnh cao, cạnh tranh đã chết dưới chân ta!\"",
                Options = new List<string>
                {
                    "Độc quyền sinh ra từ cạnh tranh, nhưng cạnh tranh không hề mất đi mà biến đổi sang hình thức mới, gay gắt hơn",
                    "Ngươi nói đúng, cạnh tranh đã hoàn toàn biến mất",
                    "Cạnh tranh chỉ là chuyện của những kẻ yếu"
                },
                CorrectIndex = 0,
                Feedback = new List<string>
                {
                    "Đúng! Solandor loạng choạng lùi lại.",
                    "Sai — nếu cạnh tranh đã chết, sao ngươi vẫn phải đề phòng những thế lực khác?",
                    "Sai — cạnh tranh là quy luật khách quan, không phân biệt mạnh yếu."
                }
            },
            new BossRound
            {
                Statement = "\"Ta không cần ai kiểm soát, thị trường tự nó sẽ công bằng!\"",
                Options = new List<string>
                {
                    "Không có điều tiết, độc quyền sẽ chèn ép người tiêu dùng và doanh nghiệp nhỏ, cần vai trò của Nhà nước",
                    "Đúng, thị trường luôn tự công bằng mà không cần ai can thiệp",
                    "Chỉ cần ngươi tự giác là đủ, không cần luật lệ"
                },
                CorrectIndex = 0,
                Feedback = new List<string>
                {
                    "Solandor rú lên đau đớn — lý lẽ ngươi quá sắc bén!",
                    "Sai — không có ai giám sát, kẻ mạnh nhất luôn tìm cách chèn ép kẻ yếu.",
                    "Sai — lịch sử chứng minh 'tự giác' không đủ để ngăn lạm dụng độc quyền."
                }
            },
            new BossRound
            {
                Statement = "\"Nhà nước can thiệp sẽ phá vỡ mọi quy luật thị trường!\"",
                Options = new List<string>
                {
                    "Ngươi nói đúng, Nhà nước không nên can thiệp gì cả",
                    "Nhà nước không xóa bỏ quy luật thị trường mà điều tiết để cạnh tranh lành mạnh, hài hòa lợi ích",
                    "Nhà nước nên thay thế hoàn toàn thị trường"
                },
                CorrectIndex = 1,
                Feedback = new List<string>
                {
                    "Sai — không điều tiết, ngươi sẽ tự do lạm dụng vị thế độc quyền mãi mãi.",
                    "Chính xác! Solandor không còn lời nào để đáp trả.",
                    "Sai — điều tiết không có nghĩa là xóa bỏ thị trường."
                }
            },
            new BossRound
            {
                Statement = "\"Kinh tế thị trường thì không thể có vai trò Nhà nước mạnh!\"",
                Options = new List<string>
                {
                    "Kinh tế thị trường định hướng xã hội chủ nghĩa ở Việt Nam vẫn có Nhà nước kiểm soát độc quyền, bảo vệ cạnh tranh lành mạnh",
                    "Đúng, Nhà nước và thị trường không thể cùng tồn tại",
                    "Nhà nước chỉ nên đứng ngoài quan sát"
                },
                CorrectIndex = 0,
                Feedback = new List<string>
                {
                    "Solandor run rẩy — thực tiễn Việt Nam đã chứng minh điều ngược lại với hắn!",
                    "Sai — nhiều nền kinh tế thị trường vẫn có vai trò điều tiết mạnh của Nhà nước.",
                    "Sai — đứng ngoài quan sát sẽ để độc quyền tự do lộng hành."
                }
            },
            new BossRound
            {
                Statement = "\"Ngươi không thể chứng minh được điều gì cả!\"",
                Options = new List<string>
                {
                    "Thực tiễn đã chứng minh: cạnh tranh và độc quyền thống nhất và đấu tranh với nhau, được điều tiết để thúc đẩy phát triển",
                    "Ngươi nói đúng, ta chịu thua",
                    "Ta chỉ đang đoán mò thôi"
                },
                CorrectIndex = 0,
                Feedback = new List<string>
                {
                    "Solandor sụp đổ! Nguyên lý kinh tế chính trị đã chiến thắng ngụy biện.",
                    "Đừng nản lòng — mọi lý lẽ trước đó của cháu đều đúng, hãy giữ vững lập trường!",
                    "Không phải đoán mò — đây là quy luật khách quan đã được kiểm chứng qua thực tiễn."
                }
            }
        };

        // ================================================================ JOURNAL
        public static readonly List<JournalEntry> Journal = new List<JournalEntry>
        {
            new JournalEntry { Id = "canh_tranh", Title = "Cạnh tranh", Body =
                "Cạnh tranh là sự ganh đua giữa những người sản xuất, kinh doanh nhằm giành giật những điều kiện thuận lợi để thu lợi nhuận cao hơn. " +
                "Khi có nhiều người bán cùng một loại hàng hóa, họ buộc phải giảm giá, nâng cao chất lượng để giữ khách. " +
                "Ví dụ: các sạp rau trong chợ liên tục giảm giá vì có quá nhiều người bán cùng mặt hàng." },
            new JournalEntry { Id = "doc_quyen", Title = "Độc quyền", Body =
                "Độc quyền là tình trạng một hoặc một nhóm nhỏ chủ thể kiểm soát toàn bộ việc sản xuất, cung ứng một loại hàng hóa, dịch vụ, khiến họ không còn chịu áp lực cạnh tranh trực tiếp. " +
                "Độc quyền thường hình thành từ quá trình tích tụ, tập trung vốn sau một giai đoạn cạnh tranh gay gắt. " +
                "Ví dụ: Ông Muối Trắng là người duy nhất được cấp phép bán muối trong vùng." },
            new JournalEntry { Id = "hinh_thuc_moi", Title = "Cạnh tranh dưới hình thức mới", Body =
                "Độc quyền không thủ tiêu cạnh tranh mà khiến nó trở nên đa dạng, gay gắt hơn. " +
                "Cạnh tranh vẫn tiếp diễn: giữa các tổ chức độc quyền với nhau, giữa tổ chức độc quyền với các doanh nghiệp ngoài, và cả cạnh tranh ngầm bên trong một tổ chức độc quyền. " +
                "Đây chính là quy luật thống nhất và đấu tranh giữa cạnh tranh và độc quyền." },
            new JournalEntry { Id = "tich_tu_tap_trung", Title = "Tích tụ và tập trung sản xuất", Body =
                "Tích tụ là quá trình một chủ thể kinh tế tăng dần quy mô vốn từ lợi nhuận thu được. " +
                "Tập trung là quá trình hợp nhất nhiều chủ thể nhỏ thành một chủ thể lớn hơn, thường thông qua cạnh tranh, sáp nhập hoặc thâu tóm. " +
                "Đây là bước đệm trực tiếp dẫn tới sự hình thành độc quyền." },
            new JournalEntry { Id = "tu_ban_tai_chinh", Title = "Tư bản tài chính", Body =
                "Tư bản tài chính là sự kết hợp giữa tư bản ngân hàng và tư bản công nghiệp, khi ngân hàng không chỉ cho vay mà còn nắm cổ phần, cử người vào hội đồng quản trị của các doanh nghiệp sản xuất. " +
                "Đây là một trong những biểu hiện quan trọng nhất của chủ nghĩa tư bản độc quyền theo lý luận của V.I. Lênin." },
            new JournalEntry { Id = "xuat_khau_tu_ban", Title = "Xuất khẩu tư bản", Body =
                "Xuất khẩu tư bản là việc mang vốn đầu tư ra nước ngoài (hoặc vùng đất khác) nhằm tìm kiếm lợi nhuận cao hơn so với đầu tư trong nước. " +
                "Khác với xuất khẩu hàng hóa thông thường, ở đây chính đồng vốn được đưa đi sinh lời ở nơi khác. " +
                "Đây là một đặc điểm kinh tế quan trọng của chủ nghĩa tư bản độc quyền." },
            new JournalEntry { Id = "phan_chia_thi_truong", Title = "Phân chia thị trường thế giới", Body =
                "Các tổ chức độc quyền lớn thường thỏa thuận phân chia khu vực ảnh hưởng, cam kết không cạnh tranh trên địa bàn của nhau để cùng bảo vệ lợi nhuận. " +
                "Đây là biểu hiện của sự liên kết giữa các thế lực độc quyền nhằm giảm thiểu rủi ro từ cạnh tranh lẫn nhau." },
            new JournalEntry { Id = "dieu_tiet_nha_nuoc", Title = "Vai trò điều tiết của Nhà nước", Body =
                "Vì cạnh tranh tự do tuyệt đối có thể dẫn tới độc quyền lạm dụng vị thế, Nhà nước cần đóng vai trò điều tiết: kiểm soát độc quyền, bảo vệ cạnh tranh lành mạnh, hài hòa lợi ích giữa doanh nghiệp và xã hội. " +
                "Ở Việt Nam, đây là một đặc trưng của nền kinh tế thị trường định hướng xã hội chủ nghĩa." },
            new JournalEntry { Id = "ngo_nhan_thi_truong", Title = "Những ngộ nhận phổ biến về thị trường", Body =
                "Có hai ngộ nhận thường gặp: (1) cho rằng độc quyền lúc nào cũng xấu và phải xóa bỏ hoàn toàn, và (2) cho rằng thị trường nên tự do tuyệt đối, không cần ai can thiệp. " +
                "Cả hai quan điểm đều cực đoan — thực tế cần có sự điều tiết hợp lý, không xóa bỏ hoàn toàn cũng không thả nổi tuyệt đối." },
            new JournalEntry { Id = "lore_market", Title = "Chuyện ẩn: Đồng tiền đầu tiên", Body =
                "Tấm bia đá cổ ở Market of Many Hands kể rằng chợ xưa chỉ có trao đổi hàng lấy hàng, bất tiện vì hiếm khi nhu cầu hai bên khớp nhau. " +
                "Khi một vật trung gian được mọi người tin nhận xuất hiện (vỏ sò — tiền tệ sơ khai), trao đổi bùng nổ và cạnh tranh mới có thước đo chung. " +
                "Bài học: tiền tệ ra đời từ chính nhu cầu của lưu thông hàng hóa, không phải do ai áp đặt." },
            new JournalEntry { Id = "lore_guild", Title = "Chuyện ẩn: Người thợ dệt cuối cùng", Body =
                "Bà Mette từ chối gia nhập Liên Minh Dệt và sống sót bằng cách làm ra thứ mà tổ chức độc quyền không làm được — hoa văn thủ công độc bản. " +
                "Câu chuyện minh họa: độc quyền không thể bịt kín mọi khe hở của thị trường; cạnh tranh len lỏi trở lại qua sự khác biệt hóa sản phẩm." },
            new JournalEntry { Id = "lore_bank", Title = "Chuyện ẩn: Nhà băng sụp đổ", Body =
                "Ngân hàng Vàng Ròng dồn vốn vào các chuyến buôn rủi ro cao và sụp đổ chỉ sau một mùa bão, kéo theo hàng nghìn xưởng thợ phụ thuộc vốn vay. " +
                "Câu chuyện cho thấy mặt trái của tư bản tài chính: khi vốn ngân hàng chi phối cả nền sản xuất, một cú sập của nó trở thành thảm họa toàn xã hội — vì vậy cần sự giám sát, điều tiết." },
            new JournalEntry { Id = "lore_valley", Title = "Chuyện ẩn: Kẻ gieo ảo ảnh", Body =
                "Thung lũng ảo ảnh do một thương nhân tạo ra bằng cách gieo rắc ngộ nhận: kẻ tin 'phải xóa sạch độc quyền' và kẻ tin 'thị trường tự lo được hết' đều vô tình phục vụ hắn. " +
                "Bài học: nhận thức sai về quy luật kinh tế cũng là một công cụ thao túng — tư duy phản biện là lớp phòng vệ đầu tiên." },
            new JournalEntry { Id = "lore_palace", Title = "Chuyện ẩn: Quá khứ của Solandor", Body =
                "Solandor từng là nạn nhân của độc quyền: gia đình phá sản vì bị thương hội chèn ép. Trở nên hùng mạnh, hắn lặp lại đúng những gì mình từng căm ghét. " +
                "Câu chuyện cho thấy: nếu không có điều tiết, độc quyền là một vòng lặp tự tái sinh — thay người cầm quyền chứ không thay bản chất." },
            new JournalEntry { Id = "tong_hop_quy_luat", Title = "Tổng hợp: Quy luật thống nhất và đấu tranh giữa cạnh tranh và độc quyền", Body =
                "Cạnh tranh sinh ra độc quyền, nhưng độc quyền không thủ tiêu cạnh tranh mà làm nó trở nên đa dạng và gay gắt hơn — đây là quy luật thống nhất và đấu tranh khách quan trong nền kinh tế thị trường. " +
                "Nhà nước, thông qua vai trò điều tiết của mình, không xóa bỏ quy luật này mà định hướng nó để phục vụ sự phát triển hài hòa của xã hội." }
        };

        public static string JournalTitle(string id)
        {
            foreach (var j in Journal) if (j.Id == id) return j.Title;
            return id;
        }

        public static JournalEntry GetJournal(string id)
        {
            foreach (var j in Journal) if (j.Id == id) return j;
            return null;
        }

        // ================================================================ ITEMS
        public static readonly Dictionary<string, ItemDef> Items = new Dictionary<string, ItemDef>
        {
            ["muoi_phep"] = new ItemDef { Id = "muoi_phep", Name = "Muối Phép", Desc = "Một túi muối nhỏ từ Ông Muối Trắng, lấp lánh kỳ lạ.", Icon = "bag", Color = new Color(0.9f, 0.9f, 0.95f) },
            ["soi_det"] = new ItemDef { Id = "soi_det", Name = "Cuộn Chỉ Dệt", Desc = "Món quà từ Lina, nhắc nhớ về những xưởng dệt nhỏ đã từng tồn tại.", Icon = "scroll", Color = new Color(0.7f, 0.4f, 0.3f) },
            ["so_ghi_chep"] = new ItemDef { Id = "so_ghi_chep", Name = "Sổ Ghi Chép Ngân Thành", Desc = "Cuốn sổ của ký lục viên Piu, ghi lại các giao dịch bí mật.", Icon = "book", Color = new Color(0.3f, 0.3f, 0.4f) },
            ["kinh_soi"] = new ItemDef { Id = "kinh_soi", Name = "Kính Soi Ảo Ảnh", Desc = "Giúp nhìn xuyên qua những ngộ nhận về thị trường.", Icon = "lens", Color = new Color(0.6f, 0.8f, 0.9f) },
            ["huy_hieu_dieu_tiet"] = new ItemDef { Id = "huy_hieu_dieu_tiet", Name = "Huy Hiệu Điều Tiết", Desc = "Biểu tượng chiến thắng sau trận đối chất với Solandor.", Icon = "book", Color = new Color(0.9f, 0.75f, 0.25f) }
        };
    }
}
