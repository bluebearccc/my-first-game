using System.Collections.Generic;
using UnityEngine;

namespace KTG
{
    // Tong hop hieu ung am thanh bang song sine/square — khong can file audio ngoai.
    public class AudioSynth : MonoBehaviour
    {
        const int SR = 44100;
        AudioSource src;
        readonly Dictionary<string, AudioClip> cache = new Dictionary<string, AudioClip>();

        void Awake()
        {
            src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
        }

        public void Play(string name)
        {
            if (!cache.TryGetValue(name, out var clip))
            {
                clip = Build(name);
                cache[name] = clip;
            }
            src.PlayOneShot(clip, 0.6f);
        }

        AudioClip Build(string name)
        {
            switch (name)
            {
                case "blip": return Tone(new[] { 880f }, 0.06f, 0.5f, false);
                case "open": return Tone(new[] { 520f, 780f }, 0.12f, 0.4f, false);
                case "ok": return Chord(new[] { 523.25f, 659.25f, 783.99f }, 0.35f);
                case "no": return Tone(new[] { 160f }, 0.25f, 0.6f, true);
                case "crystal": return Sweep(600f, 1400f, 0.5f);
                case "hit": return Tone(new[] { 220f, 110f }, 0.15f, 0.7f, true);
                case "win": return Chord(new[] { 523.25f, 659.25f, 783.99f, 1046.5f }, 0.8f);
                default: return Tone(new[] { 440f }, 0.1f, 0.4f, false);
            }
        }

        AudioClip Tone(float[] freqs, float dur, float vol, bool square)
        {
            int n = Mathf.CeilToInt(SR * dur);
            var data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SR;
                float env = Mathf.Exp(-4f * t / dur);
                float s = 0f;
                for (int k = 0; k < freqs.Length; k++)
                {
                    float w = Mathf.Sin(2f * Mathf.PI * freqs[k] * t);
                    s += square ? Mathf.Sign(w) : w;
                }
                data[i] = s / freqs.Length * vol * env;
            }
            var clip = AudioClip.Create("tone", n, 1, SR, false);
            clip.SetData(data, 0);
            return clip;
        }

        AudioClip Chord(float[] freqs, float dur)
        {
            int n = Mathf.CeilToInt(SR * dur);
            var data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SR;
                float env = Mathf.Exp(-2.5f * t / dur);
                float s = 0f;
                for (int k = 0; k < freqs.Length; k++)
                    s += Mathf.Sin(2f * Mathf.PI * freqs[k] * t);
                data[i] = s / freqs.Length * 0.5f * env;
            }
            var clip = AudioClip.Create("chord", n, 1, SR, false);
            clip.SetData(data, 0);
            return clip;
        }

        AudioClip Sweep(float f0, float f1, float dur)
        {
            int n = Mathf.CeilToInt(SR * dur);
            var data = new float[n];
            float phase = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SR;
                float frac = t / dur;
                float f = Mathf.Lerp(f0, f1, frac);
                phase += 2f * Mathf.PI * f / SR;
                float env = Mathf.Sin(Mathf.PI * Mathf.Clamp01(frac));
                data[i] = Mathf.Sin(phase) * 0.5f * env;
            }
            var clip = AudioClip.Create("sweep", n, 1, SR, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
