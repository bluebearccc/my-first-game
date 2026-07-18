using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KTG.EditorTools
{
    // Phase A cua ke hoach HD-2D (plan/01): tao URP asset + 2D Renderer va gan vao
    // Graphics/Quality settings. Chay qua menu Tools/HD2D hoac batchmode -executeMethod.
    public static class HD2DUrpSetup
    {
        const string SettingsFolder = "Assets/Settings";
        const string RendererPath = SettingsFolder + "/Renderer2D.asset";
        const string PipelinePath = SettingsFolder + "/URP-HD2D.asset";

        [MenuItem("Tools/HD2D/Setup URP 2D (Phase A)")]
        public static void Setup()
        {
            if (!AssetDatabase.IsValidFolder(SettingsFolder))
                AssetDatabase.CreateFolder("Assets", "Settings");

            // Tao 2D Renderer data (ResourceReloader trong editor tu dien cac shader/material mac dinh)
            var rendererData = AssetDatabase.LoadAssetAtPath<Renderer2DData>(RendererPath);
            if (rendererData == null)
            {
                rendererData = ScriptableObject.CreateInstance<Renderer2DData>();
                AssetDatabase.CreateAsset(rendererData, RendererPath);
            }

            // Tao pipeline asset dung 2D Renderer
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(rendererData);
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
            }

            // Gan vao Graphics va MOI muc Quality (buoc 4 cua Phase A)
            GraphicsSettings.defaultRenderPipeline = pipeline;
            int currentLevel = QualitySettings.GetQualityLevel();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = pipeline;
            }
            QualitySettings.SetQualityLevel(currentLevel, false);

            AssetDatabase.SaveAssets();
            EditorApplication.ExecuteMenuItem("File/Save Project");
            Debug.Log("[HD2D] Phase A: URP 2D da duoc tao va gan vao Graphics/Quality.");
        }
    }
}
