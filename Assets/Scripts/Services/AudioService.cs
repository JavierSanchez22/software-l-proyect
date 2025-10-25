using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Patterns.Singleton;
using Assets.Scripts.Core; 


namespace Assets.Scripts.Services
{
    public class AudioService : SingletonMonoBehaviour<AudioService>,
    IAudioService
    {
        [Header("Audio Sources (Assign or create dynamically)")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource; // General SFX
        [SerializeField] private AudioSource playerFootstepSource; // Specific sources if needed
        [SerializeField] private AudioSource playerJumpSource;
        [SerializeField] private AudioSource coinSource;

        // Add more specific sources as needed (e.g., enemy sounds, UI sounds)
        [Header("Audio Clips (Assign in Inspector or load from Resources)")]
        [SerializeField] private List<AudioClipData> musicClips;
        [SerializeField] private List<AudioClipData> sfxClips;
        private Dictionary<string, AudioClip> musicCache = new
        Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> sfxCache = new
        Dictionary<string, AudioClip>();
        private float masterVolume = 1f;
        private float musicVolume = 1f;
        private float sfxVolume = 1f;
        [System.Serializable]
        public class AudioClipData
        {
            public string name; // Identifier used in code (e.g., "jump","coin_collect")

public AudioClip clip;
        }
        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return; // Prevent setup in duplicates
            InitializeSources();
            CacheClips();
            ServiceLocator.Instance.RegisterService<IAudioService>(this);
            // Load volume settings if using ISaveService
            // masterVolume = SaveService.Instance?.LoadGame<float>("masterVolume", 1f) ?? 1f;
            // musicVolume = SaveService.Instance?.LoadGame<float>("musicVolume", 1f) ?? 1f;
            // sfxVolume = SaveService.Instance?.LoadGame<float>("sfxVolume", 1f) ?? 1f;
            ApplyVolumeSettings();
        }

        private void InitializeSources()
        {
            // If sources aren't assigned, create them dynamically as children
            if (musicSource == null) musicSource = CreateAudioSource("MusicSource", true);
            if (sfxSource == null) sfxSource = CreateAudioSource("SFXSource", false);
            if (playerFootstepSource == null) playerFootstepSource = CreateAudioSource("PlayerFootstepSource", false);
            if (playerJumpSource == null) playerJumpSource = CreateAudioSource("PlayerJumpSource", false);
            if (coinSource == null) coinSource = CreateAudioSource("CoinSource", false);
            CreateAudioSource("CoinSource", false);
            // Initialize others...
        }

        private AudioSource CreateAudioSource(string name, bool loop)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(this.transform);
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            return source;
        }

        private void CacheClips()
        {
            foreach (var data in musicClips) if (data.clip != null)
                    musicCache[data.name] = data.clip;
            foreach (var data in sfxClips) if (data.clip != null)
                    sfxCache[data.name] = data.clip;
        }

        // --- IAudioService Implementation ---
        public void PlaySound(string soundName, float volumeScale = 1f)
        {
            if (sfxCache.TryGetValue(soundName, out AudioClip clip))
            {
                // Use a specific source if logic dictates, otherwise usegeneral SFX source
                AudioSource sourceToUse = GetSourceForSound(soundName);
                if (sourceToUse != null)
                {
                    sourceToUse.PlayOneShot(clip, volumeScale);
                }
            }
            else Debug.LogWarning($"[AudioService] SFX Clip not found:{ soundName}");
        }

        public void PlaySoundAt(string soundName, Vector3 position, float
        volumeScale = 1f)
        {
            if (sfxCache.TryGetValue(soundName, out AudioClip clip))
            {
                AudioSource.PlayClipAtPoint(clip, position, sfxVolume *
                masterVolume * volumeScale);
            }
            else Debug.LogWarning($"[AudioService] SFX Clip not found:{ soundName}");
        }

        public void PlayMusic(string musicName)
        {
            if (musicCache.TryGetValue(musicName, out AudioClip clip))
            {
                if (musicSource.clip == clip && musicSource.isPlaying) return;
                // Already playing this track
                musicSource.clip = clip;
                musicSource.Play();
            }
            else Debug.LogWarning($"[AudioService] Music Clip not found:{ musicName}");
        }

        public void StopMusic()
        {
            musicSource?.Stop();
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            // Save setting potentially:
            SaveService.Instance?.SaveGame("masterVolume", masterVolume);
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            // Save setting: SaveService.Instance?.SaveGame("musicVolume",musicVolume);
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            // Save setting: SaveService.Instance?.SaveGame("sfxVolume",sfxVolume);
        }

        private void ApplyVolumeSettings()
        {
            // Apply volume respecting master volume
            if (musicSource != null) musicSource.volume = musicVolume * masterVolume;
            if (sfxSource != null) sfxSource.volume = sfxVolume * masterVolume;
            if (playerFootstepSource != null) playerFootstepSource.volume = sfxVolume * masterVolume;
            if (playerJumpSource != null) playerJumpSource.volume = sfxVolume * masterVolume;
            if (coinSource != null) coinSource.volume = sfxVolume * masterVolume;
            // Apply to other sources...
            // Also update global AudioListener volume (can be redundant ifusing masterVolume)
            // AudioListener.volume = masterVolume;
}

        // Helper to decide which AudioSource to use (expand with more logic)
        private AudioSource GetSourceForSound(string soundName)
        {
            switch (soundName)
            {
                case "footstep_grass": // Example specific names
                case "footstep_stone":
                    return playerFootstepSource;
                case "jump":
                case "land":
                    return playerJumpSource;
                case "coin_collect":
                case "chest_open":
                    return coinSource;
                case "button_click":
                    return sfxSource; // Or a dedicated UI source
                                      // Add cases for enemy sounds, hazards etc.
                default:
                    return sfxSource; // Default general source
            }
        }


        protected override void OnDestroy()
        {
            if (ServiceLocator.Instance != null &&
            ServiceLocator.Instance.HasService<IAudioService>() && ServiceLocator.Instance.GetService<IAudioService>() == this)
{
                ServiceLocator.Instance.UnregisterService<IAudioService>();
            }
            base.OnDestroy();
        }
    }
}
