using UnityEngine;

namespace KTG
{
    // Dung toan bo the gioi game bang code khi scene duoc nap — khong can keo-tha gi trong Editor.
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            if (GameManager.Instance != null) return;

            var camGO = new GameObject("MainCamera");
            camGO.transform.position = new Vector3(0f, 0f, -10f);
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.5f;
            cam.backgroundColor = new Color(0.04f, 0.05f, 0.09f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.tag = "MainCamera";
            camGO.AddComponent<AudioListener>();
            var camFollow = camGO.AddComponent<CameraFollow>();

            var worldRoot = new GameObject("World").transform;

            var playerGO = new GameObject("Player");
            var playerController = playerGO.AddComponent<PlayerController>();
            camFollow.target = playerGO.transform;

            var root = new GameObject("GameRoot");
            var gm = root.AddComponent<GameManager>();
            var ui = root.AddComponent<UIManager>();
            var audioSynth = root.AddComponent<AudioSynth>();
            root.AddComponent<MusicSynth>();

            gm.Init(ui, audioSynth, playerController, worldRoot, camFollow);
        }
    }
}
