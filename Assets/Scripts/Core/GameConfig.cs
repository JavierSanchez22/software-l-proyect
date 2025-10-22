using UnityEngine;

namespace DinoRunner.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "DinoRunner/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Game Settings")]
        public int targetFrameRate = 60;
        public float gravity = -20f;

        [Header("Audio Settings")]
        public float defaultMusicVolume = 0.7f;
        public float defaultSFXVolume = 0.8f;

        [Header("Sharing Settings (Original GameManager)")]
        [TextArea(3, 10)]
        public string shareText = "IDontWantAPlayThisF*kinGame (Just Kidding! This Game is Awesome!)";
        public string shareUrl = "https://giftosoftware.netlify.app";
    }
}