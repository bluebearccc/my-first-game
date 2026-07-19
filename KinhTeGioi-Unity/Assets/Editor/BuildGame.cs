using UnityEditor;
using UnityEngine;

namespace KTG.EditorTools
{
    // Build game ra file .exe: menu "KTG > Build Windows" hoac chay batchmode voi
    // -executeMethod KTG.EditorTools.BuildGame.BuildWindows
    public static class BuildGame
    {
        // Shader gán runtime qua code (Sprite-Lit) không được scene/material nào tham chiếu nên
        // Unity strip khỏi build → Shader.Find trả null → màn đen. Ép vào Always Included Shaders.
        static readonly string[] RequiredShaders =
        {
            "Universal Render Pipeline/2D/Sprite-Lit-Default"
        };

        [MenuItem("KTG/Build Windows")]
        public static void BuildWindows()
        {
            foreach (var s in RequiredShaders) EnsureAlwaysIncludedShader(s);

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

        // Thêm shader vào Project Settings > Graphics > Always Included Shaders (nếu chưa có).
        // Bền vững: ghi vào GraphicsSettings.asset nên các lần build sau (kể cả menu) đều có shader.
        static void EnsureAlwaysIncludedShader(string shaderName)
        {
            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogWarning($"[KinhTeGioi] Không tìm thấy shader trong Editor: {shaderName}");
                return;
            }

            var graphicsSettings = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0];
            var so = new SerializedObject(graphicsSettings);
            var arr = so.FindProperty("m_AlwaysIncludedShaders");
            for (int i = 0; i < arr.arraySize; i++)
                if (arr.GetArrayElementAtIndex(i).objectReferenceValue == shader) return; // đã có

            int idx = arr.arraySize;
            arr.InsertArrayElementAtIndex(idx);
            arr.GetArrayElementAtIndex(idx).objectReferenceValue = shader;
            so.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            Debug.Log($"[KinhTeGioi] Đã thêm shader vào Always Included: {shaderName}");
        }
    }
}
