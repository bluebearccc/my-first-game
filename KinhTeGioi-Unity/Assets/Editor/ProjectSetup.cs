using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace KTG.EditorTools
{
    // Tu dong tao scene Main.unity lan dau mo project, de nguoi dung chi can bam Play.
    public static class ProjectSetup
    {
        [InitializeOnLoadMethod]
        static void Ensure()
        {
            const string scenePath = "Assets/Scenes/Main.unity";
            if (File.Exists(scenePath)) return;
            EditorApplication.delayCall += () =>
            {
                if (File.Exists(scenePath)) return;
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
                Directory.CreateDirectory("Assets/Scenes");
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, scenePath);
                EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };
                Debug.Log("[KinhTeGioi] Da tao scene Main.unity — bam Play de choi game.");
            };
        }
    }
}
