// --- Assets/Scripts/Core/GameInitializer.cs ---
using UnityEngine;
using DinoRunner.Core;
using DinoRunner.Services;
using DinoRunner.Interfaces;

namespace DinoRunner
{
    public class GameInitializer : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameConfig gameConfig;
        [Tooltip("Assign the prefab or GameObject containing MobileInputService")]
        [SerializeField] private GameObject mobileInputServicePrefab; // Assign prefab/GO here

        private void Awake()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            if (gameConfig != null)
            {
                Application.targetFrameRate = gameConfig.targetFrameRate;
            }
            else { Debug.LogError("[GameInitializer] GameConfig not assigned!"); }

            var serviceLocator = ServiceLocator.Instance; // Ensures ServiceLocator is created first

            // Initialize and Register Input Service based on Platform
            #if UNITY_ANDROID || UNITY_IOS
                MobileInputService mobileInputInstance = FindObjectOfType<MobileInputService>();
                if (mobileInputInstance == null && mobileInputServicePrefab != null)
                {
                    GameObject go = Instantiate(mobileInputServicePrefab);
                    mobileInputInstance = go.GetComponent<MobileInputService>();
                    Debug.Log("[GameInitializer] Instantiated MobileInputService.");
                }
                else if(mobileInputInstance == null)
                {
                    Debug.LogError("[GameInitializer] MobileInputService instance not found and prefab not assigned!");
                }
                // Registration happens in MobileInputService.Awake via Singleton pattern
                IInputService inputService = MobileInputService.Instance; // Force instance check/creation
                Debug.Log("[GameInitializer] Using MobileInputService.");
            #else
                if (FindObjectOfType<InputService>() == null)
                {
                    GameObject inputGO = new GameObject("InputService");
                    inputGO.AddComponent<InputService>(); // Will auto-register in Awake
                }
                IInputService inputService = InputService.Instance; // Force instance check/creation
                Debug.Log("[GameInitializer] Using Standard InputService.");
            #endif

            // Force initialization/registration of other essential services
            var audioService = AudioService.Instance;
            var saveService = SaveService.Instance;
            var scoreService = ScoreService.Instance;

            // Initialize GameManager last, as it might depend on other services
            var gameManager = GameManager.Instance;

            Debug.Log("[GameInitializer] Core systems initialized for platform: " + Application.platform);
        }
    }
}