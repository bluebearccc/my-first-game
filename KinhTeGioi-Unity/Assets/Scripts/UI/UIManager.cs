using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KTG
{
    public class UIManager : MonoBehaviour
    {
        static readonly Color PanelFill = new Color(0.08f, 0.09f, 0.16f, 0.95f);
        static readonly Color PanelBorder = new Color(0.82f, 0.68f, 0.32f, 1f);
        static readonly Color Gold = new Color(0.88f, 0.74f, 0.35f);
        static readonly Color Ink = new Color(0.93f, 0.9f, 0.82f);
        static readonly Color Cyan = new Color(0.45f, 0.85f, 0.9f);

        Canvas canvas;

        GameObject mainMenuPanel;
        Button continueBtn;

        GameObject hudPanel;
        Text objectiveText;
        Image[] crystalIcons;

        GameObject dialoguePanel;
        Image portraitImg;
        Text nameText;
        Text dialogueText;
        Text feedbackText;
        Text backHint;
        Transform choicesRoot;
        readonly List<GameObject> choiceButtons = new List<GameObject>();
        Coroutine dialogueRoutine;

        GameObject journalPanel;
        Transform journalListRoot;
        Text journalContentTitle;
        Text journalContentBody;

        GameObject inventoryPanel;
        Transform inventoryGridRoot;

        GameObject pausePanel;

        GameObject endingPanel;

        CanvasGroup mapTitleGroup;
        Text mapTitleText;
        Coroutine mapTitleRoutine;

        CanvasGroup toastGroup;
        Text toastText;
        Coroutine toastRoutine;

        Image fadeImage;

        public PuzzleUI Puzzle { get; private set; }
        public BossUI Boss { get; private set; }

        public void Build()
        {
            canvas = UIFactory.CreateCanvas("UI");
            BuildMainMenu();
            BuildHud();
            BuildDialogue();
            BuildJournal();
            BuildInventory();
            BuildPause();
            BuildEnding();
            BuildMapTitle();
            BuildToast();

            var puzzleGO = new GameObject("PuzzleUI");
            puzzleGO.transform.SetParent(canvas.transform, false);
            Puzzle = puzzleGO.AddComponent<PuzzleUI>();
            Puzzle.Build();

            var bossGO = new GameObject("BossUI");
            bossGO.transform.SetParent(canvas.transform, false);
            Boss = bossGO.AddComponent<BossUI>();
            Boss.Build();

            BuildFade();

            hudPanel.SetActive(false);
        }

        void Update()
        {
            if (GameManager.Instance == null) return;
            var state = GameManager.Instance.State;
            if (state == GameState.Explore)
            {
                if (Input.GetKeyDown(KeyCode.I)) OpenInventory();
                else if (Input.GetKeyDown(KeyCode.J)) OpenJournal();
                else if (Input.GetKeyDown(KeyCode.Escape)) OpenPause();
            }
            else if (state == GameState.Paused)
            {
                if (inventoryPanel.activeSelf && (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))) CloseInventory();
                else if (journalPanel.activeSelf && (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Escape))) CloseJournal();
                else if (pausePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape)) ClosePause();
            }
        }

        // ------------------------------------------------------------ MAIN MENU
        void BuildMainMenu()
        {
            mainMenuPanel = new GameObject("MainMenu", typeof(RectTransform));
            var rt = (RectTransform)mainMenuPanel.transform;
            rt.SetParent(canvas.transform, false);
            UIFactory.Stretch(rt);

            var bg = UIFactory.CreateImage(rt, "Bg", PixelArt.VGradient(new Color(0.1f, 0.12f, 0.22f), new Color(0.03f, 0.03f, 0.07f)), new Vector2(1280, 720));
            UIFactory.Stretch(bg.rectTransform);

            var glow = UIFactory.CreateImage(rt, "Glow", PixelArt.Glow(new Color(0.5f, 0.8f, 1f, 0.35f), 128), new Vector2(700, 700));
            glow.rectTransform.anchoredPosition = new Vector2(0, 60);

            var title = UIFactory.CreateText(rt, "Title", "KINH TẾ GIỚI", 56, Gold, TextAnchor.MiddleCenter, new Vector2(900, 90));
            title.fontStyle = FontStyle.Bold;
            title.rectTransform.anchoredPosition = new Vector2(0, 210);

            var subtitle = UIFactory.CreateText(rt, "Subtitle", "Hành trình qua năm vùng đất kinh tế", 20, Ink, TextAnchor.MiddleCenter, new Vector2(900, 40));
            subtitle.rectTransform.anchoredPosition = new Vector2(0, 150);

            // 5 vien crystal bay lo lung — dai dien 5 vung dat
            Color[] crystalCols =
            {
                new Color(0.92f, 0.62f, 0.4f), new Color(0.62f, 0.8f, 0.42f), new Color(0.42f, 0.82f, 0.9f),
                new Color(0.78f, 0.52f, 0.9f), new Color(0.95f, 0.8f, 0.38f)
            };
            for (int i = 0; i < crystalCols.Length; i++)
            {
                var ci = UIFactory.CreateImage(rt, "MenuCrystal" + i, PixelArt.Crystal(crystalCols[i], 32), new Vector2(34, 34));
                ci.rectTransform.anchoredPosition = new Vector2((i - 2) * 84, 102);
                ci.raycastTarget = false;
                var bob = ci.gameObject.AddComponent<Bobber>();
                bob.Amplitude = 6f;
                bob.Speed = 1.4f + i * 0.35f;
            }

            UIFactory.CreateButton(rt, "StartBtn", "Bắt Đầu Mới", new Vector2(280, 52), () => GameManager.Instance.NewGame())
                .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);

            var contBtn = UIFactory.CreateButton(rt, "ContinueBtn", "Tiếp Tục", new Vector2(280, 52), () => GameManager.Instance.ContinueGame());
            contBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);
            continueBtn = contBtn;

            UIFactory.CreateButton(rt, "QuitBtn", "Thoát", new Vector2(280, 52), () => Application.Quit())
                .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -110);

            var help = UIFactory.CreateText(rt, "Help",
                "Di chuyển: WASD / Mũi tên   ·   Tương tác: E hoặc Space   ·   Túi đồ: I   ·   Nhật ký: J   ·   Tạm dừng: Esc",
                16, new Color(0.7f, 0.72f, 0.8f), TextAnchor.MiddleCenter, new Vector2(1100, 30));
            help.rectTransform.anchoredPosition = new Vector2(0, -280);
        }

        public void ShowMainMenu()
        {
            mainMenuPanel.SetActive(true);
            continueBtn.interactable = GameManager.Instance.HasSave();
            hudPanel.SetActive(false);
        }

        public void HideMainMenu() => mainMenuPanel.SetActive(false);

        // ------------------------------------------------------------ HUD
        void BuildHud()
        {
            hudPanel = new GameObject("Hud", typeof(RectTransform));
            var rt = (RectTransform)hudPanel.transform;
            rt.SetParent(canvas.transform, false);
            UIFactory.Stretch(rt);

            // Vignette gio do URP Volume dam nhiem (Phase C) — Image PixelArt.Vignette cu da go
            // vi canvas overlay render sau post, hai lop vignette se chong nhau.

            var objPanel = UIFactory.CreatePanel(rt, "ObjectivePanel", new Vector2(520, 56), PanelFill, PanelBorder);
            UIFactory.SetAnchor(objPanel.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20));
            objectiveText = UIFactory.CreateText(objPanel.transform, "Text", "", 18, Ink, TextAnchor.MiddleLeft, new Vector2(500, 50));
            UIFactory.Stretch(objectiveText.rectTransform);
            objectiveText.rectTransform.offsetMin = new Vector2(14, 4);
            objectiveText.rectTransform.offsetMax = new Vector2(-14, -4);

            // Bang ngoc goc phai-tren (khung go nhu bang Gold/Time cua RPG)
            var crysPanel = UIFactory.CreatePanel(rt, "CrystalPanel", new Vector2(256, 56), PanelFill, PanelBorder);
            UIFactory.SetAnchor(crysPanel.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -20));
            var crysLabel = UIFactory.CreateText(crysPanel.transform, "Label", "Ngọc", 17, Gold, TextAnchor.MiddleLeft, new Vector2(70, 30));
            UIFactory.SetAnchor(crysLabel.rectTransform, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(16, 0));
            crystalIcons = new Image[5];
            for (int i = 0; i < 5; i++)
            {
                var icon = UIFactory.CreateImage(crysPanel.transform, "Crystal" + i, PixelArt.Crystal(new Color(0.4f, 0.4f, 0.5f, 0.5f), 32), new Vector2(28, 28));
                UIFactory.SetAnchor(icon.rectTransform, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(78 + i * 34, 0));
                crystalIcons[i] = icon;
            }

            // 2 nut tron goc trai-duoi: Tui do (I) va Nhat ky (J)
            BuildHudButton(rt, "InvBtn", "bag", new Color(0.85f, 0.7f, 0.4f), "I", new Vector2(24, 24), OpenInventory);
            BuildHudButton(rt, "JourBtn", "book", new Color(0.5f, 0.75f, 0.9f), "J", new Vector2(90, 24), OpenJournal);

            var hint = UIFactory.CreateText(rt, "Hint", "E: Tương tác   I: Túi đồ   J: Nhật ký   Esc: Tạm dừng", 15,
                new Color(0.8f, 0.8f, 0.85f, 0.85f), TextAnchor.MiddleCenter, new Vector2(600, 28));
            UIFactory.SetAnchor(hint.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 16));
        }

        void BuildHudButton(Transform parent, string name, string icon, Color iconColor, string key, Vector2 pos, System.Action onClick)
        {
            var btn = UIFactory.CreateButton(parent, name, "", new Vector2(56, 56), () =>
            {
                if (GameManager.Instance.State == GameState.Explore) onClick();
            });
            UIFactory.SetAnchor(btn.GetComponent<RectTransform>(), new Vector2(0, 0), new Vector2(0, 0), pos);
            var iconImg = UIFactory.CreateImage(btn.transform, "Icon", PixelArt.Icon(icon, iconColor), new Vector2(32, 32));
            UIFactory.SetAnchor(iconImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 3));
            iconImg.raycastTarget = false;
            var keyText = UIFactory.CreateText(btn.transform, "Key", key, 12, Gold, TextAnchor.LowerRight, new Vector2(50, 16));
            UIFactory.SetAnchor(keyText.rectTransform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-8, 5));
            keyText.raycastTarget = false;
        }

        public void ShowHUD(bool show) => hudPanel.SetActive(show);

        public void UpdateObjective(string text) => objectiveText.text = text;

        public void UpdateCrystals(bool[] collected)
        {
            for (int i = 0; i < crystalIcons.Length; i++)
            {
                bool has = i < collected.Length && collected[i];
                crystalIcons[i].sprite = PixelArt.Crystal(has ? new Color(0.5f, 0.9f, 1f) : new Color(0.4f, 0.4f, 0.5f, 0.4f), 32);
            }
        }

        // ------------------------------------------------------------ DIALOGUE
        void BuildDialogue()
        {
            var panelImg = UIFactory.CreatePanel(canvas.transform, "Dialogue", new Vector2(1180, 230), PanelFill, PanelBorder);
            UIFactory.SetAnchor(panelImg.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 16));
            dialoguePanel = panelImg.gameObject;

            var portrait = UIFactory.CreatePanel(panelImg.transform, "PortraitBg", new Vector2(120, 120), new Color(0.05f, 0.05f, 0.1f, 0.6f), PanelBorder);
            UIFactory.SetAnchor(portrait.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(16, -16));
            portraitImg = UIFactory.CreateImage(portrait.transform, "Portrait", null, new Vector2(96, 96));
            UIFactory.SetAnchor(portraitImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);

            nameText = UIFactory.CreateText(panelImg.transform, "Name", "", 22, Gold, TextAnchor.MiddleLeft, new Vector2(400, 30));
            nameText.fontStyle = FontStyle.Bold;
            UIFactory.SetAnchor(nameText.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(150, -22));

            dialogueText = UIFactory.CreateText(panelImg.transform, "Text", "", 19, Ink, TextAnchor.UpperLeft, new Vector2(1000, 100));
            UIFactory.SetAnchor(dialogueText.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(150, -58));

            feedbackText = UIFactory.CreateText(panelImg.transform, "Feedback", "", 17, Cyan, TextAnchor.UpperLeft, new Vector2(1000, 60));
            UIFactory.SetAnchor(feedbackText.rectTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(150, 44));
            feedbackText.gameObject.SetActive(false);

            var continueHint = UIFactory.CreateText(panelImg.transform, "ContinueHint", "▶ Space / E", 14, new Color(0.7f, 0.72f, 0.8f), TextAnchor.LowerRight, new Vector2(200, 22));
            UIFactory.SetAnchor(continueHint.rectTransform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-14, 8));

            backHint = UIFactory.CreateText(panelImg.transform, "BackHint", "◀ Q: Câu trước", 14, new Color(0.7f, 0.72f, 0.8f), TextAnchor.LowerLeft, new Vector2(220, 22));
            UIFactory.SetAnchor(backHint.rectTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(150, 8));
            backHint.gameObject.SetActive(false);

            var choicesGO = new GameObject("Choices", typeof(RectTransform));
            choicesRoot = choicesGO.transform;
            choicesRoot.SetParent(panelImg.transform, false);
            UIFactory.SetAnchor((RectTransform)choicesRoot, new Vector2(0, 0), new Vector2(0, 0), new Vector2(150, 12));

            dialoguePanel.SetActive(false);
        }

        public void ShowDialogue(DialogueDef d, System.Action<string> onComplete)
        {
            if (dialogueRoutine != null) StopCoroutine(dialogueRoutine);
            dialogueRoutine = StartCoroutine(DialogueRoutine(d, onComplete));
        }

        IEnumerator DialogueRoutine(DialogueDef d, System.Action<string> onComplete)
        {
            dialoguePanel.SetActive(true);
            int i = 0;
            while (i < d.Lines.Count)
            {
                var line = d.Lines[i];
                SetSpeaker(line.Speaker);
                feedbackText.gameObject.SetActive(false);

                bool hasChoices = line.Choices != null && line.Choices.Count > 0;
                // Chi cho phep quay lai khi con cau doc-duoc phia truoc (khong tua vao cau hoi trac nghiem).
                bool canBack = !hasChoices && PrevReadableLine(d, i) >= 0;
                backHint.gameObject.SetActive(canBack);

                yield return Typewriter(line.Text);

                if (hasChoices)
                {
                    bool correct;
                    do
                    {
                        DialogueChoice picked = null;
                        ShowChoices(line.Choices, c => picked = c);
                        yield return new WaitUntil(() => picked != null);
                        HideChoices();
                        correct = picked.Correct;
                        if (!string.IsNullOrEmpty(picked.Feedback))
                        {
                            feedbackText.text = picked.Feedback;
                            feedbackText.gameObject.SetActive(true);
                            yield return WaitAdvanceKey();
                            feedbackText.gameObject.SetActive(false);
                        }
                        GameManager.Instance.PlaySfx(correct ? "ok" : "no");
                    }
                    while (!correct);
                    i++;
                }
                else
                {
                    bool goBack = false;
                    yield return WaitAdvanceOrBack(canBack, b => goBack = b);
                    if (goBack)
                    {
                        GameManager.Instance.PlaySfx("blip");
                        i = PrevReadableLine(d, i);
                    }
                    else i++;
                }
            }
            backHint.gameObject.SetActive(false);
            dialoguePanel.SetActive(false);
            dialogueRoutine = null;
            onComplete?.Invoke(d.Action);
        }

        // Chi so cau thoai doc-duoc (khong co lua chon) gan nhat truoc vi tri "from"; -1 neu khong co.
        int PrevReadableLine(DialogueDef d, int from)
        {
            for (int k = from - 1; k >= 0; k--)
            {
                var l = d.Lines[k];
                if (l.Choices == null || l.Choices.Count == 0) return k;
            }
            return -1;
        }

        IEnumerator WaitAdvanceKey()
        {
            yield return null;
            while (!(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
                yield return null;
        }

        // Cho nguoi choi bam tien (Space/E) hoac quay lai (Q/Backspace). onResult(true) = quay lai.
        IEnumerator WaitAdvanceOrBack(bool canBack, System.Action<bool> onResult)
        {
            yield return null;
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)) { onResult(false); yield break; }
                if (canBack && (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Backspace))) { onResult(true); yield break; }
                yield return null;
            }
        }

        IEnumerator Typewriter(string text)
        {
            dialogueText.text = "";
            bool skip = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)) skip = true;
                if (skip) { dialogueText.text = text; yield break; }
                dialogueText.text += text[i];
                yield return new WaitForSeconds(0.018f);
            }
            dialogueText.text = text;
        }

        void SetSpeaker(string code)
        {
            if (string.IsNullOrEmpty(code) || code == "!")
            {
                portraitImg.gameObject.SetActive(false);
                nameText.text = "";
                return;
            }
            portraitImg.gameObject.SetActive(true);
            if (code == "@")
            {
                nameText.text = "Bạn";
                portraitImg.sprite = PixelArt.Portrait(PlayerController.Hair, PlayerController.Skin, PlayerController.Shirt);
                return;
            }
            if (code == "X")
            {
                nameText.text = GameContent.BossName;
                portraitImg.sprite = PixelArt.Portrait(new Color(0.85f, 0.7f, 0.2f), new Color(0.9f, 0.8f, 0.6f), new Color(0.3f, 0.1f, 0.4f));
                return;
            }
            var map = GameContent.Maps[GameManager.Instance.CurrentMapIndex];
            if (map.Npcs.TryGetValue(code[0], out var npc))
            {
                nameText.text = npc.Name;
                portraitImg.sprite = PixelArt.Portrait(npc.Hair, npc.Skin, npc.Shirt);
            }
        }

        void ShowChoices(List<DialogueChoice> choices, System.Action<DialogueChoice> onPick)
        {
            ClearChoices();
            for (int i = 0; i < choices.Count; i++)
            {
                var c = choices[i];
                var btn = UIFactory.CreateButton(choicesRoot, "Choice" + i, c.Text, new Vector2(1000, 36), () => onPick(c), 16);
                var rt = btn.GetComponent<RectTransform>();
                UIFactory.SetAnchor(rt, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, i * 40));
                choiceButtons.Add(btn.gameObject);
            }
        }

        void HideChoices() => ClearChoices();

        void ClearChoices()
        {
            foreach (var go in choiceButtons) Destroy(go);
            choiceButtons.Clear();
        }

        // ------------------------------------------------------------ JOURNAL
        void BuildJournal()
        {
            var panel = UIFactory.CreatePanel(canvas.transform, "Journal", new Vector2(900, 560), PanelFill, PanelBorder);
            UIFactory.SetAnchor(panel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            journalPanel = panel.gameObject;

            var titleText = UIFactory.CreateText(panel.transform, "Title", "NHẬT KÝ KIẾN THỨC", 26, Gold, TextAnchor.MiddleCenter, new Vector2(860, 34));
            UIFactory.SetAnchor(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -20));

            var listGO = new GameObject("List", typeof(RectTransform));
            journalListRoot = listGO.transform;
            journalListRoot.SetParent(panel.transform, false);
            UIFactory.SetAnchor((RectTransform)journalListRoot, new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -70));

            journalContentTitle = UIFactory.CreateText(panel.transform, "ContentTitle", "", 22, Cyan, TextAnchor.UpperLeft, new Vector2(500, 30));
            UIFactory.SetAnchor(journalContentTitle.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(320, -70));

            journalContentBody = UIFactory.CreateText(panel.transform, "ContentBody", "", 17, Ink, TextAnchor.UpperLeft, new Vector2(540, 400));
            UIFactory.SetAnchor(journalContentBody.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(320, -110));

            UIFactory.CreateButton(panel.transform, "CloseBtn", "Đóng (J)", new Vector2(140, 40), CloseJournal)
                .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -( 560 / 2f) + 34);

            journalPanel.SetActive(false);
        }

        void OpenJournal()
        {
            GameManager.Instance.State = GameState.Paused;
            var ids = GameManager.Instance.Save.journal;
            foreach (Transform t in journalListRoot) Destroy(t.gameObject);

            journalContentTitle.text = "";
            journalContentBody.text = ids.Count == 0 ? "Chưa có mục nào được mở khóa." : "";

            for (int i = 0; i < ids.Count; i++)
            {
                var entry = GameContent.GetJournal(ids[i]);
                if (entry == null) continue;
                var btn = UIFactory.CreateButton(journalListRoot, "Entry" + i, entry.Title, new Vector2(260, 36), () =>
                {
                    journalContentTitle.text = entry.Title;
                    journalContentBody.text = entry.Body;
                });
                UIFactory.SetAnchor(btn.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, -i * 42));
                if (i == 0)
                {
                    journalContentTitle.text = entry.Title;
                    journalContentBody.text = entry.Body;
                }
            }
            journalPanel.SetActive(true);
        }

        void CloseJournal()
        {
            journalPanel.SetActive(false);
            GameManager.Instance.State = GameState.Explore;
        }

        // ------------------------------------------------------------ INVENTORY
        void BuildInventory()
        {
            var panel = UIFactory.CreatePanel(canvas.transform, "Inventory", new Vector2(700, 420), PanelFill, PanelBorder);
            UIFactory.SetAnchor(panel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            inventoryPanel = panel.gameObject;

            var titleText = UIFactory.CreateText(panel.transform, "Title", "TÚI ĐỒ", 26, Gold, TextAnchor.MiddleCenter, new Vector2(660, 34));
            UIFactory.SetAnchor(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -20));

            var gridGO = new GameObject("Grid", typeof(RectTransform));
            inventoryGridRoot = gridGO.transform;
            inventoryGridRoot.SetParent(panel.transform, false);
            UIFactory.SetAnchor((RectTransform)inventoryGridRoot, new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -70));

            UIFactory.CreateButton(panel.transform, "CloseBtn", "Đóng (I)", new Vector2(140, 40), CloseInventory)
                .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(420 / 2f) + 34);

            inventoryPanel.SetActive(false);
        }

        void OpenInventory()
        {
            GameManager.Instance.State = GameState.Paused;
            foreach (Transform t in inventoryGridRoot) Destroy(t.gameObject);

            var ids = GameManager.Instance.Save.items;
            int col = 0, row = 0;
            const int cellW = 130, cellH = 90;
            foreach (var id in ids)
            {
                if (!GameContent.Items.TryGetValue(id, out var item)) continue;
                var cellGO = new GameObject("Item_" + id, typeof(RectTransform));
                var cellRt = (RectTransform)cellGO.transform;
                cellRt.SetParent(inventoryGridRoot, false);
                UIFactory.SetAnchor(cellRt, new Vector2(0, 1), new Vector2(0, 1), new Vector2(col * cellW, -row * cellH));

                var icon = UIFactory.CreateImage(cellRt, "Icon", PixelArt.Icon(item.Icon, item.Color), new Vector2(48, 48));
                UIFactory.SetAnchor(icon.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, 0));

                var label = UIFactory.CreateText(cellRt, "Label", item.Name, 13, Ink, TextAnchor.UpperCenter, new Vector2(120, 34));
                UIFactory.SetAnchor(label.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -52));

                col++;
                if (col >= 4) { col = 0; row++; }
            }
            if (ids.Count == 0)
            {
                var empty = UIFactory.CreateText(inventoryGridRoot, "Empty", "Chưa có vật phẩm nào.", 17, Ink, TextAnchor.MiddleLeft, new Vector2(500, 30));
            }
            inventoryPanel.SetActive(true);
        }

        void CloseInventory()
        {
            inventoryPanel.SetActive(false);
            GameManager.Instance.State = GameState.Explore;
        }

        // ------------------------------------------------------------ PAUSE
        void BuildPause()
        {
            var panel = UIFactory.CreatePanel(canvas.transform, "Pause", new Vector2(380, 280), PanelFill, PanelBorder);
            UIFactory.SetAnchor(panel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            pausePanel = panel.gameObject;

            var titleText = UIFactory.CreateText(panel.transform, "Title", "TẠM DỪNG", 24, Gold, TextAnchor.MiddleCenter, new Vector2(340, 34));
            UIFactory.SetAnchor(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -24));

            UIFactory.CreateButton(panel.transform, "ResumeBtn", "Tiếp Tục", new Vector2(280, 46), ClosePause)
                .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 24);
            UIFactory.CreateButton(panel.transform, "SaveMenuBtn", "Lưu & Về Menu", new Vector2(280, 46), () =>
            {
                GameManager.Instance.SaveGame();
                pausePanel.SetActive(false);
                GameManager.Instance.ReturnToMenu();
            }).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -36);

            pausePanel.SetActive(false);
        }

        void OpenPause()
        {
            GameManager.Instance.State = GameState.Paused;
            pausePanel.SetActive(true);
        }

        void ClosePause()
        {
            pausePanel.SetActive(false);
            GameManager.Instance.State = GameState.Explore;
        }

        // ------------------------------------------------------------ ENDING
        CanvasGroup endingGroup;
        RectTransform endingPanelRt;
        Text endingTitle;
        Coroutine endingRoutine;
        static readonly Color[] EndCrystalCols =
        {
            new Color(0.92f, 0.62f, 0.4f), new Color(0.62f, 0.8f, 0.42f), new Color(0.42f, 0.82f, 0.9f),
            new Color(0.78f, 0.52f, 0.9f), new Color(0.95f, 0.8f, 0.38f)
        };

        void BuildEnding()
        {
            // Lop phu toan man hinh — nen dien anh cho man ket
            var root = new GameObject("Ending", typeof(RectTransform), typeof(CanvasGroup));
            var rootRt = (RectTransform)root.transform;
            rootRt.SetParent(canvas.transform, false);
            UIFactory.Stretch(rootRt);
            endingPanel = root;
            endingGroup = root.GetComponent<CanvasGroup>();

            var bg = UIFactory.CreateImage(rootRt, "Bg", PixelArt.VGradient(new Color(0.06f, 0.05f, 0.12f), new Color(0.01f, 0.01f, 0.03f)), new Vector2(1280, 720));
            UIFactory.Stretch(bg.rectTransform);

            // Hao quang chien thang phia sau
            var glow = UIFactory.CreateImage(rootRt, "Glow", PixelArt.Glow(new Color(1f, 0.85f, 0.45f, 0.4f), 128), new Vector2(1000, 1000));
            glow.rectTransform.anchoredPosition = new Vector2(0, 140);
            glow.raycastTarget = false;
            var glowBob = glow.gameObject.AddComponent<Bobber>();
            glowBob.Amplitude = 14f; glowBob.Speed = 0.8f;

            var panel = UIFactory.CreatePanel(rootRt, "Panel", new Vector2(960, 620), PanelFill, PanelBorder);
            UIFactory.SetAnchor(panel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            endingPanelRt = panel.rectTransform;

            // Chom sang lap lanh hai ben tieu de
            for (int s = 0; s < 6; s++)
            {
                var star = UIFactory.CreateImage(panel.transform, "Star" + s, PixelArt.Glow(new Color(1f, 0.92f, 0.6f, 0.9f), 32), new Vector2(18, 18));
                UIFactory.SetAnchor(star.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2((s - 2.5f) * 150f, -18));
                star.raycastTarget = false;
                var sb = star.gameObject.AddComponent<Bobber>();
                sb.Amplitude = 5f; sb.Speed = 2f + s * 0.4f;
            }

            endingTitle = UIFactory.CreateText(panel.transform, "Title", "✦  KHÚC KHẢI HOÀN  ✦", 34, Gold, TextAnchor.MiddleCenter, new Vector2(880, 46));
            endingTitle.fontStyle = FontStyle.Bold;
            UIFactory.SetAnchor(endingTitle.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -36));

            // 5 vien ngoc ruc sang — thanh qua ca hanh trinh
            for (int c = 0; c < EndCrystalCols.Length; c++)
            {
                var glowC = UIFactory.CreateImage(panel.transform, "CrystalGlow" + c, PixelArt.Glow(EndCrystalCols[c] * 1.1f, 64), new Vector2(72, 72));
                UIFactory.SetAnchor(glowC.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2((c - 2) * 96f, -104));
                glowC.raycastTarget = false;

                var ci = UIFactory.CreateImage(panel.transform, "Crystal" + c, PixelArt.Crystal(EndCrystalCols[c], 32), new Vector2(44, 44));
                UIFactory.SetAnchor(ci.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2((c - 2) * 96f, -104));
                ci.raycastTarget = false;
                var cb = ci.gameObject.AddComponent<Bobber>();
                cb.Amplitude = 7f; cb.Speed = 1.5f + c * 0.35f;
            }

            var body = UIFactory.CreateText(panel.transform, "Body",
                "Năm Economic Crystal hội tụ, ánh sáng bừng lên xé tan bóng tối độc quyền — Solandor, Vua Độc Quyền, quỳ gối trước quy luật của thị trường.\n\n" +
                "Nhưng bạn hiểu rằng chiến thắng này không phải dấu chấm hết. Cạnh tranh sinh ra độc quyền, song độc quyền không bao giờ thủ tiêu được cạnh tranh — nó chỉ khiến cạnh tranh trở nên đa dạng và gay gắt hơn. Đó là quy luật thống nhất và đấu tranh bất tận giữa cạnh tranh và độc quyền.\n\n" +
                "Trên mảnh đất Econia hồi sinh, những khu chợ lại rộn ràng, người bán kẻ mua lại tự do trao đổi. Và ở quê nhà, bạn thấy bóng dáng một nền kinh tế thị trường định hướng xã hội chủ nghĩa — nơi Nhà nước điều tiết, kiểm soát độc quyền, giữ cho cạnh tranh luôn lành mạnh, hài hòa lợi ích của doanh nghiệp và toàn xã hội.\n\n" +
                "Hành trình khép lại, nhưng tri thức bạn mang theo sẽ còn thắp sáng những con đường phía trước.\n\nCảm ơn bạn đã đồng hành cùng Econia.",
                17, Ink, TextAnchor.UpperLeft, new Vector2(880, 380));
            UIFactory.SetAnchor(body.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -150));

            UIFactory.CreateButton(panel.transform, "MenuBtn", "Về Menu Chính", new Vector2(260, 46), () =>
            {
                if (endingRoutine != null) StopCoroutine(endingRoutine);
                endingPanel.SetActive(false);
                GameManager.Instance.ReturnToMenu();
            }).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(620 / 2f) + 40);

            endingPanel.SetActive(false);
        }

        public void ShowEnding()
        {
            hudPanel.SetActive(false);
            endingPanel.SetActive(true);
            if (endingRoutine != null) StopCoroutine(endingRoutine);
            endingRoutine = StartCoroutine(EndingRoutine());
        }

        IEnumerator EndingRoutine()
        {
            endingGroup.alpha = 0f;
            GameManager.Instance.PlaySfx("fanfare");

            // Hien dan + phong to nhe cho panel (hieu ung "bung sang")
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 1.1f;
                float e = Mathf.Clamp01(t);
                endingGroup.alpha = e;
                float scale = Mathf.Lerp(0.82f, 1f, 1f - (1f - e) * (1f - e)); // ease-out
                endingPanelRt.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }
            endingGroup.alpha = 1f;
            endingPanelRt.localScale = Vector3.one;

            // Nhip vang lap lai cho tieu de
            float pulse = 0f;
            while (endingPanel.activeSelf)
            {
                pulse += Time.deltaTime * 2.2f;
                float g = 0.82f + Mathf.Sin(pulse) * 0.12f;
                endingTitle.color = new Color(g, g * 0.85f, 0.38f);
                yield return null;
            }
        }

        // ------------------------------------------------------------ MAP TITLE
        void BuildMapTitle()
        {
            var go = new GameObject("MapTitle", typeof(RectTransform), typeof(CanvasGroup));
            var rt = (RectTransform)go.transform;
            rt.SetParent(canvas.transform, false);
            UIFactory.SetAnchor(rt, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -130));
            mapTitleGroup = go.GetComponent<CanvasGroup>();
            mapTitleGroup.alpha = 0f;
            mapTitleText = UIFactory.CreateText(rt, "Text", "", 34, Gold, TextAnchor.MiddleCenter, new Vector2(900, 50));
        }

        public void ShowMapTitle(string title)
        {
            mapTitleText.text = title;
            if (mapTitleRoutine != null) StopCoroutine(mapTitleRoutine);
            mapTitleRoutine = StartCoroutine(MapTitleRoutine());
        }

        IEnumerator MapTitleRoutine()
        {
            float t = 0f;
            while (t < 1f) { t += Time.deltaTime * 2f; mapTitleGroup.alpha = t; yield return null; }
            yield return new WaitForSeconds(1.6f);
            t = 1f;
            while (t > 0f) { t -= Time.deltaTime * 1.5f; mapTitleGroup.alpha = t; yield return null; }
        }

        // ------------------------------------------------------------ TOAST
        // Hop thong bao khung go goc phai-duoi (kieu "Arrived at ... Talk to ...")
        void BuildToast()
        {
            var panel = UIFactory.CreatePanel(canvas.transform, "Toast", new Vector2(460, 74), PanelFill, PanelBorder);
            UIFactory.SetAnchor(panel.rectTransform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-20, 20));
            toastGroup = panel.gameObject.AddComponent<CanvasGroup>();
            toastGroup.alpha = 0f;
            toastGroup.blocksRaycasts = false;
            panel.raycastTarget = false;
            toastText = UIFactory.CreateText(panel.transform, "Text", "", 16, Ink, TextAnchor.MiddleLeft, new Vector2(420, 60));
            UIFactory.SetAnchor(toastText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            toastText.raycastTarget = false;
        }

        public void Toast(string msg)
        {
            toastText.text = msg;
            if (toastRoutine != null) StopCoroutine(toastRoutine);
            toastRoutine = StartCoroutine(ToastRoutine());
        }

        IEnumerator ToastRoutine()
        {
            float t = 0f;
            while (t < 1f) { t += Time.deltaTime * 3f; toastGroup.alpha = t; yield return null; }
            yield return new WaitForSeconds(2.2f);
            t = 1f;
            while (t > 0f) { t -= Time.deltaTime * 2f; toastGroup.alpha = t; yield return null; }
        }

        // ------------------------------------------------------------ FADE
        void BuildFade()
        {
            var img = UIFactory.CreateImage(canvas.transform, "Fade", PixelArt.Solid(Color.black, 4), new Vector2(1280, 720));
            UIFactory.Stretch(img.rectTransform);
            img.color = new Color(0, 0, 0, 0f);
            img.raycastTarget = false;
            fadeImage = img;
        }

        public IEnumerator FadeOut()
        {
            fadeImage.raycastTarget = true;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 2.2f;
                fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(t));
                yield return null;
            }
        }

        public IEnumerator FadeIn()
        {
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime * 2.2f;
                fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(t));
                yield return null;
            }
            fadeImage.raycastTarget = false;
        }
    }
}
