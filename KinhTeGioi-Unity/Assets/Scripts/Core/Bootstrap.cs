using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
            // Camera tao bang code nen them tuong minh camera data cua URP (Phase A ke hoach HD-2D)
            var camData = camGO.AddComponent<UniversalAdditionalCameraData>();
            camData.renderPostProcessing = true; // Phase C: bat post-processing

            // Phase C: Volume + profile dung bang code (khong asset ngoai — dung triet ly procedural).
            // UI o ScreenSpaceOverlay render sau post nen luon sac net.
            var volGO = new GameObject("GlobalVolume");
            var volume = volGO.AddComponent<Volume>();
            volume.isGlobal = true;
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();

            var bloom = profile.Add<Bloom>();
            bloom.intensity.Override(0.85f);
            bloom.threshold.Override(0.85f); // glow/duoc/crystal du sang de "ruc"
            bloom.scatter.Override(0.6f);

            var vig = profile.Add<Vignette>();
            vig.intensity.Override(0.28f); // thay cho Image PixelArt.Vignette cu tren HUD
            vig.smoothness.Override(0.45f);

            var tone = profile.Add<Tonemapping>();
            tone.mode.Override(TonemappingMode.Neutral);

            var colorAdj = profile.Add<ColorAdjustments>();
            colorAdj.saturation.Override(6f);
            colorAdj.contrast.Override(4f);

            // DoF Gaussian de rat nhe: camera ortho + sprite cung do sau (z=0, cach cam 10)
            // → the gioi nam TRUOC gaussianStart nen van sac net; hieu ung diorama that
            // can camera phoi canh (Phase D3, chua lam).
            var dof = profile.Add<DepthOfField>();
            dof.mode.Override(DepthOfFieldMode.Gaussian);
            dof.gaussianStart.Override(11f);
            dof.gaussianEnd.Override(30f);

            volume.profile = profile;

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
