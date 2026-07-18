using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KTG
{
    // Giao dien tran doi chat voi boss: statement + 3 lua chon phan bien + thanh mau.
    public class BossUI : MonoBehaviour
    {
        static readonly Color PanelFill = new Color(0.1f, 0.06f, 0.12f, 0.97f);
        static readonly Color PanelBorder = new Color(0.82f, 0.68f, 0.32f, 1f);
        static readonly Color Ink = new Color(0.93f, 0.9f, 0.82f);
        static readonly Color Cyan = new Color(0.45f, 0.85f, 0.9f);
        static readonly Color Gold = new Color(0.88f, 0.74f, 0.35f);

        GameObject panel;
        Image[] hpPips;
        Text statementText;
        Text feedbackText;
        Transform optionsRoot;

        List<BossRound> rounds;
        int roundIndex;
        int hp;
        System.Action onWin;

        public void Build()
        {
            var img = UIFactory.CreatePanel(transform, "Panel", new Vector2(960, 500), PanelFill, PanelBorder);
            UIFactory.SetAnchor(img.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            panel = img.gameObject;

            var nameText = UIFactory.CreateText(panel.transform, "BossName", GameContent.BossName, 26, Gold, TextAnchor.MiddleCenter, new Vector2(900, 36));
            UIFactory.SetAnchor(nameText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -20));

            var portrait = UIFactory.CreateImage(panel.transform, "Portrait",
                PixelArt.Portrait(new Color(0.85f, 0.7f, 0.2f), new Color(0.9f, 0.8f, 0.6f), new Color(0.3f, 0.1f, 0.4f)),
                new Vector2(150, 150));
            UIFactory.SetAnchor(portrait.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -66));

            hpPips = new Image[5];
            const int pipW = 34;
            float startX = -((hpPips.Length - 1) * pipW) / 2f;
            for (int i = 0; i < hpPips.Length; i++)
            {
                var pip = UIFactory.CreateImage(panel.transform, "Hp" + i, PixelArt.Crystal(new Color(0.9f, 0.3f, 0.3f), 28), new Vector2(28, 28));
                UIFactory.SetAnchor(pip.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(startX + i * pipW, -230));
                hpPips[i] = pip;
            }

            statementText = UIFactory.CreateText(panel.transform, "Statement", "", 20, Ink, TextAnchor.MiddleCenter, new Vector2(880, 60));
            UIFactory.SetAnchor(statementText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -260));

            var optGO = new GameObject("Options", typeof(RectTransform));
            optionsRoot = optGO.transform;
            optionsRoot.SetParent(panel.transform, false);
            UIFactory.SetAnchor((RectTransform)optionsRoot, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 60));

            feedbackText = UIFactory.CreateText(panel.transform, "Feedback", "", 16, Cyan, TextAnchor.MiddleCenter, new Vector2(880, 40));
            UIFactory.SetAnchor(feedbackText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 18));

            gameObject.SetActive(false);
        }

        public void Open(List<BossRound> bossRounds, System.Action onWinCallback)
        {
            rounds = bossRounds;
            roundIndex = 0;
            hp = rounds.Count;
            onWin = onWinCallback;
            feedbackText.text = "";
            gameObject.SetActive(true);
            UpdateHp();
            RenderRound();
        }

        public void Close() => gameObject.SetActive(false);

        void RenderRound()
        {
            ClearOptions();
            if (roundIndex >= rounds.Count) return;
            var r = rounds[roundIndex];
            statementText.text = r.Statement;
            for (int i = 0; i < r.Options.Count; i++)
            {
                int idx = i;
                var btn = UIFactory.CreateButton(optionsRoot, "Opt" + i, r.Options[i], new Vector2(880, 42), () => Pick(idx));
                UIFactory.SetAnchor(btn.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, i * 48));
            }
        }

        void Pick(int idx)
        {
            var r = rounds[roundIndex];
            bool correct = idx == r.CorrectIndex;
            feedbackText.text = idx < r.Feedback.Count ? r.Feedback[idx] : "";

            if (correct)
            {
                GameManager.Instance.PlaySfx("hit");
                GameManager.Instance.ShakeCamera(0.18f); // don phan bien trung dich
                hp--;
                UpdateHp();
                roundIndex++;
                if (roundIndex >= rounds.Count)
                {
                    GameManager.Instance.PlaySfx("win");
                    ClearOptions();
                    StartCoroutine(WinDelay());
                }
                else
                {
                    RenderRound();
                }
            }
            else
            {
                GameManager.Instance.PlaySfx("no");
            }
        }

        IEnumerator WinDelay()
        {
            yield return new WaitForSeconds(1.4f);
            onWin?.Invoke();
        }

        void UpdateHp()
        {
            for (int i = 0; i < hpPips.Length; i++)
                hpPips[i].sprite = PixelArt.Crystal(i < hp ? new Color(0.9f, 0.3f, 0.3f) : new Color(0.3f, 0.3f, 0.35f, 0.4f), 28);
        }

        void ClearOptions()
        {
            for (int i = optionsRoot.childCount - 1; i >= 0; i--) Destroy(optionsRoot.GetChild(i).gameObject);
        }
    }
}
