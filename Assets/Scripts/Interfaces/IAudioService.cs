using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IAudioService
    {
        void PlaySound(string soundName, float volume = 1f);
        void PlaySoundAt(string soundName, Vector3 position, float volume = 1f);
        void PlayMusic(string musicName);
        void StopMusic();
        void SetMasterVolume(float volume);
        void SetMusicVolume(float volume);
        void SetSFXVolume(float volume);
    }
}