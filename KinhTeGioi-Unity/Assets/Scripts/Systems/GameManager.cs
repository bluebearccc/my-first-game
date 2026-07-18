using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KTG
{
    [System.Serializable]
    public class SaveData
    {
        public bool started;
        public int currentMap;
        public int[] stage = new int[5];
        public bool[] crystal = new bool[5];
        public List<string> journal = new List<string>();
        public List<string> items = new List<string>();
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        const string SaveKey = "ktg_save";

        public GameState State = GameState.Menu;
        public int CurrentMapIndex;
        public SaveData Save = new SaveData();

        UIManager ui;
        AudioSynth audio;
        PlayerController player;
        Transform worldRoot;
        CameraFollow camFollow;

        void Awake()
        {
            Instance = this;
        }

        public void Init(UIManager uiManager, AudioSynth audioSynth, PlayerController playerController, Transform world, CameraFollow follow)
        {
            ui = uiManager;
            audio = audioSynth;
            player = playerController;
            worldRoot = world;
            camFollow = follow;

            ui.Build();
            State = GameState.Menu;
            ui.ShowMainMenu();
        }

        public bool HasSave() => PlayerPrefs.HasKey(SaveKey);

        public void PlaySfx(string name) => audio.Play(name);

        public void ShakeCamera(float amp = 0.12f) { if (camFollow != null) camFollow.Shake(amp); }

        // ---------------------------------------------------------- FLOW
        public void NewGame()
        {
            Save = new SaveData { started = true };
            ui.HideMainMenu();
            ui.ShowHUD(true);
            GoToMap(0);
        }

        public void ContinueGame()
        {
            LoadGameData();
            ui.HideMainMenu();
            ui.ShowHUD(true);
            GoToMap(Save.currentMap);
        }

        public void ReturnToMenu()
        {
            State = GameState.Menu;
            ui.ShowHUD(false);
            ui.ShowMainMenu();
        }

        public void GoToMap(int index) => StartCoroutine(GoToMapRoutine(index));

        IEnumerator GoToMapRoutine(int index)
        {
            State = GameState.Dialogue; // khoa input trong luc chuyen canh
            yield return StartCoroutine(ui.FadeOut());

            CurrentMapIndex = index;
            Save.currentMap = index;
            MapBuilder.Build(index, worldRoot);

            var spawnPos = MapBuilder.CellToWorld(MapBuilder.SpawnCell) - new Vector3(0f, 0.5f, 0f);
            player.Warp(spawnPos);
            if (camFollow != null) camFollow.Apply();

            var map = GameContent.Maps[index];
            int stage = Mathf.Min(Save.stage[index], map.Objectives.Length - 1);
            ui.UpdateObjective(map.Objectives[stage]);
            ui.UpdateCrystals(Save.crystal);
            ui.ShowMapTitle(map.Title);
            ui.Toast("Đã đến " + map.Title + ".\n" + map.Objectives[stage]);
            if (MusicSynth.Instance != null) MusicSynth.Instance.SetMapMood(index);

            SaveGame();
            yield return StartCoroutine(ui.FadeIn());
            State = GameState.Explore;
        }

        // ---------------------------------------------------------- INTERACT
        public void Interact(InteractableKind kind, char code)
        {
            if (State != GameState.Explore) return;
            switch (kind)
            {
                case InteractableKind.Npc:
                    var d = GameContent.GetDialogue(CurrentMapIndex, code, Save.stage[CurrentMapIndex]);
                    RunDialogue(d);
                    break;
                case InteractableKind.Pedestal:
                    HandlePedestal();
                    break;
                case InteractableKind.PortalForward:
                    HandlePortal(true);
                    break;
                case InteractableKind.PortalBack:
                    HandlePortal(false);
                    break;
                case InteractableKind.Boss:
                    HandleBossTile();
                    break;
                case InteractableKind.Lore:
                    audio.Play("open");
                    RunDialogue(GameContent.GetLore(CurrentMapIndex));
                    break;
            }
        }

        void HandlePedestal()
        {
            var map = GameContent.Maps[CurrentMapIndex];
            int puzzleStage = map.Flow.Length - 1;
            if (Save.crystal[CurrentMapIndex]) { ui.Toast("Bạn đã lấy Crystal ở vùng này rồi."); return; }
            if (Save.stage[CurrentMapIndex] < puzzleStage) { ui.Toast("Hãy nói chuyện với mọi người ở đây trước đã."); return; }

            var pd = GameContent.GetPuzzle(CurrentMapIndex);
            if (pd == null) return;
            audio.Play("open");
            State = GameState.Puzzle;
            ui.Puzzle.Open(pd, OnPuzzleSolved);
        }

        void OnPuzzleSolved()
        {
            ui.Puzzle.Close();
            HandleDialogueAction(GameContent.Maps[CurrentMapIndex].RewardAction);
        }

        void HandlePortal(bool forward)
        {
            if (forward)
            {
                if (!Save.crystal[CurrentMapIndex]) { ui.Toast("Cần lấy Crystal ở đây trước khi đi tiếp."); return; }
                if (CurrentMapIndex >= GameContent.Maps.Length - 1) return;
                GoToMap(CurrentMapIndex + 1);
            }
            else
            {
                if (CurrentMapIndex == 0) return;
                GoToMap(CurrentMapIndex - 1);
            }
        }

        void HandleBossTile()
        {
            var map = GameContent.Maps[CurrentMapIndex];
            if (Save.crystal[CurrentMapIndex]) { ui.Toast("Bạn đã đánh bại hắn rồi."); return; }
            if (Save.stage[CurrentMapIndex] < map.Flow.Length - 1) { ui.Toast("Hãy nói chuyện với lính gác trước."); return; }
            StartBossFight();
        }

        void StartBossFight()
        {
            audio.Play("open");
            State = GameState.Boss;
            ui.Boss.Open(GameContent.BossRounds, OnBossDefeated);
        }

        void OnBossDefeated()
        {
            ui.Boss.Close();
            HandleDialogueAction(GameContent.Maps[CurrentMapIndex].RewardAction);
        }

        // ---------------------------------------------------------- DIALOGUE
        void RunDialogue(DialogueDef d)
        {
            State = GameState.Dialogue;
            ui.ShowDialogue(d, HandleDialogueAction);
        }

        void HandleDialogueAction(string action)
        {
            bool win = false;
            if (!string.IsNullOrEmpty(action))
            {
                foreach (var tokRaw in action.Split(';'))
                {
                    var tok = tokRaw.Trim();
                    if (tok.Length == 0) continue;

                    if (tok == "stage+") Save.stage[CurrentMapIndex]++;
                    else if (tok == "crystal")
                    {
                        Save.crystal[CurrentMapIndex] = true;
                        ui.UpdateCrystals(Save.crystal);
                        audio.Play("crystal");
                        ShakeCamera(0.1f);
                        SparkFly.Burst(player.transform.position + Vector3.up * 0.7f, worldRoot);
                    }
                    else if (tok.StartsWith("journal:"))
                    {
                        var id = tok.Substring(8);
                        if (!Save.journal.Contains(id)) Save.journal.Add(id);
                    }
                    else if (tok.StartsWith("item:"))
                    {
                        var id = tok.Substring(5);
                        if (!Save.items.Contains(id)) Save.items.Add(id);
                    }
                    else if (tok.StartsWith("toast:")) ui.Toast(tok.Substring(6));
                    else if (tok == "win") win = true;
                }
            }

            var map = GameContent.Maps[CurrentMapIndex];
            int stage = Mathf.Min(Save.stage[CurrentMapIndex], map.Objectives.Length - 1);
            ui.UpdateObjective(map.Objectives[stage]);
            SaveGame();

            if (win) { ui.ShowEnding(); State = GameState.Menu; }
            else State = GameState.Explore;
        }

        // ---------------------------------------------------------- SAVE
        public void SaveGame()
        {
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(Save));
            PlayerPrefs.Save();
        }

        void LoadGameData()
        {
            var json = PlayerPrefs.GetString(SaveKey, "");
            Save = string.IsNullOrEmpty(json) ? new SaveData { started = true } : JsonUtility.FromJson<SaveData>(json);
        }
    }
}
