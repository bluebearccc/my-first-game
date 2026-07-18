using UnityEngine;

namespace KTG
{
    // Nhac nen mo mang tong hop bang code — vong hoa am C/Am/F/G voi pad sine + arpeggio nhe,
    // loop lien mach, khong can file audio ngoai.
    public class MusicSynth : MonoBehaviour
    {
        public static MusicSynth Instance { get; private set; }

        const int SR = 44100;
        const float BarDur = 4f;                 // moi hop am keo dai 4 giay
        AudioSource src;

        // Tan so goc (C major, quang tam 3-4)
        static readonly float[][] Chords =
        {
            new[] { 130.81f, 164.81f, 196.00f, 261.63f }, // C
            new[] { 110.00f, 130.81f, 164.81f, 220.00f }, // Am
            new[] { 87.31f, 130.81f, 174.61f, 220.00f },  // F
            new[] { 98.00f, 123.47f, 146.83f, 196.00f }   // G
        };

        void Awake()
        {
            Instance = this;
            src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = true;
            src.volume = 0.32f;
            src.clip = BuildLoop();
            src.Play();
        }

        // Doi sac thai theo vung dat: map cang sau pitch cang tram mot chut.
        public void SetMapMood(int mapIndex)
        {
            if (src == null) return;
            src.pitch = 1.04f - mapIndex * 0.02f;
        }

        AudioClip BuildLoop()
        {
            int barSamples = Mathf.CeilToInt(SR * BarDur);
            int n = barSamples * Chords.Length;
            var data = new float[n];

            for (int bar = 0; bar < Chords.Length; bar++)
            {
                var chord = Chords[bar];
                int offset = bar * barSamples;
                for (int i = 0; i < barSamples; i++)
                {
                    float t = i / (float)SR;
                    // Pad: cac not hop am ngan nga, vao/ra mem de noi bar lien mach
                    float fadeIn = Mathf.Clamp01(t / 0.6f);
                    float fadeOut = Mathf.Clamp01((BarDur - t) / 0.6f);
                    float env = Mathf.Min(fadeIn, fadeOut);
                    float s = 0f;
                    for (int k = 0; k < chord.Length; k++)
                        s += Mathf.Sin(2f * Mathf.PI * chord[k] * t) * 0.5f
                           + Mathf.Sin(2f * Mathf.PI * chord[k] * 2f * t) * 0.12f;
                    s = s / chord.Length * env * 0.55f;

                    // Arpeggio: not don lap lai moi 0.5s, lay lan luot cac not trong hop am (quang cao hon)
                    int step = Mathf.FloorToInt(t * 2f);
                    float st = t - step * 0.5f;
                    float noteFreq = chord[step % chord.Length] * 4f;
                    float noteEnv = Mathf.Exp(-5f * st) * 0.16f;
                    s += Mathf.Sin(2f * Mathf.PI * noteFreq * st) * noteEnv;

                    data[offset + i] = s;
                }
            }

            var clip = AudioClip.Create("bgm_loop", n, 1, SR, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
