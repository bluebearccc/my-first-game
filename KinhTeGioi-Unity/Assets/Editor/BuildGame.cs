using UnityEditor;
using UnityEngine;

namespace KTG.EditorTools
{
    // Build game ra file .exe: menu "KTG > Build Windows" hoac chay batchmode voi
    // -executeMethod KTG.EditorTools.BuildGame.BuildWindows
    public static class BuildGame
    {
        [MenuItem("KTG/Build Windows")]
        public static void BuildWindows()
        {
            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Main.unity" },
                locationPathName = "Build/Windows/KinhTeGioi.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;
            Debug.Log($"[KinhTeGioi] Build {summary.result} — {summary.totalSize / (1024 * 1024)} MB tai {summary.outputPath}");

            if (Application.isBatchMode)
                EditorApplication.Exit(summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? 0 : 1);
        }
    }
}
