using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KTG
{
    // Giao dien chung cho 3 loai puzzle: Classify, Order, Quiz.
    public class PuzzleUI : MonoBehaviour
    {
        static readonly Color PanelFill = new Color(0.08f, 0.09f, 0.16f, 0.97f);
        static readonly Color PanelBorder = new Color(0.82f, 0.68f, 0.32f, 1f);
        static readonly Color Ink = new Color(0.93f, 0.9f, 0.82f);
        static readonly Color Cyan = new Color(0.45f, 0.85f, 0.9f);

        GameObject panel;
        Text titleText;
        Text instructionsText;
        Text feedbackText;
        Transform contentRoot;

        PuzzleDef current;
        System.Action onSuccess;

        int classifyIndex;
        List<PuzzleItem> classifyShuffled;

        List<string> orderRemaining;
        List<string> orderChosen;

        int quizIndex;

        public void Build()
        {
            var img = UIFactory.CreatePanel(transform, "Panel", new Vector2(960, 560), PanelFill, PanelBorder);
            UIFactory.SetAnchor(img.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            panel = img.gameObject;

            titleText = UIFactory.CreateText(panel.transform, "Title", "", 24, new Color(0.88f, 0.74f, 0.35f), TextAnchor.MiddleCenter, new Vector2(880, 32));
            UIFactory.SetAnchor(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -20));

            instructionsText = UIFactory.CreateText(panel.transform, "Instructions", "", 16, Ink, TextAnchor.MiddleCenter, new Vector2(860, 26));
            UIFactory.SetAnchor(instructionsText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -56));

            feedbackText = UIFactory.CreateText(panel.transform, "Feedback", "", 16, Cyan, TextAnchor.MiddleCenter, new Vector2(860, 50));
            UIFactory.SetAnchor(feedbackText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 18));

            var contentGO = new GameObject("Content", typeof(RectTransform));
            contentRoot = contentGO.transform;
            contentRoot.SetParent(panel.transform, false);
            UIFactory.SetAnchor((RectTransform)contentRoot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -6));

            gameObject.SetActive(false);
        }

        public void Open(PuzzleDef def, System.Action onSuccessCallback)
        {
            current = def;
            onSuccess = onSuccessCallback;
            titleText.text = def.Title;
            instructionsText.text = def.Instructions;
            feedbackText.text = "";
            gameObject.SetActive(true);

            switch (def.Type)
            {
                case PuzzleType.Classify: OpenClassify(); break;
                case PuzzleType.Order: OpenOrder(); break;
                case PuzzleType.Quiz: OpenQuiz(); break;
            }
        }

        public void Close() => gameObject.SetActive(false);

        void ClearContent()
        {
            for (int i = contentRoot.childCount - 1; i >= 0; i--) Destroy(contentRoot.GetChild(i).gameObject);
        }

        // ---------------- Classify
        void OpenClassify()
        {
            classifyShuffled = new List<PuzzleItem>(current.Items);
            Shuffle(classifyShuffled);
            classifyIndex = 0;
            RenderClassify();
        }

        void RenderClassify()
        {
            ClearContent();
            if (classifyIndex >= classifyShuffled.Count) { Succeed(); return; }

            var item = classifyShuffled[classifyIndex];
            var label = UIFactory.CreateText(contentRoot, "Item", item.Label, 19, Ink, TextAnchor.MiddleCenter, new Vector2(840, 96));
            UIFactory.SetAnchor(label.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, 30));

            int count = current.Categories.Count;
            const int w = 250;
            const int gap = 24;
            int totalW = count * (w + gap) - gap;
            float startX = -totalW / 2f + w / 2f;
            for (int i = 0; i < count; i++)
            {
                var cat = current.Categories[i];
                var btn = UIFactory.CreateButton(contentRoot, "Cat" + i, cat, new Vector2(w, 58), () => PickClassify(cat), 18);
                UIFactory.SetAnchor(btn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(startX + i * (w + gap), -110));
            }
        }

        void PickClassify(string category)
        {
            var item = classifyShuffled[classifyIndex];
            if (item.Category == category)
            {
                GameManager.Instance.PlaySfx("ok");
                feedbackText.text = "Chính xác!";
                classifyIndex++;
                RenderClassify();
            }
            else
            {
                GameManager.Instance.PlaySfx("no");
                feedbackText.text = "Chưa đúng, thử lại nhé.";
            }
        }

        // ---------------- Order
        void OpenOrder()
        {
            orderRemaining = new List<string>(current.Steps);
            Shuffle(orderRemaining);
            orderChosen = new List<string>();
            RenderOrder();
        }

        void RenderOrder()
        {
            ClearContent();

            var chosenText = UIFactory.CreateText(contentRoot, "Chosen", ChosenLines(), 16, Cyan, TextAnchor.UpperLeft, new Vector2(840, 150));
            UIFactory.SetAnchor(chosenText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, 0));

            for (int i = 0; i < orderRemaining.Count; i++)
            {
                var step = orderRemaining[i];
                var btn = UIFactory.CreateButton(contentRoot, "Step" + i, step, new Vector2(860, 54), () => PickOrder(step), 16);
                UIFactory.SetAnchor(btn.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, i * 60));
            }
        }

        string ChosenLines()
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < orderChosen.Count; i++) sb.Append((i + 1) + ". " + orderChosen[i] + "\n");
            return sb.ToString();
        }

        void PickOrder(string step)
        {
            string expected = current.Steps[orderChosen.Count];
            if (step == expected)
            {
                GameManager.Instance.PlaySfx("ok");
                orderChosen.Add(step);
                orderRemaining.Remove(step);
                if (orderChosen.Count == current.Steps.Count) { Succeed(); return; }
                RenderOrder();
            }
            else
            {
                GameManager.Instance.PlaySfx("no");
                feedbackText.text = "Chưa đúng thứ tự, thử lại nhé.";
            }
        }

        // ---------------- Quiz
        void OpenQuiz()
        {
            quizIndex = 0;
            RenderQuiz();
        }

        void RenderQuiz()
        {
            ClearContent();
            if (quizIndex >= current.Quiz.Count) { Succeed(); return; }
            var q = current.Quiz[quizIndex];

            var stmt = UIFactory.CreateText(contentRoot, "Statement", q.Statement, 19, Ink, TextAnchor.MiddleCenter, new Vector2(840, 66));
            stmt.fontStyle = FontStyle.Bold;
            UIFactory.SetAnchor(stmt.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, 0));

            for (int i = 0; i < q.Options.Count; i++)
            {
                int idx = i;
                var btn = UIFactory.CreateButton(contentRoot, "Opt" + i, q.Options[i], new Vector2(860, 60), () => PickQuiz(idx), 16);
                UIFactory.SetAnchor(btn.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, i * 68));
            }
        }

        void PickQuiz(int idx)
        {
            var q = current.Quiz[quizIndex];
            bool correct = idx == q.CorrectIndex;
            feedbackText.text = idx < q.Feedback.Count ? q.Feedback[idx] : "";
            GameManager.Instance.PlaySfx(correct ? "ok" : "no");
            if (correct)
            {
                quizIndex++;
                RenderQuiz();
            }
        }

        void Succeed()
        {
            ClearContent();
            feedbackText.text = "";
            GameManager.Instance.PlaySfx("crystal");
            onSuccess?.Invoke();
        }

        static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}
