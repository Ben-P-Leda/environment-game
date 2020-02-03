using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static void PlaySound(string name, float pitch = 1.0f, float volume = 1.0f) { _instance.PlaySoundEffect(name, pitch, volume); }
        public static void PlayRandomSound(string nameStem, float pitch = 1.0f, float volume = 1.0f) { _instance.PlayRandomSoundEffect(nameStem, pitch, volume); }
        public static bool MusicPlaying { set { _instance.SetMusicActive(value); } }

        [SerializeField] private string[] _effectsFolders;
        [SerializeField] private int _voiceCount = 10;
        [SerializeField] private AudioClip _musicClip;

        private List<AudioSource> _voices;
        private Dictionary<string, AudioClip> _effects;
        private AudioSource _musicVoice;

        private void Awake()
        {
            _instance = this;
            _musicVoice = gameObject.AddComponent<AudioSource>();
            _musicVoice.clip = _musicClip;

            LoadEffects();
            InitializeVoices();
        }

        private void LoadEffects()
        {
            _effects = new Dictionary<string, AudioClip>();

            foreach (string folder in _effectsFolders)
            {
                AudioClip[] clips = Resources.LoadAll<AudioClip>($"Sound Effects/{folder}");
                foreach (AudioClip clip in clips)
                {
                    _effects.Add(clip.name, clip);
                }
            }
        }

        private void InitializeVoices()
        {
            _voices = new List<AudioSource>();
            for (int i = 0; i < _voiceCount; i++)
            {
                _voices.Add(gameObject.AddComponent<AudioSource>());
            }
        }

        private void PlaySoundEffect(string name, float pitch, float volumeModifier)
        {
            if (_effects.ContainsKey(name))
            {
                AudioSource voice = null;
                for (int i = 0; ((i < _voices.Count) && (voice == null)); i++)
                {
                    if (!_voices[i].isPlaying)
                    {
                        voice = _voices[i];
                    }
                }

                if (voice != null)
                {
                    voice.pitch = pitch;
                    voice.volume = Mathf.Clamp01(volumeModifier);
                    voice.clip = _effects[name];
                    voice.Play();
                }
            }
        }

        private void PlayRandomSoundEffect(string nameStem, float pitch, float volumeModifier)
        {
            string[] validNames = _effects.Where(x => x.Key.ToLower().StartsWith(nameStem.ToLower())).Select(x => x.Key).ToArray();
            if (validNames.Any())
            {
                PlaySoundEffect(validNames[Random.Range(0, validNames.Length)], pitch, volumeModifier);
            }
        }

        public void SetMusicActive(bool isActive)
        {
            if (isActive)
            {
                _musicVoice.volume = 1.0f;
                _musicVoice.Play();
            }
            else
            {
                StartCoroutine(FadeOutMusic());
            }
        }

        private IEnumerator FadeOutMusic()
        {
            while (_musicVoice.volume > 0.0f)
            {
                _musicVoice.volume = Mathf.Clamp01(_musicVoice.volume - Time.fixedDeltaTime * 4.0f);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}