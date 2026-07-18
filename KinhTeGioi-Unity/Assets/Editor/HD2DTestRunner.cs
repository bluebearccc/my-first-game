using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KTG.EditorTools
{
    // Nghiem thu tu dong cho ke hoach HD-2D (plan/02 muc C): vao Play mode,
    // tu New Game, di qua du 5 map, chup anh + do FPS tung map roi thoat.
    // Chay: Unity.exe -projectPath ... -executeMethod KTG.EditorTools.HD2DTestRunner.Run -hd2dLabel <ten> -hd2dOut <thumuc>
    // (KHONG dung -batchmode vi can render that de chup anh.)
    public static class HD2DTestRunner
    {
        public static void Run()
        {
            EditorApplication.isPlaying = true;
        }

        [InitializeOnLoadMethod]
        static void Hook()
        {
            if (GetArg("-hd2dLabel") == null) return; // chi chay khi duoc goi tu dong
            EditorApplication.playModeStateChanged += st =>
            {
                if (st == PlayModeStateChange.EnteredPlayMode)
                {
                    var go = new UnityEngine.GameObject("HD2DCaptureDriver");
                    UnityEngine.Object.DontDestroyOnLoad(go);
                    go.AddComponent<HD2DCaptureDriver>();
                }
            };
        }

        internal static string GetArg(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
                if (args[i] == name) return args[i + 1];
            return null;
        }
    }

    // Driver chay trong Play mode: dieu khien GameManager di qua 5 map.
    public class HD2DCaptureDriver : MonoBehaviour
    {
        const string SaveKey = "ktg_save";
        string label, outDir;
        string savedData; bool hadSave;

        void Start()
        {
            label = HD2DTestRunner.GetArg("-hd2dLabel") ?? "run";
            outDir = HD2DTestRunner.GetArg("-hd2dOut") ?? Path.Combine(Application.dataPath, "../../shots");
            outDir = Path.GetFullPath(Path.Combine(outDir, label));
            Directory.CreateDirectory(outDir);

            // Bao ve save cua nguoi choi: sao luu truoc, khoi phuc khi xong
            hadSave = PlayerPrefs.HasKey(SaveKey);
            savedData = hadSave ? PlayerPrefs.GetString(SaveKey) : null;

            StartCoroutine(Drive());
        }

        IEnumerator Drive()
        {
            float deadline = Time.realtimeSinceStartup + 240f;
            var log = new System.Text.StringBuilder();
            log.AppendLine("label: " + label);

            while (GameManager.Instance == null && Time.realtimeSinceStartup < deadline)
                yield return null;
            if (GameManager.Instance == null) { Finish(2, "GameManager khong xuat hien"); yield break; }

            yield return new WaitForSeconds(1f); // cho menu dung xong
            GameManager.Instance.NewGame();

            for (int i = 0; i < GameContent.Maps.Length; i++)
            {
                if (i > 0) GameManager.Instance.GoToMap(i);
                while (!(GameManager.Instance.State == GameState.Explore && GameManager.Instance.CurrentMapIndex == i))
                {
                    if (Time.realtimeSinceStartup > deadline) { Finish(2, "Timeout o map " + i); yield break; }
                    yield return null;
                }
                yield return new WaitForSeconds(0.8f); // cho fade-in + FX on dinh

                // Do FPS trung binh trong 1.5s
                int frames = 0; float t0 = Time.realtimeSinceStartup;
                while (Time.realtimeSinceStartup - t0 < 1.5f) { frames++; yield return null; }
                float fps = frames / (Time.realtimeSinceStartup - t0);

                string shot = Path.Combine(outDir, "map" + i + ".png");
                ScreenCapture.CaptureScreenshot(shot);
                yield return null; yield return null; yield return null;

                log.AppendLine(string.Format("map{0}: fps={1:0.0} shot={2}", i, fps, File.Exists(shot) ? "OK" : "MISSING"));
            }

            File.WriteAllText(Path.Combine(outDir, "summary.txt"), log.ToString());
            Finish(0, "xong");
        }

        void Finish(int code, string reason)
        {
            if (hadSave) PlayerPrefs.SetString(SaveKey, savedData); else PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
            Debug.Log("[HD2D] Capture ket thuc (" + code + "): " + reason);
            EditorApplication.Exit(code);
        }
    }
}
